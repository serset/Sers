package Sers.Core.Module.Api.ApiDesc;

public class SsModelProperty {

    /**
         * 名称
     */
    public String name;

    /**
         * 数据类型。 可为 string、int32、int64、float、double、bool、datetime 或 SsModelEntity的name
     */
    public String type;

    /**
         * 数据模式。只可为 value、object、array
     */
    public String mode;

    /**
         * 描述
     */
    public String description;

    /**
         * 默认值
     */
    public Object defaultValue;

    /**
         * 示例值
     */
    public Object example;
}
