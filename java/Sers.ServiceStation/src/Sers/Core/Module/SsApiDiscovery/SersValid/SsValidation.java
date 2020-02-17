package Sers.Core.Module.SsApiDiscovery.SersValid;

import Sers.Core.Util.SsError.SsError;

public class SsValidation {
	
    //  {"path":"user.userType","ssError":{}, "ssValid":{"type":"Equal","value":"Logined"} }

    public String path;
    public SsError ssError;
    public Object ssValid;
}
