using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;

namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Demo
{
    public class ApiModel_User
    {

        /// <summary>
        /// 用户手机号
        /// </summary>        
        [SsName("mobile"), SsDescription("用户手机号")]
        [SsDefaultValue("15000000000"), SsExample("15012345678")]
        [   SsMaxValue(15000000000, errorMessage = "手机号格式不正确,不是11位"),
            SsMinValue(10000000000, errorMessage = "手机号格式不正确,不是11位"),
            SsEqual("15012345678", errorMessage = "手机号必须为15012345678"),
            SsRegex("^\\d{11}$", errorMessage = "手机号格式不正确,不是11位"),
            SsRequired(errorMessage = "手机号不能为空"),
            SsSize(min = 11, max = 11, errorMessage = "手机号格式不正确,不是11位")]
        public string Mobile { get; set; }


        [SsRoute("ApiStation1/path1/api1")]
        public void GetMobile()
        {

        }

        /// <summary>
        /// 会被忽略的成员
        /// </summary>
        [System.Runtime.Serialization.IgnoreDataMember]
        public string WillIgnore { get; set; }




        /// <summary>
        /// 待检测使用的成员
        /// </summary>
        [SsType(EValueType.String)]
        [System.ComponentModel.Category("cccc")]
        [System.ComponentModel.ReadOnly(true)]
        [System.ComponentModel.DataAnnotations.Range(1, 2)]
        [Required(ErrorMessage = "手机号不能为空"), RegularExpression("^\\d{11}$", ErrorMessage = "手机号格式不正确,不是11位")]
        [DefaultValue("15000000000"), Description("用户手机号")]
        public string Tel { get; set; }



        /// <summary>
        /// Ss弃用的成员
        /// </summary>
        [SsDefaultValue("15000000000"), SsDescription("用户手机号")]         
     
        public string Droped { get; set; }
    }
}
