import Sers.ServiceStation.ServiceStation;




public class Main {





    public static void main(String[] args) { 

    	//(x.1) init
    	ServiceStation.Instance.InitStation();
    	
    	//(x.2) Discovery api
    	try {
			ServiceStation.Instance.Discovery();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return;
		}
    	
    	//(x.3) StartStation
    	ServiceStation.Instance.StartStation();
    	
    	//(x.4) RunAwait
    	ServiceStation.Instance.RunAwait();
    	
    	  
    }
}
