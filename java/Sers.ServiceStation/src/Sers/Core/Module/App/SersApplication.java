package Sers.Core.Module.App;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Util.Threading.AutoResetEvent;

public class SersApplication {
	 static AutoResetEvent stopEvent = new AutoResetEvent(false);

     public static boolean IsRunning = false;
     

     public static void Stop()
     {
         IsRunning = false;


         stopEvent.set();

         try
         {
        	 System.exit(0);
         }
         catch (Exception ex)
         {
             Logger.Error(ex);
         }
     }



     /// <summary>
     /// 强制控制台不退出，除非执行Stop()
     /// </summary>
     public static void RunAwait() throws InterruptedException
     {
         if (!IsRunning) return;
         stopEvent.reset();
         stopEvent.waitOne();
     }

}
