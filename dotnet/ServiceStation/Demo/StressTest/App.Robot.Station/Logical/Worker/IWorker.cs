using System;
using System.Collections.Generic;
using System.Text;

namespace App.Robot.Station.Logical.Worker
{
    public interface IWorker
    {
        int id { get; set; }

        TaskConfig config { get;  }

        void Start();

        void Stop();
    }
}
