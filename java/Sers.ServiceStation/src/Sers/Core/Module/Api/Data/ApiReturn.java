package Sers.Core.Module.Api.Data;

import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDescription;
import Sers.Core.Util.SsError.SsError;

public class ApiReturn<T> extends ApiReturnBase {
	
	/**
	 * 数据
	 */
	@SsDescription("数据")
	public T data;

	public ApiReturn() {
		super();
	}

	public ApiReturn(SsError error) {
		super(error);
	}

	public ApiReturn(T data) {
		super();
		this.data = data;
	}

}
