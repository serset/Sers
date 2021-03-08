using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Extensions;

namespace Sers.Core.CL.MessageOrganize.DefaultOrganize
{
    public class OrganizeClient : IOrganizeClient
    {
        public IOrganizeConnection conn { get; }


        private IDeliveryClient delivery;

        readonly RequestAdaptor requestAdaptor;


        /// <summary>
        /// 连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        /// </summary>
        private string secretKey;


        public OrganizeClient(IDeliveryClient delivery, JObject config)
        {
            this.delivery = delivery;
            requestAdaptor = new RequestAdaptor(config);

            conn = new OrganizeConnection(delivery.conn, requestAdaptor);
            requestAdaptor.BindConnection(delivery.conn, conn);

            

            secretKey = config["secretKey"].ConvertToString();

            requestAdaptor.GetConnList = () => {
                return new[] { conn };
            };
        }


        #region  event
        public Action<IOrganizeConnection> Conn_OnDisconnected
        {
            set
            {
                delivery.Conn_OnDisconnected = (deliveryConn)=> { value(conn); };
            }
        }

        /// <summary>
        /// 会在内部线程中被调用 
        /// (conn,sender,requestData,callback)
        /// </summary>
        public Action<IOrganizeConnection,object, ArraySegment<byte>, Action<object, Vit.Core.Util.Pipelines.ByteData>> conn_OnGetRequest
        {
            set
            {
                requestAdaptor.event_OnGetRequest = value;
            }
        }

        public Action<IOrganizeConnection,ArraySegment<byte>> conn_OnGetMessage
        {
            set
            {
                requestAdaptor.event_OnGetMessage = value;
            }
        }
        #endregion

     


        #region Start


        public bool Start()
        {
            requestAdaptor.Start();

            if (!Connect())
            {
                Stop();
                return false;
            }

            return true;
        }


        #endregion

        #region Stop

        public void Stop()
        {
            if (delivery == null) return;

            requestAdaptor.Stop();

            try
            {
                delivery.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            delivery = null;
        }
        #endregion


        #region Connect
       
        bool Connect()
        {
            if (!delivery.Connect())
            {
                return false;
            }


            if (!CheckSecretKey())
            {
                return false;
            }

            conn.GetDeliveryConn().state = DeliveryConnState.certified;

            return true;
        }
        #endregion

        #region CheckSecretKey

        private bool CheckSecretKey()
        {
            var requestData = new ByteData(secretKey.SerializeToBytes());

            //发送身份验证
            Logger.Info("[CL.OrganizeClient] Authentication - sending SecretKey...");

            bool success = false;
            try
            {
                if (conn.SendRequest(requestData, out var replyData) && replyData.ByteDataToString() == "true")
                {
                    success = true;
                    Logger.Info("[CL.OrganizeClient] Authentication - succeed.");
                    return true;
                }
                return false;
            }
            finally
            {
                if (!success)
                {
                    Logger.Info("[CL.OrganizeClient] Authentication - failed.");
                }
            }
            
        }

        #endregion

    }
}
