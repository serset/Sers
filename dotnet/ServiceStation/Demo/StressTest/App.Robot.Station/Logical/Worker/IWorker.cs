namespace App.Robot.Station.Logical.Worker
{
    public interface IWorker
    {
        int RunningThreadCount { get; }

        bool IsRunning { get; }


        //TaskConfig config { get;  }

        void Start();

        void Stop();
    }
}
