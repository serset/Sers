using Newtonsoft.Json.Linq;

namespace Vit.Extensions
{
    public static partial class JArrayExtensions
    {
        #region ToArray

        /// <summary>
        /// (注：若jArray不为 JArray类型 则 返回null)
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="extCount">数组额外长度</param>
        /// <param name="destinationIndex">开始赋值的位置偏移量，从0开始</param>
        /// <returns></returns>
        public static object[] ToArray(this JToken jArray, long extCount=0, long destinationIndex=0)
        {
            if (!jArray.TypeMatch(JTokenType.Array)) return null;

            JArray arr = jArray.Value<JArray>();
            object[] oa = new object[arr.Count + extCount];


            for (long i = arr.Count - 1; i >= 0; i--)
            {
                oa[i + destinationIndex] = arr[i].GetValue();
            }
            return oa;
        }

        #endregion


        #region ToArray<T>

        /// <summary>
        /// (注：若jArray不为 JArray类型 则 返回null)
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="extCount">数组额外长度</param>
        /// <param name="destinationIndex">开始赋值的位置偏移量，从0开始</param>
        /// <returns></returns>
        public static T[] ToArray<T>(JToken jArray, long extCount=0, long destinationIndex=0)
        {
            if (!jArray.TypeMatch(JTokenType.Array)) return null;

            JArray arr = jArray.Value<JArray>();
            T[] oa = new T[arr.Count + extCount];

            for (long i = arr.Count - 1; i >= 0; i--)
            {
                oa[i + destinationIndex] = arr[i].Value<T>();
            }
            return oa;
        }
        #endregion

    }
}
