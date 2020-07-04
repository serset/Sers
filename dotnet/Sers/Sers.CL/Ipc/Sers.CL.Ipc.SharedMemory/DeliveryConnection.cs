using System;
using System.Collections.Generic;
using Sers.CL.Ipc.SharedMemory.Stream;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.CL.Ipc.SharedMemory
{
    public class DeliveryConnection : IDeliveryConnection
    {

        public DeliveryConnection()
        {
            readStream = new ReadStream() { conn=this};
            writeStream = new WriteStream();
        }


        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _securityManager = value; }
        Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;


        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame {
            set
            {
                if (_securityManager != null)
                {
                    value =
                        (conn, data) => { _securityManager.Decryption(data); }
                    + value;
                }

                readStream.OnReceiveMessage = value;
            }
        }


        public Action<IDeliveryConnection> OnDisconnected { set => readStream.OnDisconnected = value; }

        WriteStream writeStream;
        ReadStream readStream;



        public bool InitAsServer(string memoryName,int nodeCount,int nodeBufferSize)
        {
            if (!writeStream.SharedMemory_Malloc(memoryName + ".ServerToClient", nodeCount, nodeBufferSize))
            {               
                return false;
            }

            if (!readStream.SharedMemory_Malloc(memoryName + ".ClientToServer", nodeCount, nodeBufferSize))
            {               
                return false;
            }
            return true;
        }


        public bool InitAsClient(string memoryName)
        {
            if (!writeStream.SharedMemory_Attach(memoryName + ".ClientToServer"))
            {              
                return false;
            }

            if (!readStream.SharedMemory_Attach(memoryName + ".ServerToClient"))
            {           
                return false;
            }
            return true;
        }


        public bool Start()
        {
            if (!readStream.Start())
            {              
                return false;
            }
            if (!writeStream.Start())
            {
                
                return false;
            }
            return true;
        }


        public void Close()
        {
            state = DeliveryConnState.closed;

            try
            {
                writeStream.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                readStream.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            var bytes = data.ByteDataToBytes();
            _securityManager?.Encryption(bytes.BytesToArraySegmentByte());

            writeStream.SendMessageAsync(bytes);
        }
    }
}
