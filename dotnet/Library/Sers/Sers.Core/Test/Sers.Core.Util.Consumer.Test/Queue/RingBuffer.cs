using System.Threading;

namespace Sers.Core.Util.PubSub.Test.Queue
{
    public class RingBuffer<T> where T : class
    {

        T[] buffer;
        int power;
        int length;
        int mouban;
        public RingBuffer(int power = 20)
        {
            this.power = power;
            this.length = 2 << power;
            buffer = new T[length];


            mouban = 1;
            while (--power > 0)
            {
                mouban <<= 1;
                mouban++;
            }


        }


        int headIndex = 0;
        int tailIndex = 0;

        public void Publish(T t)
        {
            var index = Interlocked.Increment(ref headIndex);

            buffer[index & mouban] = t;
        }




        public T Take()
        {
            var index = Interlocked.Increment(ref tailIndex) & mouban;
            var t = buffer[index];
            buffer[index] = null;

            return t;
        }


    }
}
