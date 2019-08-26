using System;
using System.Collections.Generic;

namespace Sers.Core.Mq
{
    public interface IClientMq: IDisposable
    {
 
        Action Conn_OnDisconnected {  set; }


        bool IsConnected { get; }

        bool Connect();

        void Close();


        #region ReqRep       

        ArraySegment<byte> SendRequest( List<ArraySegment<byte>> request);

        /// <summary>
        /// 接受到请求（处理请求）的回调
        /// List<ArraySegment<byte>> OnReceiveRequest(ArraySegment<byte> oriData)
        /// </summary>
        Func< ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest {  set; }
        #endregion




        #region Message

        void SendMessage(List<ArraySegment<byte>> msgData);

        Action<ArraySegment<byte>> OnReceiveMessage { get; set; }
        #endregion




    }
}
