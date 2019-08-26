package Test;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Util.ConfigurationManager.ConfigurationManager;
import Sers.Core.Util.Data.ArraySegment;
import Sers.Mq.SocketMq.ClientMq;
import Sers.Mq.SocketMq.ClientMqConfig;

import java.util.ArrayList;

public class SocketMqTest {

    public static void Test() {


        try {

            Logger.onLog=(String level,String msg)->{
                System.out.println("["+level+"]"+msg);
            };

     
            ClientMq mq=new ClientMq();

            mq.config=ConfigurationManager.Instance.GetByPath("Sers.Mq.Socket",ClientMqConfig.class);
            
            mq.OnReceiveRequest_Set(
                    (ArraySegment req)->{
                        ArrayList<ArraySegment> reply=new ArrayList<ArraySegment>();
                        reply.add(req);
                        return  reply;
                    }
            );

            mq.Connect();
            Thread.sleep(1000000);
        } catch (Exception e) {
            Logger.Error(e);
        }

    }

}
