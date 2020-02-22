namespace Sers.CL.Ipc.SharedMemory.Stream
{
    internal class Util
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">共享内存名称</param>
        /// <param name="nodeCount">共享内存节点个数</param>
        /// <param name="nodeBufferSize">共享内存节点大小</param>
        /// <returns></returns>
        public static global::SharedMemory.CircularBuffer SharedMemory_Malloc(string name, int nodeCount, int nodeBufferSize)
        {             
            string ipcName = "sers.ipc." + name;
            return new global::SharedMemory.CircularBuffer(name: ipcName, nodeCount: nodeCount, nodeBufferSize: nodeBufferSize);
        }

        public static global::SharedMemory.CircularBuffer SharedMemory_Attach(string name)
        {
            string ipcName = "sers.ipc." + name;
            return new global::SharedMemory.CircularBuffer(name: ipcName);
        }

    }
}
