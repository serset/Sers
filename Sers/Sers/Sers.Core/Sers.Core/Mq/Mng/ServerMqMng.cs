using System;
using System.Collections.Generic;
using Sers.Core.Util.Threading;

namespace Sers.Core.Mq.Mng
{
    /// <summary>
    /// mode : single,multi
    /// </summary>
    public class ServerMqMng :IDisposable
    {
     

        public void AddMqBuilder(Func<IServerMq> builder)
        {
            mqBuilderList.Add(builder);
        }
        public void AddMqBuilder(IServerMqBuilder builder)
        {
            mqBuilderList.Add(builder.BuildMq);
        }
       
        List<Func<IServerMq>> mqBuilderList = new List<Func<IServerMq>>();

        //List<IServerMq> mqList = new List<IServerMq>();


        IServerMq _mq;
        //public virtual IServerMq serverMq => _mq;

        void UpdateAction()
        {

            if (null != _mq)
            {
                if (null != _Conn_OnConnected) _mq.Conn_OnConnected = _Conn_OnConnected;
                if (null != _Conn_OnDisconnected) _mq.Conn_OnDisconnected = _Conn_OnDisconnected;
                if (null != _OnException) _mq.OnException = _OnException;
                if (null != _OnReceiveRequest) _mq.OnReceiveRequest = _OnReceiveRequest;
                if (null != _OnReceiveMessage) _mq.OnReceiveMessage = _OnReceiveMessage;                
            }
        }

        #region delegate

      
        Action<string> _Conn_OnConnected;
       
        public Action<string> Conn_OnConnected
        {
            get => _Conn_OnConnected;
            set {
                _Conn_OnConnected = value;
                UpdateAction();
            }
        }

        Action<string> _Conn_OnDisconnected;
        public Action<string> Conn_OnDisconnected
        {
            get => _Conn_OnDisconnected;
            set
            {
                _Conn_OnDisconnected = value;
                UpdateAction();
            }
        }

        Action<Exception> _OnException;
        public Action<Exception> OnException
        {
            get => _OnException;
            set
            {
                _OnException = value;
                UpdateAction();
            }
        }

       

        #endregion


        public bool Start()
        {
            foreach (var builder in mqBuilderList)
            {
                try
                {
                    var mq = builder?.Invoke();
                    if (mq != null && mq.Start())
                    {
                        _mq = mq;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);
                }
            }
            if (null == _mq) return false;

            UpdateAction();
            return true;
        }
        public void Close()
        {
            _mq?.Close();
        }


        public List<string> GetConnGuidList() => _mq.GetConnGuidList();


        #region ReqRep       
        public bool SendRequest(string connGuid, List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData ) => _mq.SendRequest(connGuid, requestData,out replyData);


        Func<string, ArraySegment<byte>, List<ArraySegment<byte>>> _OnReceiveRequest;
        public Func<string, ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest
        {
            get => _OnReceiveRequest;
            set
            {
                _OnReceiveRequest = value;
                UpdateAction();
            }
        }
        #endregion

        #region Message

        public void SendMessage(string connGuid, List<ArraySegment<byte>> msgData)
        => _mq.SendMessage(connGuid, msgData);

        Action<string,ArraySegment<byte>> _OnReceiveMessage;
        public Action<string, ArraySegment<byte>> OnReceiveMessage
        {
            get => _OnReceiveMessage;
            set
            {
                _OnReceiveMessage = value;
                UpdateAction();
            }
        }
        #endregion


        ~ServerMqMng()
        {
            Dispose();
        }
        public void Dispose()
        {
            Close();
            _mq?.Dispose();
        }

     


        #region static CurMqConnGuid

        static AsyncCache<string> curMqConnGuid = new AsyncCache<string>();

        /// <summary>
        /// 当前消息队列的连接唯一码
        /// </summary>
        public static string CurMqConnGuid { get => curMqConnGuid.Value; set => curMqConnGuid.Value = value; }

        
   
    
        
        #endregion
    }
}
