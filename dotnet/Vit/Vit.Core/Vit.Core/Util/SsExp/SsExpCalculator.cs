using Newtonsoft.Json.Linq;
using Vit.Extensions;
using System.Text.RegularExpressions;

namespace Vit.Core.Util.SsExp
{
    /// <summary>
    /// json ExpressionCalculator
    /// json表达式计算器
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

            {"type":"Switch", "path":"a.b"  ,   "body":[  {"condition":SsExp,"value":SsExp } ,...   ] , "default":SsExp   }


            以下为bool表达式,默认返回值为bool类型，也可手动指定满足条件时的返回值
            均存在如下属性：
            resultWhenNull   存在null值时的返回值，默认false
            resultWhenTrue   结果为true时的返回值，默认为true
            resultWhenFalse 结果为false时的返回值，默认为false

           
            {"type":"If", "path":"a.b"  ,"condition":SsExp }                 

            {"type":"And",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

            {"type":"Or",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

            {"type":"Not",    "path":"a.b",    "value":SsExp    }

            {"type":"NotNull",    "path":"a.b",    "value":SsExp    }

            { "type":"Regex",    "path":"a.b",    "value":SsExp  }  //正则匹配

            {"type":"==",    "path":"a.b",    "value":SsExp  }

            {"type":"!=",    "path":"a.b",    "value":SsExp  }

            {"type":">",    "path":"a.b",   "value":SsExp   }
                                                             
            {"type":">=",    "path":"a.b",    "value":SsExp    }
                                                             
            {"type":"<",    "path":"a.b",    "value":SsExp   }
                                                             
            {"type":"<=",    "path":"a.b",    "value":SsExp    }

            {"type":"in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组,否则返回 false

            {"type":"not in",    "path":"a.b",    "value":SsExp }   //value 值必须为数组,否则返回 true

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

                case "If":
                    //{"type":"If", "path":"a.b"  ,"condition":SsExp  }
                    #region (x.2) 
                    if (JTokenToBool(Calculate(curValue, ssExp["condition"])))
                    {
                        return GetResult(true);
                    }
                    else
                    {
                        return GetResult(false);
                    }              
                #endregion
                case "And":
                    //{"type":"And",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }
                    #region (x.1)
                    {
                        var ssExps_children = ssExp["value"].Value<JArray>();
                        foreach (JToken ssExp_child in ssExps_children)
                        {
                            if (!JTokenToBool(Calculate(curValue, ssExp_child)))
                                return GetResult(false);
                        }
                        return GetResult(true);
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
                                return GetResult(true);
                        }
                        return GetResult(false);
                    }
                #endregion

                case "Not":
                    //{"type":"Not",    "path":"a.b",    "value":SsExp    }
                    #region (x.3)
                    {
                        JToken destValue;
                        var ssExp_child = ssExp["value"];
                        if (ssExp_child == null)
                        {
                            destValue = curValue;
                        }
                        else
                        {
                            destValue = Calculate(curValue, ssExp_child);
                        }                  
                        bool calculatedValue = !JTokenToBool(destValue);
                        return GetResult(calculatedValue );
                    }
                #endregion

                case "NotNull":
                    // {"type":"NotNull",    "path":"a.b",    "value":SsExp    }
                    #region (x.4)
                    {
                        JToken destValue;
                        var ssExp_child = ssExp["value"];
                        if (ssExp_child == null)
                        {
                            destValue = curValue;
                        }
                        else
                        {
                            destValue = Calculate(curValue, ssExp_child);
                        }          
                      
                        bool calculatedValue = !destValue.IsNull();
                        return GetResult(calculatedValue);
                    }
                #endregion
                case "Regex":
                    //{ "type":"Regex",    "path":"a.b",    "value":SsExp   }  //正则匹配
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]).ConvertToString();
                        var curValue_Str = curValue.ConvertToString();

