using System.Threading.Channels;

namespace Sers.Core.Util.PubSub.Test.Queue
{
    public class Queue_Channel<T>
    {

        Channel<T> channel = Channel.CreateUnbounded<T>();

        public async System.Threading.Tasks.Task PublishAsync(T t)
        {
            await channel.Writer.WriteAsync(t);
        }

        public void Publish(T t)
        {
            PublishAsync(t);
            //channel.Writer.TryWrite(t);
        }


        /// <summary>
        /// true if an item was read; otherwise, false.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryTake(out T item)
        {
            return channel.Reader.TryRead(out item);
        }


        public T Take()
        {
            //if (channel.Reader.WaitToReadAsync().Result)
            //{
            if (channel.Reader.TryRead(out var item)) return item;

            //}
            return default;
        }


    }
}
