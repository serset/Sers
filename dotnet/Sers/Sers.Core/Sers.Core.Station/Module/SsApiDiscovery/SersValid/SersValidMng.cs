using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.SsApiDiscovery.SersValid
{
    public class SersValidMng
    {

        public static List<SsValidation> GetRpcValidationsFromMethod(MethodInfo info)
        {
            var attrs = info.GetCustomAttributes<SsValidationBaseAttribute>();

            var validations = new List<SsValidation>();
            foreach (var attr in attrs)
            {
                attr.GetSsValidation(validations);
            }
            if (validations.Count == 0) return null;
            return validations;
        }

        public enum EValidType
        {
            /// <summary>
            /// { "type":"Equal","value":"Logined"}
            /// </summary>
            Equal,
            /// <summary>
            /// { "type":"Regex","value":"^\\d{11}$"}
            /// </summary>
            Regex,
            /// <summary>
            /// { "type":"Required" }
            /// </summary>
            Required,


            /// <summary>
            /// { "type":"Null"}   always true
            /// </summary>
            Null,

            ///// <summary>
            ///// { "type":"MaxValue","value":11}
            ///// </summary>
            //MaxValue,
            ///// <summary>
            ///// { "type":"MinValue","value":11}
            ///// </summary>
            //MinValue,
            ///// <summary>
            ///// { "type":"Size","min":11,"max":11}
            ///// </summary>
            //Size
        }

        /*
       [   
            SsRegex("^\\d{11}$", ErrorMessage = "手机号格式不正确,不是11位"),
            SsEqual("15012345678", ErrorMessage = "手机号必须为15012345678"),
            SsRequired(ErrorMessage = "手机号不能为空"),


            SsMaxValue(15000000000, ErrorMessage = "手机号格式不正确,不是11位"),
            SsMinValue(10000000000, ErrorMessage = "手机号格式不正确,不是11位"),         
            SsSize(min = 11, max = 11, ErrorMessage = "手机号格式不正确,不是11位")]
      */

        /*
         ssValidations:
       [
        {"path":"user.userType","ssError":{}, "ssValid":{"type":"Equal","value":"Logined"} }		 
       ]
         *
         */

        public static bool Valid(JObject obj, List<SsValidation> validations,   out SsError ssError)
        {
            ssError = null;

            if (null == validations) return true;
            foreach (var validation in validations)
            {
                if (!Valid(obj, validation))
                {
                    ssError = validation.ssError;
                    return false;
                }
            }           
            return true;
        }


 
        static bool Valid(JObject obj, SsValidation validation)
        {
           
            var ssValid = validation.ssValid;

            JToken value= obj.SelectToken(validation.path);


            EValidType validType= EValidType.Null;

            #region get  validType
            try
            {
                validType = ssValid["type"].Value<string>().StringToEnum<EValidType>();
            }
            catch
            {
            }
            #endregion

          
            switch (validType)
            {

                #region (x.1)SsEqual
                case EValidType.Equal:
                {
                    //{ "type":"Equal","value":"Logined"}
                    if (value.IsNull() ||
                        ssValid["value"].ConvertToString() == value.ConvertToString()
                    )
                        return true;
                    break;
                }
                #endregion

                #region (x.2)SsRegex
                case EValidType.Regex:
                    {
                        //{ "type":"Regex","value":"^\\d{11}$"}
                        if (value.IsNull()) return true;

                        Regex regex = new Regex(ssValid["value"].ConvertToString());

                        if (regex.IsMatch(value.ConvertToString()))
                            return true;
                        break;
                    }
                #endregion

                #region (x.3)SsRequired
                case EValidType.Required:
                    {
                        //{ "type":"Required" }
                        if (!value.IsNull()) return true;
                        break;
                    }
                #endregion

                case EValidType.Null:
                    return true;
            }           
            return false;
        }
         


    }
}