                        bool ? calculatedValue;
                        if (null == ssExp_Value || null == curValue_Str)
                        {
                            calculatedValue = null;
                        }
                        else
                        {
                            calculatedValue= new Regex(ssExp_Value).IsMatch(curValue_Str);
                        }
                        return GetResult(calculatedValue);
                    }
                #endregion
                case "==":
                    //{"type":"==",    "path":"a.b",    "value":SsExp   }
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]).ConvertToString();
                        var curValue_Str = curValue.ConvertToString();
              
                        if (null == ssExp_Value || null == curValue_Str)
                        {
                            return GetResult(null);
                        }                                
                        return GetResult(curValue_Str == ssExp_Value);
                    }
                #endregion

                case "!=":
                    //{"type":"!=",    "path":"a.b",    "value":SsExp   }
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]).ConvertToString(); 
                        var curValue_Str = curValue.ConvertToString();                       
                     
                        if (null == ssExp_Value || null == curValue_Str)
                        {
                            return GetResult(null);
                        }
                       
                        return GetResult(curValue_Str != ssExp_Value);
                    }
                #endregion
                case ">":
                    //{"type":">",    "path":"a.b",   "value":SsExp,  "resultWhenNull":false   }
                    #region (x.x)
                    {
                        if (!Calculate(curValue, ssExp["value"]).TryParseIgnore(out double ssExp_Value)
                            || !curValue.TryParseIgnore(out double curValue_Double)
                            )
                        {
                            return GetResult(null);
                        }
                        return GetResult(curValue_Double > ssExp_Value);                     
                    }
                #endregion
                case ">=":
                    //{"type":">=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }
                    #region (x.x)
                    {
                        if (!Calculate(curValue, ssExp["value"]).TryParseIgnore(out double ssExp_Value)
                               || !curValue.TryParseIgnore(out double curValue_Double)
                               )
                        {
                            return GetResult(null);
                        }
                        return GetResult(curValue_Double >= ssExp_Value);
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
                            return GetResult(null);
                        }
                        return GetResult(curValue_Double < ssExp_Value);
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
                            return GetResult(null);
                        }
                        return GetResult(curValue_Double <= ssExp_Value);
                    }
                #endregion

                case "in":
                    // {"type":"in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["ssExp"]);
                        if (!ssExp_Value.IsJArray())
                        {
                            return GetResult(false);
                        }


                        var ssExp_Value_Array = ssExp_Value as JArray;

                        var curValue_Str = curValue.ConvertToString();


                        foreach (var item in ssExp_Value_Array)
                        {
                            if ( curValue_Str == item.ConvertToString()) return GetResult(true);
                        }
                        return GetResult(false); 
                    }
                #endregion

                case "not in":
                    // {"type":"not in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组
                    #region (x.x)
                    {
                        var ssExp_Value = Calculate(curValue, ssExp["value"]);
                        if (!ssExp_Value.IsJArray())
                        {
                            return GetResult(true);
                        }

                        var ssExp_Value_Array = ssExp_Value as JArray;

                        var curValue_Str = curValue.ConvertToString();

                        foreach (var item in ssExp_Value_Array)
                        {
                            if (curValue_Str == item.ConvertToString()) return GetResult(false);
                        }
                        return GetResult(true);
                    }
                    #endregion

                    #endregion
            }

            return null;

            #region GetResult
            JToken GetResult(bool? calculatedResult)
            {
                if (calculatedResult == null)
                {
                    var result = ssExp["resultWhenNull"];
                    if (result.IsNull()) return false;
                    return Calculate(curValue, result);
                }

                if (calculatedResult.Value)
                {
                    var result = ssExp["resultWhenTrue"];
                    if (result.IsNull()) return true;
                    return Calculate(curValue, result);
                }else
                {
                    var result = ssExp["resultWhenFalse"];
                    if (result.IsNull()) return false;
                    return Calculate(curValue, result);
                }       
            }
            #endregion
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
