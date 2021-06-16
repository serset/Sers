package Sers.Core.Module.Api.Data;

import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDescription;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsExample;
import Sers.Core.Util.SsError.SsError;

public class ApiReturnBase {
	/**
	 * 是否成功
	 */
	@SsDescription("是否成功")
	@SsExample("true")
	public boolean success = true;

	/**
	 * 错误信息
	 */
	@SsDescription("错误信息")
	public SsError error;
	
	public ApiReturnBase() {
		
	}
	
	public ApiReturnBase(SsError error) {
		success=false;
		this.error=error;
	}
}
