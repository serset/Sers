using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Util.Pool;

namespace Sers.Mq.Socket.Iocp
{
    public class MqConn : IMqConn
    {

        public SocketAsyncEventArgs receiveEventArgs;
 

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = MqConnState.waitForCertify;

        public string connTag { get; set; }



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件          public delegate void OnGetFrame(IMqConn conn, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> OnGetFrame;


        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            if (data == null) return;

            Int32 len = data.ByteDataCount();
            data.Insert(0, len.Int32ToArraySegmentByte());

            socket.SendAsync(data, SocketFlags.None);
        }
     

        public Action<IMqConn> Conn_OnDisconnected { get; set; }
        public void Close()
        {
            if (socket == null) return;


            state = MqConnState.closed;

            var socket_ = socket;
            socket = null;

          

            try
            {
                socket_.Close();
                socket_.Dispose();

                //socket_.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                Conn_OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        public void Init(global::System.Net.Sockets.Socket socket)
        {
            this.socket = socket;
            connectTime = DateTime.Now;  
        }
 
        /// <summary>
        /// 通信SOCKET
        /// </summary>
        public global::System.Net.Sockets.Socket socket { get;private set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        private DateTime connectTime { get; set; }




        #region buffer

        byte[] buffLen_bytes = new byte[4];

        int buffLen = 0;
        int frameLen = -1;
 


        #region Queue Buff     

        Queue<ArraySegment<byte>> queueBuff = new Queue<ArraySegment<byte>>();

        int QueueBuff_dataLenOfRemoved = 0;


        public void AppendData(ArraySegment<byte> data)
        {

            //(x.1)
            buffLen += data.Count;
            queueBuff.Enqueue(data);

            //(x.2)
            ArraySegment<byte> msgFrame;
            while (QueueBuff_PopMessageFrame(out msgFrame))
            {
                OnGetFrame.Invoke(this, msgFrame);
            }
        }


        private bool QueueBuff_PopMessageFrame(out ArraySegment<byte> msgFrame)
        {
            if (frameLen < 0)
            {
                if (buffLen < 4) return false;

                frameLen = QueueBuff_PopBytes(buffLen_bytes,4).BytesToInt32();
                buffLen -= 4;
            }

            if (frameLen > buffLen)
            {
                return false;
            }

            msgFrame = new ArraySegment<byte>(QueueBuff_PopBytes(DataPool.BytesGet(frameLen), frameLen), 0, frameLen);

            buffLen -= frameLen;
            frameLen = -1;       
            return true;
        }

        #region QueueBuff_PopBytes

     
        private byte[] QueueBuff_PopBytes(byte[] dataToPop, int lenToPop)
        {
           

            int copyedIndex = 0;

            while (copyedIndex < lenToPop)
            {
                int leftCount = lenToPop - copyedIndex;

                var cur = queueBuff.Peek();
                if (QueueBuff_dataLenOfRemoved != 0)
                {
                    cur = new ArraySegment<byte>(cur.Array, cur.Offset + QueueBuff_dataLenOfRemoved, cur.Count - QueueBuff_dataLenOfRemoved);                
                }

                if (cur.Count <= leftCount)
                {
                    //dataToPop 数据长
                    Array.Copy(cur.Array, cur.Offset, dataToPop, copyedIndex, cur.Count);
                    copyedIndex += cur.Count;
                    QueueBuff_dataLenOfRemoved = 0;
                    queueBuff.Dequeue().ReturnToPool();
                }
                else
                {
                    //queueBuff 数据长
                    Array.Copy(cur.Array, cur.Offset, dataToPop, copyedIndex, leftCount);
                    copyedIndex += leftCount;
                    QueueBuff_dataLenOfRemoved += leftCount;
                }


            }
            return dataToPop;
        }
        #endregion

        #endregion





        #region   List buff
        /*
        List<ArraySegment<byte>> ListBuff  = new List<ArraySegment<byte>>();

        public void AppendData___(ArraySegment<byte> data)
        {
            //(x.1)
            ListBuff.Add(data);
            buffLen += data.Count;


            //(x.2)
            ArraySegment<byte> messageFrame;
            while (ListBuff_PopMessageFrame(out messageFrame))
            {
                OnReceiveMessage.Invoke(this, messageFrame);
            }
        }
        private bool ListBuff_PopMessageFrame(out ArraySegment<byte> messageFrame)
        {
            if (frameLen < 0)
            {
                if (buffLen < 4) return false;

                frameLen = ListBuff.ByteDataPopBytes(buffLen_bytes).BytesToInt32();
                buffLen -= 4;
            }

            if (frameLen > buffLen)
            {
                return false;
            }

            if (frameLen == buffLen)
            {
                frameLen = -1;
                buffLen = 0;

                messageFrame = ListBuff.ByteDataToArraySegment();
                ListBuff.Clear();
                return true;
            }

            buffLen -= frameLen;           
            messageFrame= ListBuff.ByteDataPopBytes(new byte[frameLen]).BytesToArraySegmentByte();
            frameLen = -1;
            return true;
        }

        //*/

        #endregion

        #endregion








    }
}
