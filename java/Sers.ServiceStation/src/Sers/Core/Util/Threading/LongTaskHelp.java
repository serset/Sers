package Sers.Core.Util.Threading;

public class LongTaskHelp    {

    public Runnable action;

    public int threadCount =1;


    Thread []threads;

    public void start()
    {
        threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            Thread thread = new Thread(action);
            threads[i] = thread;
            thread.start();
        }
    }


    public void stop()
    {

        if (null != threads)
        {
            for(Thread thread : threads)
            {
                try
                {
                    if(thread.isAlive()){
                        thread.interrupt();
                    }
                }
                catch (Exception ex){

                }
            }
        }

        //if (semaphore.WaitOne(1000))
        //{
        //    semaphore.Release();
        //}

    }

}
