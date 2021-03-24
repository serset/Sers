using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageDelivery;
using Sers.Core.Module.Message;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.Core.CL.MessageOrganize.DefaultOrganize
{
    public class OrganizeServer : IOrganizeServer
    {

        readonly IDeliveryServer delivery;
        readonly RequestAdaptor requestAdaptor;

        /// <summary>
        /// 连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        /// </summary>
        readonly string secretKey;

        public OrganizeServer(IDeliveryServer delivery, JObject config)
        {
            this.delivery = delivery;

            secretKey = config["secretKey"].ConvertToString();

            requestAdaptor = new RequestAdaptor(config);

            requestAdaptor.GetConnList = () =>
            {
                return connMap.Values;
            };



            delivery.Conn_OnConnected = (deliveryConn) =>
            {
                var conn = new OrganizeConnection(deliveryConn, requestAdaptor);
                requestAdaptor.BindConnection(deliveryConn, conn);
            };


            delivery.Conn_OnDisconnected = (deliveryConn) =>
            {
                if (connMap.TryRemove(deliveryConn, out var organizeConn))
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            Conn_OnDisconnected(organizeConn);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    });
                }
            };
        }




        public Action<IOrganizeConnection> Conn_OnConnected
        {
            set; get;
        }
        public Action<IOrganizeConnection> Conn_OnDisconnected
        {
            get; set;
        }

        /// <summary>
        /// 已经认证过的连接
        /// </summary>
        ConcurrentDictionary<IDeliveryConnection, IOrganizeConnection> connMap = new ConcurrentDictionary<IDeliveryConnection, IOrganizeConnection>();



        public bool Start()
        {
            lock (this)
            {
                if (isRunning) return true;
                isRunning = true;
            }

            requestAdaptor.Start();
            if (!delivery.Start())
            {
                Stop();
                return false;
            }
            return true;
        }
        bool isRunning = false;

        public void Stop()
        {
            lock (this)
            {
                if (!isRunning) return;
                isRunning = false;
            }

            requestAdaptor.Stop();
            delivery.Stop();
        }




        /// <summary>
        /// 会在内部线程中被调用 
        /// </summary>
        public Action<IOrganizeConnection, object, ApiMessage, Action<object, Vit.Core.Util.Pipelines.ByteData>> conn_OnGetRequest
        {
            set
            {
                requestAdaptor.event_OnGetRequest = (IOrganizeConnection organizeConn, Object sender, ArraySegment<byte> requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback) =>
                {
                    var deliveryConn = organizeConn.GetDeliveryConn();
                    if (deliveryConn.state == DeliveryConnState.certified)
                    {
                        value(organizeConn, sender, new ApiMessage(requestData), callback);
                        return;
                    }

                    if (deliveryConn.state == DeliveryConnState.waitForCertify)
                    {
                        ConnCheckSecretKey(organizeConn, deliveryConn, sender, requestData, callback);
                        return;
                    }
                };
            }
        }

        public Action<IOrganizeConnection, ArraySegment<byte>> conn_OnGetMessage
        {
            set
            {
                requestAdaptor.event_OnGetMessage = value;
            }
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void Station_SendMessageAsync(IOrganizeConnection conn, Vit.Core.Util.Pipelines.ByteData message)
        //{
        //    requestAdaptor.SendMessageAsync(conn, message);
        //}


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void Station_SendRequestAsync(IOrganizeConnection conn, Object sender, Vit.Core.Util.Pipelines.ByteData requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        //{
        //    requestAdaptor.SendRequestAsync(conn, sender, requestData, callback);
        //}



        #region ConnCheckSecretKey

        private void ConnCheckSecretKey(IOrganizeConnection organizeConn, IDeliveryConnection deliveryConn, Object sender, ArraySegment<byte> requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        {
            // 身份验证
            try
            {
                var reqSecretKey = requestData.ArraySegmentByteToString();

                ArraySegment<byte> replyData;
                if (secretKey == reqSecretKey)
                {
                    deliveryConn.state = DeliveryConnState.certified;

                    //验证通过
                    replyData = "true".SerializeToArraySegmentByte();

                    callback?.Invoke(sender, new Vit.Core.Util.Pipelines.ByteData(replyData));

                    #region 新连接 事件
                    connMap[deliveryConn] = organizeConn;
                    Conn_OnConnected?.Invoke(organizeConn);
                    #endregion
                    return;
                }
                else
                {
                    //验证不通过
                    deliveryConn.state = DeliveryConnState.waitForClose;
                    Logger.Info("[CL.OrganizeServer] Authentication - failed！(" + reqSecretKey + ")");
                    replyData = "false".SerializeToArraySegmentByte();
                    callback?.Invoke(sender, new Vit.Core.Util.Pipelines.ByteData(replyData));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            deliveryConn.Close();
        }
        #endregion



    }
}
