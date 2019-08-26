package Sers.Core.Util.ConfigurationManager;

import Sers.Core.Util.Common.CommonHelp;

public class ConfigurationManager extends JsonFile {
	
	public final static ConfigurationManager Instance=new ConfigurationManager();
	
	private ConfigurationManager() {
		super(CommonHelp.GetAbsPathByRealativePath("appsettings.json"));
	}
}
