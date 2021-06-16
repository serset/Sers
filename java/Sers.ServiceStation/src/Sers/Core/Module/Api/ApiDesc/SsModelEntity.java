package Sers.Core.Module.Api.ApiDesc;

public class SsModelEntity {

    /**
         * 数据类型。可以唯一定位到一个模型
     */
    public String type;

    /**
         * 数据模式。只可为 value、object、array   
     */
    public String mode;

 
    public SsModelProperty[] propertys;
}
