using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using System.Text.RegularExpressions;

namespace Sers.Core.Util.SsExp
{
    /// <summary>
    /// SsExpression
    /// </summary>
    public class SsExpCalculator
    {
        /*
         * 
            //path 可不指定

            {"type":"SsExp",  "path":"a.b"  ,"value":SsExp  }

            {"type":"Cur",  "path":"a.b"  }  //返回当前path索引的值
            
            {"type":"Value", "value":Value  } //返回Value

            {"type":"_", ...  }  //直接返回当前表达式 

            {"type":"If", "path":"a.b"  ,"condition":SsExp,  "valueWhenTrue":SsExp,  "valueWhenFalse":SsExp }
            
            {"type":"Switch", "path":"a.b"  ,   "body":[  {"condition":SsExp,"value":SsExp } ,...   ] , "default":SsExp   }

            {"type":"And",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

            {"type":"Or",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

            {"type":"Not",    "path":"a.b",    "value":SsExp    }

            {"type":"NotNull",    "path":"a.b",    "value":SsExp    }

            { "type":"Regex",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }  //正则匹配

            {"type":"==",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

            {"type":"!=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

            {"type":">",    "path":"a.b",   "value":SsExp,  "resultWhenNull":false    }

            {"type":">=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

            {"type":"<",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false   }

            {"type":"<=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

            {"type":"in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组

            {"type":"not in",    "path":"a.b",    "value":SsExp }   //value 值必须为数组


            */


        public static JToken Calculate(JToken baseValue, JToken ssExp)
        {       
            if (!ssExp.TypeMatch(JTokenType.Object))
            {
                //if (ssExp.IsNull()) return null;
                //if (ssExp.IsNull()) return baseValue;
                return ssExp;
            }
           
            var joSsValue = ssExp.Value<JObject>();
            var curValue = GetCurValue(baseValue, joSsValue);
            switch (ssExp["type"].Value<string>())
            {
                #region value
                case "SsExp":
                    // {"type":"SsExp",  "path":"a.b"  ,"value":SsExp  }
                    return Calculate(curValue, ssExp["value"]);
                case "Cur":
                    //{"type":"Cur",  "path":"a.b"  }  //返回当前path索引的值         
                    return curValue;

                case "Value":
                    //{"type":"Value", "value":Value  }     //Value为值            
                    return ssExp["value"];
                
                case "_":
                    //{ "type":"_", ...  }  //直接返回当前表达式             
                    return ssExp;

                case "If":
                    //{"type":"If", "path":"a.b"  ,"condition":SsExp,  "valueWhenTrue":SsExp,  "valueWhenFalse":SsExp }
                    #region (x.2) 
                    {
                        if (JTokenToBool(Calculate(curValue, ssExp["condition"])))
                        {
                            return Calculate(curValue, ssExp["valueWhenTrue"]);
                        }
                        else
                        {
                            return Calculate(curValue, ssExp["valueWhenFalse"]);
                        }
                    }
                #endregion
                case "Switch":
                    // {"type":"Switch", "path":"a.b"  ,   "body":[  {"condition":SsExp,"value":SsExp } ,...   ] , "default":SsExp   }
                    #region (x.3) 
                    {
                        foreach (JObject item in ssExp["body"].Value<JArray>())
                        {
                            if (JTokenToBool(Calculate(curValue, item["condition"])))
                                return Calculate(curValue, item["value"]);
                        }
                        return  Calculate(curValue, ssExp["default"]);
                    }
                    #endregion

                #endregion

                #region bool
                case "And":
                    //{"type":"And",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }
                    #region (x.1)
                    {
                        var ssExps_children = ssExp["value"].Value<JArray>();
                        foreach (JToken ssExp_child in ssExps_children)
                        {
                            if (!JTokenToBool(Calculate(curValue, ssExp_child)))
                                return false;
                        }
                        return true;
                    }
                #endregion
                case "Or":
                    //{"type":"Or",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }
                    #region (x.2)
                    {
                        var ssExps_children = ssExp["value"].Value<JArray>();
                        foreach (JToken ssExp_child in ssExps_children)
                        {
                            var value = Calculate(curValue, ssExp_child);
                            if (JTokenToBool(Calculate(curValue, ssExp_child)))
                                //return value;
                                return true;
                        }
                        return false;
                    }
                #endregion

                case "Not":
                    //{"type":"Not",    "path":"a.b",    "value":SsExp    }
                    #region (x.3)
                    {
                        JToken descValue;
                        var ssExp_child = ssExp["value"];
                        if (ssExp_child == null)
                        {
                            descValue = curValue;
                        }
                        else
                        {
                            descValue = Calculate(curValue, ssExp_child);
                        }                  
                        return !JTokenToBool(descValue);
                    }
                #endregion

                case "NotNull":
                    // {"type":"NotNull",    "path":"a.b",    "value":SsExp    }
                    #region (x.4)
                    {
                        JToken descValue;
                        var ssExp_child = ssExp["value"];
                        if (ssExp_child == null)
                        {
                            descValue = curValue;
                        }
                        else
                        {
                            descValue = Calculate(curValue, ssExp_child);
                        }               
                        return !descValue.IsNull();
                    }
                #endregion
                case "Regex":
                    //{ "type":"Regex",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }  //正则匹配
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]).ConvertToString();
                        var curValue_Str = curValue.ConvertToString();

