package Sers.Core.Util.SsError;

public class SsError {

    public Integer  errorCode;

    public String errorMessage;

    public String errorTag;


//    public JObject errorDetail;

/*
    /// <summary>
    ///
    /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            [SsExample("1000")]
    public int? errorCode { get; set; }


    /// <summary>
    ///
    /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            [SsExample("操作出现异常")]
    public string errorMessage { get; set; }


    /// <summary>
    /// 自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"
    /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            [SsExample("150721_lith_1")]
            [SsDescription("自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如：\"150721_lith_1\"")]
    public string errorTag { get; set; }



    /// <summary>
    /// 错误详情（json类型）
    /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            [SsExample("{}")]
            [SsDescription("错误详情（json类型）")]
    public JObject errorDetail { get; set; }

    //*/
}
