using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using System;

namespace Sers.Core.Util.Newtonsoft
{
    public class JsonHelp
    {


        #region ForceTypeValue
        /// <summary>
        /// 强制为 值类型 或 字符串类型。
        /// 若value不为值类型（bool、double等）或字符串类型，则返回 其ToString()，否则返回原值。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ForceTypeValue(object value)
        {
            return (null == value || (value is string) || value.GetType().IsValueType) ? value : value.ToString();
        }
        #endregion


        #region SetValueByPath
        /// <summary>
        /// 通过路径设置值
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="Override">在路径对应的值存在时 是否覆盖值</param>
        /// <returns></returns>
        public static bool SetValueByPath(JObject json, string path, JToken value, bool Override)
        {
            
            //  a.b    a[2]
            var token = json.SelectToken(path, false);
            if (null != token && !Override) return false;


            // token存在，则替换 a.b    a[2]
            if (null != token)
            {
                token.Replace(value);
                return true;
            }

            var sp = new char[] { '.', '[' };
            //不包含双(单)引号
            if (0 > path.IndexOfAny(new char[] { '\'', '"' }))
            {
                var ar = path.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                string parentPath = String.Join(".", ar, 0, ar.Length - 1);
                //获取 parent
                token = json.SelectToken(parentPath, false);
                if (null == token)
                {
                    token = new JObject();
                    SetValueByPath(json, parentPath, token, false);
                }
                token[ar[ar.Length - 1]] = value;
                return true;
            }

            //为一层path,例如 "name"
            if (0 > path.IndexOfAny(sp))
            {
                json[path] = value;
                return true;
            }

            //todo 为多层path,例如 "name.a[1]"

            return false;
        }
        #endregion

        #region Extend 扩充
        /// <summary>
        /// （注：若是 JObject属性， 不会覆盖原有的值；若是JArray，则仅复制引用）
        ///  Extend("{a:1,b:[{a:1},{a:1}]}","b:[{b:1},null,{b:1}],c:1") 结果为 "{a:1,b:[{a:1,b:1},{a:1},{b:1}],c:1}"
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        public static void Extend(JObject j1, JObject j2)
        {
            JToken t0, t1;
            foreach (var jp in j2)
            {
                t0 = j1[jp.Key];
                t1 = jp.Value;
                if (t1.IsNull()) { continue; }
                if (t0.IsNull()) { j1[jp.Key] = t1; continue; }
                if (JTokenType.Object == t0.Type && JTokenType.Object == t1.Type)
                    Extend(t0.Value<JObject>(), t1.Value<JObject>());
                else if (JTokenType.Array == t0.Type && JTokenType.Array == t1.Type)
                    Extend(t0.Value<JArray>(), t1.Value<JArray>());
            }
        }
        /// <summary>
        /// 仅复制引用
        /// Extend("[{a:1},{a:1}]","[{b:1},null,{b:1}]") 结果为 "[{a:1,b:1},{a:1},{b:1}]"
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        public static void Extend(JArray a0, JArray a1)
        {
            JToken t0, t1;
            long key = 0;
            for (; key < a0.Count; key++)
            {
                if (key >= a1.Count) return;

                t0 = a0[key];
                t1 = a1[key];

                if (t1.IsNull()) { continue; }
                if (t0.IsNull()) { a0[key] = t1; continue; }
                if (JTokenType.Object == t0.Type && JTokenType.Object == t1.Type)
                    Extend(t0.Value<JObject>(), t1.Value<JObject>());
                else if (JTokenType.Array == t0.Type && JTokenType.Array == t1.Type)
                    Extend(t0.Value<JArray>(), t1.Value<JArray>());
            }

            for (; key < a1.Count; key++)
            {
                a0.Add(a1[key]);
            }
        }
        #endregion     



        

    }
}
