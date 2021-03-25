namespace App.Robot.Station.Logical
{
    public class TaskConfig
    {

        /// <summary>
        /// ApiClient、ApiClientAsync、 HttpClient、HttpUtil
        /// </summary>
        public string type = "ApiClient";


        public string name="taskName";

        public string apiRoute;
        public string apiArg;
        public string httpMethod;



        public long threadCount = 1;

        public long loopCountPerThread = 2000;



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
