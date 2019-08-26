using System;
using System.Collections.Generic;
using Sers.Core.Util.Threading;

namespace Sers.Core.Mq.Mng
{
    /// <summary>
    ///
    /// </summary>
    public class ClientMqMng :IDisposable
    {
        public void AddMqBuilder(Func<IClientMq> builder)
        {
            mqBuilderList.Add(builder);
        }
        public void AddMqBuilder(IClientMqBuilder builder)
        {
            mqBuilderList.Add(builder.BuildMq);
        }
       
        List<Func<IClientMq>> mqBuilderList = new List<Func<IClientMq>>();




        IClientMq _mq; 



        void UpdateAction()
        {

            if (null != _mq)
            {
                if (null != _Conn_OnDisconnected) _mq.Conn_OnDisconnected = _Conn_OnDisconnected;
                if (null != _OnException) _mq.OnException = _OnException;
                if (null != _OnReceiveRequest) _mq.OnReceiveRequest = _OnReceiveRequest;
                if (null != _OnReceiveMessage) _mq.OnReceiveMessage = _OnReceiveMessage;                
            }
        }



        public bool Connect()
        {
            foreach (var builder in mqBuilderList)
            {
                try
                {
                    var mq = builder?.Invoke();
                    if (mq != null && mq.Connect())
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
        public bool IsConnected => null == _mq ? false : _mq.IsConnected;


        #region delegate
        

        Action _Conn_OnDisconnected;
        public Action Conn_OnDisconnected
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




        #region ReqRep
       
        public ArraySegment<byte> SendRequest(List<ArraySegment<byte>> request) => _mq.SendRequest(request);

        Func<ArraySegment<byte>, List<ArraySegment<byte>>> _OnReceiveRequest;
        public Func<ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest
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

        public void SendMessage(List<ArraySegment<byte>> msgData)
        {
            _mq.SendMessage(msgData);
        }

        Action<ArraySegment<byte>> _OnReceiveMessage;
        public Action<ArraySegment<byte>> OnReceiveMessage
        {
            get => _OnReceiveMessage;
            set
            {
                _OnReceiveMessage = value;
                UpdateAction();
            }
        }
        #endregion



        ~ClientMqMng()
        {
            Dispose();
        }
        public void Dispose()
        {
            Close();
            _mq?.Dispose();
            _mq = null;
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
