using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Sers.Core.Util.Common;

namespace Sers.Core.Module.Mq.MqManager
{
    public class ServerMqManager
    {

        #region static CurMqConn

        static AsyncCache<IMqConn> curMqConn = new AsyncCache<IMqConn>();

        /// <summary>
        /// 当前消息队列
        /// </summary>
        public static IMqConn CurMqConn { get => curMqConn.Value; set => curMqConn.Value = value; }
        #endregion



        IServerMq[] mqs;

        /// <summary>
        /// Mq连接秘钥，用以验证连接安全性。服务端和客户端必须一致(appsettings.json :: Sers.Mq.Config.secretKey)
        /// </summary>
        string secretKey;

        public ServerMqManager()
        {
            requestAdaptor.GetConnList = () => {
                return mqs?.SelectMany(mq=> mq.ConnectedList);
            };
        }


        public readonly RequestAdaptor requestAdaptor = new RequestAdaptor(); 


        public Action<IMqConn> Conn_OnConnected { get; set; }

        public Action<IMqConn> Conn_OnDisconnected { get; set; }




        public bool Start()
        {
            secretKey = ConfigurationManager.Instance.GetByPath<string>("Sers.Mq.Config.secretKey");

            requestAdaptor.Start();

            if (!BuildMqAndInit())
            {
                Stop();
                return false;
            }
            return true;
        }


        public void Stop()
        {
            if (mqs == null) return;

            requestAdaptor.Stop();

            var mqs_ = mqs;
            mqs = null;
            foreach (var mq in mqs_)
            {
                try
                {
                    mq.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }      
        }





        #region BuildMqAndInit        
        private bool BuildMqAndInit()
        {

            #region (x.1)build mq
            mqs = BuildMq();
            if (mqs == null || mqs.Length == 0) return false;
            #endregion


            #region (x.2)set event
            foreach (var mq in mqs)
            {
                mq.Conn_OnGetFrame = requestAdaptor.MqToStation_PushMessageFrame;
                mq.Conn_OnDisconnected = MqConn_OnDisconnected;
                //mq.Conn_OnConnected = Conn_OnConnected;
            }
            #endregion


            #region (x.3)mq Start
            foreach (var mq in mqs)
            {
                try
                {
                    if (!mq.Start())
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return false;
                }
            }
            #endregion

            return true;
        }

        #region BuildMq

        /// <summary>
        /// (JObject[]configs, List &lt; IServerMq &gt; mqList)=>{}
        /// </summary>
        public Action<JObject[], List<IServerMq>> BeforeBuildMq = null;

        private IServerMq[] BuildMq()
        {
       
            try
            {
                List<IServerMq> mqList = new List<IServerMq>();

                //(x.1) get configs
                var configs = ConfigurationManager.Instance.GetByPath<JObject[]>("Sers.Mq.ServerMqBuilder");


                //(x.2) BeforeBuildMq
                try
                {
                    BeforeBuildMq?.Invoke(configs, mqList);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }


                //(x.3) build Mq
                foreach (var config in configs)
                {
                    //(x.x.1) get className    
                    var className = config["className"].ConvertToString();
                    if (string.IsNullOrEmpty(className)) continue;

                    #region (x.x.2) get assembly                   
                    Assembly assembly = null;
                    var assemblyFile = config["assemblyFile"].ConvertToString();
                    if (string.IsNullOrEmpty(assemblyFile))
                    {
                        continue;
                    }
                    assembly = Assembly.LoadFrom(CommonHelp.GetAbsPathByRealativePath(assemblyFile));
                    #endregion

                    //(x.x.3) create class
                    var mqBuilder = assembly.CreateInstance(className) as IServerMqBuilder;                    

                    //(x.x.4) build mq
                    mqBuilder?.BuildMq(mqList, config);
                }

                if (mqList == null || mqList.Count == 0) return null;

                return mqList.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
            
        }
        #endregion

        #endregion

        /// <summary>
        /// 会在内部线程中被调用 
        /// (IMqConn conn,Object sender, ArraySegment&lt;byte&gt; requestData,  Action&lt;object, ArraySegment&lt;byte&gt;&gt; callback)
        /// </summary>
        public Action<IMqConn, object, ArraySegment<byte>, Action<object, List<ArraySegment<byte>>>> station_OnGetRequest
        {
            set
            {
                requestAdaptor.station_OnGetRequest = (IMqConn conn, Object sender, ArraySegment<byte> requestData, Action<object, List<ArraySegment<byte>>> callback) =>
                {                     
                    if (conn.state == MqConnState.certified)
                    {
                        value(conn, sender, requestData, callback);
                        return;
                    }

                    if (conn.state == MqConnState.waitForCertify)
                    {
                        ConnCheckSecretKey(conn, sender, requestData, callback);
                        return;
                    }                   
                };
            }
        }

        public Action<IMqConn, ArraySegment<byte>> station_OnGetMessage
        {
            set
            {
                requestAdaptor.station_OnGetMessage = value;
            }
        }


        public void Station_SendMessageAsync(IMqConn mqConn, List<ArraySegment<byte>> message)
        {
            requestAdaptor.Station_SendMessageAsync(mqConn, message);
        }


        public void Station_SendRequestAsync(IMqConn mqConn, Object sender, List<ArraySegment<byte>> requestData, Action<object, List<ArraySegment<byte>>> callback)
        {
            requestAdaptor.Station_SendRequestAsync(mqConn, sender, requestData, callback);
        }



        #region ConnCheckSecretKey

        private void ConnCheckSecretKey(IMqConn conn, Object sender, ArraySegment<byte> requestData, Action<object, List<ArraySegment<byte>>> callback)
        {
            try  // 身份验证
            {
                var reqSecretKey = requestData.ArraySegmentByteToString();
             
                ArraySegment<byte> replyData;
                if (secretKey == reqSecretKey)
                {
                    conn.state = MqConnState.certified;

                    //验证通过
                    replyData = "true".SerializeToArraySegmentByte();

                    callback?.Invoke(sender, new List<ArraySegment<byte>> {  replyData });

                    //新连接 事件
                    //connMap[conn.connGuid] = conn;
                    Conn_OnConnected?.Invoke(conn);
                    return;
                }
                else
                {
                    //验证不通过
                    conn.state = MqConnState.waitForClose;
                    Logger.Info("[ServerMq] Authentication - failed！(" + reqSecretKey + ")");
                    replyData = "false".SerializeToArraySegmentByte();
                    callback?.Invoke(sender, new List<ArraySegment<byte>> { replyData });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            conn.Close();
        }
        #endregion


        #region MqConn_OnDisconnected
        private void MqConn_OnDisconnected(IMqConn conn)
        {
            //if (conn.state!=MqConnState.closed)
            {
                Task.Run(() =>
                {
                    try
                    {
                        //conn.state = MqConnState.closed;
                        Conn_OnDisconnected(conn);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                });
            }
        }
        #endregion

    }
}
