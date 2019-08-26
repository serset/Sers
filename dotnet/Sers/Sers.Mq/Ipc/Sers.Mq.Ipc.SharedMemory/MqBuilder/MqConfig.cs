namespace Sers.Mq.Ipc.SharedMemory.MqBuilder
{
    class MqConfig
    { 

        /// <summary>
        /// 共享内存名称
        /// </summary>
        public string name;

        /// <summary>
        /// 共享内存节点个数
        /// </summary>
        public int nodeCount;

        /// <summary>
        /// 共享内存节点大小
        /// </summary>
        public int nodeBufferSize;

        /// <summary>
        /// Mq连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        /// </summary>
        public string secretKey;

    }
}
