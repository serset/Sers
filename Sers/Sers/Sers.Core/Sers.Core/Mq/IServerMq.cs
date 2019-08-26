using System;
using System.Collections.Generic;

namespace Sers.Core.Mq
{
    public interface IServerMq : IDisposable
    {

        Action<string> Conn_OnConnected { get; set; }
        Action<string> Conn_OnDisconnected { get; set; }

        Action<Exception> OnException { get; set; }

        bool IsRunning { get; }

        bool ConnIsOnline(string connGuid);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>是否成功启动了服务。 true:成功启动服务 false：服务已启动 或 服务启动失败 </returns>
        bool Start();

   

        void Close();

        List<string> GetConnGuidList();


        #region ReqRep

       
        bool SendRequest(string connGuid, List<ArraySegment<byte>> requestData,out ArraySegment<byte> replyData);


        /// <summary>
        /// 接受到请求（处理请求）的回调
        /// string connGuid,ArraySegment<byte> requestData, ArraySegment<byte> replyData
        /// </summary>
        Func<string, ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest { get; set; }

        #endregion


        #region Message

        /// <summary>
        /// 若失败则代表已下线
        /// </summary>
        /// <param name="connGuid"></param>
        /// <param name="msgData"></param>
        /// <returns></returns>
        void SendMessage(string connGuid, List<ArraySegment<byte>> msgData);

        /// <summary>
        ///  void OnReceiveMessage( string connGuid,ArraySegment<byte> msgData)
        /// </summary>
        Action<string, ArraySegment<byte>> OnReceiveMessage { get; set; }
        #endregion


 
    }
}
