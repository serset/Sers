package Sers.Core.Module.Api.ApiDesc;

import Sers.Core.Util.Data.ArraySegment;

public class SsModel {
	
	public interface IOnDeserialize {
		Object[] deserialize(ArraySegment bytes);
	}

	private IOnDeserialize onDeserialize;
	
	public void onDeserialize_Set(IOnDeserialize onDeserialize) {
		this.onDeserialize=onDeserialize;
	}
	
	
	 public Object[] deserialize(ArraySegment  bytes)
     {
		 return onDeserialize.deserialize(bytes);        
     }
	 
	  //region JsonProperty
		
		/**
		 *  数据类型。 可为 String、int32、int64、float、double、bool、datetime 或 SsModelEntity的name
		 */
		public String type;
		
		/** 
		 *   数据模式。只可为 value、object、array
		 */
		public String mode;
		
		/**
		 *  描述
		 */
		public String description;
		
		/**
		 *  默认值
		 */
		public Object defaultValue;
		
		/**
		 *  示例值
		 */ 
		public Object example;		
		
		 
		public SsModelEntity[] models;

      //endregion
}
