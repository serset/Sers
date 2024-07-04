using System;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Vit.Core.Module.Serialization;
using Vit.Extensions.Serialize_Extensions;

namespace Vit.Extensions.Newtonsoft_Extensions
{
    public static partial class JTokenExtensions
    {





        #region ValueSetByPath
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="path">value在data中的路径，可不指定。例如：new []{"taskList"}</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValueSetByPath(this JToken data, object value, params object[] path)
        {
            JToken jtValue = value.ToJToken();

            if (path == null || path.Length == 0)
            {
                data.Replace(jtValue);
                return;
            }

            JToken cur = data, next;

            int t = 0;
            object name;
            for (; t < path.Length - 1; t++)
            {
                name = path[t];

                next = cur[name];
                if (next.IsNull())
                {
                    cur = cur[name] = new JObject();
                }
                else
                {
                    cur = next;
                }
            }
            name = path[t];
            cur[name] = jtValue;
        }
        #endregion

        #region ValueGetByPath
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path">value在data中的路径，可不指定。例如：new []{"taskList"}</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JToken JTokenGetByPath(this JToken data, params object[] path)
        {
            JToken cur = data;

            foreach (var item in path)
            {
                cur = cur[item];
                if (cur.IsNull())
                {
                    return null;
                }
            }
            return cur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path">value path in data, could be null. (example: new []{"taskList"} ) </param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string StringGetByPath(this JToken data, params object[] path)
        {
            return data?.JTokenGetByPath(path)?.ConvertToString();
        }

        #endregion


        #region object ToJToken
        /// <summary>
        /// 若为JToken类型，则直接返回。若为Object,则返回序列化再反序列化后的JToken(为 JObject 或 JArray)。否则return new JValue(value)。
        /// value必须为值类型或者 JToken类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JToken ToJToken(this object value)
        {
            if (null == value) return JValue.CreateNull();
            if (value is JToken token) return token;
            if (value.IsValueTypeOrStringType())
                return new JValue(value);
            else
            {
                return JsonConvert.DeserializeObject(Json.Serialize(value)) as JToken;
            }
        }
        #endregion


        #region IsNull
        /// <summary>
        /// 是否为 null 或者 类型为JTokenType.Null
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this JToken v)
        {
            return null == v || v.Type == JTokenType.Null;
            //return null == v || v.Type == JTokenType.Null || v.Type == JTokenType.Undefined || v.Type == JTokenType.None;
        }
        #endregion

        #region IsJObject
        /// <summary>
        /// 是否为 JObject
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsJObject(this JToken v)
        {
            return null != v && v.Type == JTokenType.Object;
        }
        #endregion

        #region IsJArray
        /// <summary>
        /// 是否为 JArray
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsJArray(this JToken v)
        {
            return null != v && v.Type == JTokenType.Array;
        }
        #endregion


        #region GetOrCreateJObject


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JObject GetOrCreateJObject(this JObject v, string key)
        {
            //lock (v)
            //{
            var token = v[key];
            if (!(token is JObject jo))
            {
                v[key] = jo = new JObject();
            }
            return jo;

            //var token = v[key];
            //if (token.IsJObject())
            //    return token.Value<JObject>();

            //var cur = new JObject();
            //v[key] = cur;
            //return cur;
            //}
        }
        #endregion


        #region GetValue
        /// <summary>
        /// 返回具体的值。（若t为null则返回null）
        /// 例如 若为 double 则返回其double值
        /// （JArray、bool、byte[]、DateTime、double、long、JObject、String）
        /// 其他返回null（JTokenType.None、JTokenType.Null、JTokenType.Undefined、JTokenType.Comment 等）。
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetValue(this JToken t)
        {
            if (null == t) return null;
            switch (t.Type)
            {
                case JTokenType.Array: return t.Value<JArray>();
                case JTokenType.Boolean: return t.Value<bool>();
                case JTokenType.Bytes: return t.Value<byte[]>();
                case JTokenType.Date: return t.Value<DateTime>();
                case JTokenType.Float: return t.Value<double>();
                case JTokenType.Integer: return t.Value<long>();
                case JTokenType.Object: return t.Value<JObject>();
                case JTokenType.String: return t.Value<string>();
                //case JTokenType.Comment: return t.ToString();
                //case JTokenType.None: return null;
                //case JTokenType.Null: return null;
                //case JTokenType.Undefined: return null;
                default: return null;
            }
        }
        #endregion


        #region ConvertToString
        /// <summary>
        /// 转换为String
        /// <para>和JToken.ToString不同的是，会处理空值；若JToken为String类型，函数返回值不带双引号</para>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToString(this JToken token)
        {
            if (token.IsNull())
            {
                return null;
            }
            if (JTokenType.String == token.Type)
            {
                return token.Value<string>();
            }
            return token.ToString();
        }
        #endregion



