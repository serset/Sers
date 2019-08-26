using System;
using System.Collections.Generic;
using System.Text;

namespace App.Robot.ServiceStation.Logical
{
    public class TaskConfig
    {
        public string name="taskName";

        public string apiRoute;
        public string apiArg;


        public int threadCount=1;
        public int loopCountPerThread=2000;     

 
        /// <summary>
        /// 每次调用时间间隔。（单位:ms）
        /// </summary>
        public int interval = 100;

        public bool autoStart = true;

        /// <summary>
        /// 失败时是否控制台输出
        /// </summary>
        public bool logError = false;

    }
}
