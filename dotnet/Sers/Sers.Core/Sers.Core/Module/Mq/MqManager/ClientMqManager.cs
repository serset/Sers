using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using System;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.Extensions;
using Sers.Core.Util.ConfigurationManager;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Sers.Core.Util.Common;

namespace Sers.Core.Module.Mq.MqManager
{
    public class ClientMqManager
    {
        public IMqConn[] mqConns { get; private set; }

        IClientMq[] mqs;

        readonly RequestAdaptor requestAdaptor = new RequestAdaptor();

        public ClientMqManager()
        {
            requestAdaptor.GetConnList = () => {
                return mqConns;
            };
        }



        #region  event

 




        public Action<IMqConn> Conn_OnDisconnected { get; set; }

        /// <summary>
        /// 会在内部线程中被调用 
        /// (IMqConn conn,Object sender, ArraySegment&lt;byte&gt; requestData,  Action&lt;object, ArraySegment&lt;byte&gt;&gt; callback)
        /// </summary>
        public Action<IMqConn , object, ArraySegment<byte>, Action<object, List<ArraySegment<byte>>>> station_OnGetRequest {
            set
            {
                requestAdaptor.station_OnGetRequest = value;
            }
        }

        public Action<IMqConn, ArraySegment<byte>> station_OnGetMessage
        {
            set
            {
                requestAdaptor.station_OnGetMessage = value;
            }
        }
        #endregion

        #region Station_SendMessage Station_SendRequest      
        public void Station_SendMessageAsync(List<ArraySegment<byte>> message)
        {
            foreach (var mqConn in mqConns)
                requestAdaptor.Station_SendMessageAsync(mqConn, message);
        }

        //public void Station_SendRequestAsync(Object sender, List<ArraySegment<byte>> requestData, Action<object, ArraySegment<byte>> callback,IMqConn mqConn)
        //{
        //    requestAdaptor.Station_SendRequestAsync(mqConn, sender,requestData, callback);
        //}

        public bool Station_SendRequest(List<ArraySegment<byte>> requestData, out List<ArraySegment<byte>> replyData, IMqConn mqConn)
        {
            return requestAdaptor.Station_SendRequest(requestData,out replyData, mqConn);
        }
        
        #endregion

 
 

        #region Start


        public bool Start()
        {
            requestAdaptor.Start();

            if (!BuildMqAndConnect())
            {
                Stop();
                return false;
            }

            mqConns = mqs.Select(mq => mq.mqConn).ToArray();
            return true;
        }

       
        private bool BuildMqAndConnect()
        {
            #region (x.1)build mq           
            mqs = BuildMq();
            if (mqs == null || mqs.Length == 0) return false;
            #endregion


            #region (x.2)set event
            foreach (var mq in mqs)
            {
                mq.OnGetFrame = requestAdaptor.MqToStation_PushMessageFrame;
                mq.Conn_OnDisconnected = MqConn_OnDisconnected;
            }
            #endregion


            #region (x.3)mq Connect
            foreach (var mq in mqs)
            {
                try
                {
                    if (!mq.Connect())
                    {
                        return false;
                    }

                    if (!CheckSecretKey(mq.mqConn, mq.secretKey))
                    {
                        return false;
                    }
                    mq.mqConn.state = MqConnState.certified;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return false;
                }
            }
            #endregion

            return true;
        }


        #region BuildMq
        /// <summary>
        /// (JObject[]configs, List&lt;IClientMq&gt;mqList)=>{}
        /// </summary>
        public Action<JObject[], List<IClientMq>> BeforeBuildMq = null;
        private IClientMq[] BuildMq()
        {
            try
            {
                List<IClientMq> mqList = new List<IClientMq>();

                //(x.1) get configs
                var configs = ConfigurationManager.Instance.GetByPath<JObject[]>("Sers.Mq.ClientMqBuilder");

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
                    var clientBuilder = assembly.CreateInstance(className) as IClientMqBuilder;
                

                    //(x.x.4) build mq
                    clientBuilder?.BuildMq(mqList, config);
                }
                return mqList.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
        #endregion

        private bool CheckSecretKey(IMqConn conn, string secretKey)
        {

            var requestData = secretKey.SerializeToBytes().BytesToByteData();

            //发送身份验证
            Logger.Info("[ClientMq] Authentication - sending SecretKey...");        

            if (Station_SendRequest(requestData, out var replyData, conn) && replyData.ByteDataToString() == "true")
            {
                Logger.Info("[ClientMq] Authentication - succeed.");
                return true;
            }
            else
            {
                Logger.Info("[ClientMq] Authentication - failed.");
                return false;
            }
        }


        #endregion

        #region Stop

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
                    mq.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion


        #region MqConn_OnDisconnected
        private void MqConn_OnDisconnected(IMqConn conn)
        {         
            try
            {    
                Conn_OnDisconnected(conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            try
            {
                mqConns = mqConns?.Where(m => m != conn).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion


    }
}