                        if (null == ssExp_Value || null == curValue_Str)
                        {
                            if (ssExp["resultWhenNull"].TryParseIgnore(out bool result)) return result;
                        }                      
                        return new Regex(ssExp_Value).IsMatch(curValue_Str);                 
                    }
                #endregion
                case "==":
                    //{"type":"==",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }
                    #region (x.5)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]).ConvertToString();
                        var curValue_Str = curValue.ConvertToString();                       

                        if (null== ssExp_Value || null == curValue_Str)
                        {                       
                            if( ssExp["resultWhenNull"].TryParseIgnore(out bool result)) return result;
                        }
                        return curValue_Str == ssExp_Value;
                    }
                #endregion

                case "!=":
                    //{"type":"!=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]).ConvertToString(); 
                        var curValue_Str = curValue.ConvertToString();

                        if (null == ssExp_Value || null == curValue_Str)
                        {
                            if (ssExp["resultWhenNull"].TryParseIgnore(out bool result)) return result;
                        }
                        return curValue_Str != ssExp_Value;
                    }
                #endregion
                case ">":
                    //{"type":">",    "path":"a.b",   "value":SsExp,  "resultWhenNull":false   }
                    #region (x.6)
                    {
                        if (!Calculate(curValue, ssExp["value"]).TryParseIgnore(out double ssExp_Value)
                            || !curValue.TryParseIgnore(out double curValue_Double)
                            )
                        {
                            return ssExp["resultWhenNull"].TryParseIgnore(out bool result) ? result : false;
                        }
                        return curValue_Double > ssExp_Value;
                    }
                #endregion
                case ">=":
                    //{"type":">=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }
                    #region (x.6)
                    {
                        if (!Calculate(curValue, ssExp["value"]).TryParseIgnore(out double ssExp_Value)
                               || !curValue.TryParseIgnore(out double curValue_Double)
                               )
                        {
                            return ssExp["resultWhenNull"].TryParseIgnore(out bool result) ? result : false;
                        }
                        return curValue_Double >= ssExp_Value;
                    }
                #endregion
                case "<":
                    //{"type":"<",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }
                    #region (x.7)
                    {
                        if (!Calculate(curValue, ssExp["value"]).TryParseIgnore(out double ssExp_Value)
                               || !curValue.TryParseIgnore(out double curValue_Double)
                               )
                        {
                            return ssExp["resultWhenNull"].TryParseIgnore(out bool result) ? result : false;
                        }
                        return curValue_Double < ssExp_Value;
                    }
                #endregion
                case "<=":
                    //{"type":"<=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }
                    #region (x.8)
                    {
                        if (!Calculate(curValue, ssExp["value"]).TryParseIgnore(out double ssExp_Value)
                               || !curValue.TryParseIgnore(out double curValue_Double)
                               )
                        {
                            return ssExp["resultWhenNull"].TryParseIgnore(out bool result) ? result : false;
                        }
                        return curValue_Double <= ssExp_Value;
                    }
                #endregion

                case "in":
                    // {"type":"in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["ssExp"]);
                        if (!ssExp_Value.IsJArray()) return false;

                        var ssExp_Value_Array = ssExp_Value as JArray;

                        var curValue_Str = curValue.ConvertToString();


                        foreach (var item in ssExp_Value_Array)
                        {
                            if ( curValue_Str == item.ConvertToString()) return true;
                        }
                        return false;
                    }
                #endregion

                case "not in":
                    // {"type":"not in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]);
                        if (!ssExp_Value.IsJArray()) return true;

                        var ssExp_Value_Array = ssExp_Value as JArray;

                        var curValue_Str = curValue.ConvertToString();

                        foreach (var item in ssExp_Value_Array)
                        {
                            if (curValue_Str == item.ConvertToString()) return false;
                        }
                        return true;
                    }
                    #endregion

                    #endregion
            }

            return null;
        }

        static bool  JTokenToBool(JToken value)
        {
            if (value.TryParseIgnore(out bool v)) return v;

            if (value.IsNull()) return false;
            return true;
        }

        #region GetCurValue
        static JToken GetCurValue(JToken baseValue, JObject SsExp)
        {
            var path = SsExp["path"].ConvertToString();
            if (string.IsNullOrEmpty(path)) return baseValue;
            return baseValue.SelectToken(path);
        }
        #endregion

    }
}
