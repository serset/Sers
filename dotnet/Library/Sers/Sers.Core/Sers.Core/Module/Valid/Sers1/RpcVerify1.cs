using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.Core.Module.Valid.Sers1
{
    public class RpcVerify1
    {
        //public static List<SsValidation> GetRpcValidationsFromMethod(MethodInfo info)
        //{
        //    var attrs = info.GetCustomAttributes<SsValidationBaseAttribute>();

        //    var validations = new List<SsValidation>();
        //    foreach (var attr in attrs)
        //    {
        //        attr.GetSsValidation(validations);
        //    }
        //    if (validations.Count == 0) return null;
        //    return validations;
        //}


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

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool Verify(JObject obj, List<SsValidation> validations, out SsError ssError)
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



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static bool Valid(JObject obj, SsValidation validation)
        {

            var ssValid = validation.ssValid;

            JToken value = obj.SelectToken(validation.path);


            EValidType validType = EValidType.Null;

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
                        if (value.IsNull()) return true;

                        if (ssValid["value"].ConvertToString() == value.ConvertToString())
                            return true;
                        return false;
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
                        return false;
                    }
                #endregion

                #region (x.3)SsRequired
                case EValidType.Required:
                    {
                        //{ "type":"Required" }
                        if (!value.IsNull()) return true;
                        return false;
                    }
                #endregion


                #region (x.4)NotEqual
                case EValidType.NotEqual:
                    {
                        //{ "type":"NotEqual","value":"Logined"}
                        if (value.IsNull()) return true;

                        if (ssValid["value"].ConvertToString() != value.ConvertToString())
                            return true;
                        return false;
                    }
                #endregion

                #region (x.5)Scope
                case EValidType.Scope:
                    {
                        //{ "type":"Scope","min":10.8,"max":12.5}  //包含最大值 最小值，可只指定最大值或最小值
                        if (value.IsNull()) return true;

                        try
                        {
                            if (!value.TryParseIgnore(out double dValue))
                            {
                                return false;
                            }

                            if (ssValid["min"].TryParseIgnore(out double min))
                            {
                                if (dValue < min)
                                    return false;
                            }

                            if (ssValid["max"].TryParseIgnore(out double max))
                            {
                                if (max < dValue)
                                    return false;
                            }
                            return true;
                        }
                        catch
                        {
                        }
                        return false;
                    }
                #endregion


                case EValidType.Null:
                    return true;
            }
            return false;
        }




        #region enum EValidType


        enum EValidType
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

            /// <summary>
            /// { "type":"NotEqual","value":"Logined"}
            /// </summary>
            NotEqual,
            /// <summary>
            /// { "type":"Scope","min":10.8,"max":12.5}  //包含最大值 最小值，可只指定最大值或最小值
            /// </summary>
            Scope,
            ///// <summary>
            ///// { "type":"MaxValue","value":11} //包含最大值
            ///// </summary>
            //MaxValue,
            ///// <summary>
            ///// { "type":"MinValue","value":11}  //包含最小值
            ///// </summary>
            //MinValue,
            ///// <summary>
            ///// { "type":"Size","min":11,"max":11}
            ///// </summary>
            //Size
        }
        #endregion

    }
}