        #region TypeMatch

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TypeMatch(this JToken token, JTokenType type)
        {
            return token?.Type == type;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TypeMatch(this JToken token, JToken token2)
        {
            return token != null && token?.Type == token2?.Type;
        }
        #endregion


        #region TryParse JObject
        /// <summary>
        ///  
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(this JToken token, out JObject value)
        {
            if (token.TypeMatch(JTokenType.Object))
            {
                value = token.Value<JObject>();
                return true;
            }

            value = null;
            return false;
        }
        #endregion

        #region TryParse JArray
        /// <summary>
        ///  
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(this JToken token, out JArray value)
        {
            if (token.TypeMatch(JTokenType.Array))
            {
                value = token.Value<JArray>();
                return true;
            }

            value = null;
            return false;
        }
        #endregion

        #region TryParseIgnore


        #region TryParseIgnore Boolean
        /// <summary>
        ///  当为 String（必须为true 、 false、True 、False） 时 转换。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseIgnore(this JToken token, out bool value)
        {
            if (null != token)
                switch (token.Type)
                {
                    case JTokenType.String:
                        string str = token.Value<string>();
                        if ("true" == str || "True" == str)
                        {
                            value = true;
                            return true;
                        }
                        if ("false" == str || "False" == str)
                        {
                            value = false;
                            return true;
                        }
                        break;

                    case JTokenType.Boolean:
                        value = token.Value<bool>();
                        return true;
                }
            value = default;
            return false;
        }
        #endregion


        #region TryParseIgnore Integer
        /// <summary>
        ///  当为 String（必须为数值） double long 类型时 转换。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseIgnore(this JToken token, out int value)
        {
            if (null == token) { value = 0; return false; }
            switch (token.Type)
            {
                case JTokenType.String:
                    return int.TryParse(token.Value<string>(), out value);
                case JTokenType.Integer:
                    value = token.Value<int>();
                    return true;
                case JTokenType.Float:
                    value = (int)token.Value<double>();
                    return true;
            }
            value = default;
            return false;
        }


        /// <summary>
        ///  当为 String（必须为数值） double long 类型时 转换。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseIgnore(this JToken token, out long value)
        {
            if (null == token) { value = 0; return false; }
            switch (token.Type)
            {
                case JTokenType.String:
                    return long.TryParse(token.Value<string>(), out value);
                case JTokenType.Integer:
                    value = token.Value<long>();
                    return true;
                case JTokenType.Float:
                    value = (long)token.Value<double>();
                    return true;
            }
            value = default;
            return false;
        }
        #endregion


        #region TryParseIgnore Float
        /// <summary>
        ///  当为 String（必须为数值） double long 类型时 转换。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseIgnore(this JToken token, out double value)
        {
            if (null == token) { value = 0; return false; }
            switch (token.Type)
            {
                case JTokenType.String:
                    return double.TryParse(token.Value<string>(), out value);
                case JTokenType.Integer:
                    value = token.Value<long>();
                    return true;
                case JTokenType.Float:
                    value = token.Value<double>();
                    return true;
            }
            value = default;
            return false;
        }
        #endregion


        #region TryParseIgnore DateTime
        /// <summary>
        /// 当为 String（必须为DateTime）时 亦转换。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseIgnore(this JToken token, out DateTime value)
        {
            if (token.TypeMatch(JTokenType.Date))
            {
                value = token.Value<DateTime>();
                return true;
            }
            if (token.TypeMatch(JTokenType.String))
            {
                if (DateTime.TryParse(token.Value<string>(), out value))
                    return true;
            }
            value = default;
            return false;
        }
        #endregion


        #endregion



        #region Equal (Boolean String Integer Float DateTime)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(this JToken token, bool value)
        {
            return token.TypeMatch(JTokenType.Boolean) && value == token.Value<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(this JToken token, string value)
        {
            return token.TypeMatch(JTokenType.String) && value == token.Value<string>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(this JToken token, long value)
        {
            return token.TypeMatch(JTokenType.Integer) && value == token.Value<long>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(this JToken token, double value)
        {
            return token.TypeMatch(JTokenType.Float) && value == token.Value<double>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(this JToken token, DateTime value)
        {
            return token.TypeMatch(JTokenType.Date) && value == token.Value<DateTime>();
        }
        #endregion


        #region EqualIgnore (Boolean string long double DateTime)

        /// <summary>
        /// 若不为Boolean型，则先转换（若可以转换），再比较
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualIgnore(this JToken token, bool value)
        {
            if (!token.TryParseIgnore(out bool dest)) return false;
            return dest == value;
        }


        /// <summary>
        /// 若不为long型，则先转换（若可以转换），再比较
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualIgnore(this JToken token, long value)
        {
            if (!token.TryParseIgnore(out long dest)) return false;
            return dest == value;
        }


        /// <summary>
        /// 若不为字符串，则先转换为字符串再进行比较
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualIgnore(this JToken token, string value)
        {
            return value == token.ConvertToString();
        }


        #endregion

    }
}
