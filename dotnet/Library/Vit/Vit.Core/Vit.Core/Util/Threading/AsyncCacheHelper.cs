using System;
using System.Collections.Concurrent;


namespace Vit.Core.Util.Threading
{
    public class AsyncCacheHelper
    {
        static  AsyncCache<ConcurrentDictionary<string,object>> _AsyncCache = new AsyncCache<ConcurrentDictionary<string, object>>();
        static ConcurrentDictionary<string, object> dic
        {
            get
            {
                return _AsyncCache.Value ?? (_AsyncCache.Value = new ConcurrentDictionary<string, object>());                
            }             
        }
        public static void Set(string key,Object value)
        {
            dic[key] = value;
        }

        public static Object Get(string key)
        {
            if (dic.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        public static T Get<T>(string key)
        {
            try
            {
                if (dic.TryGetValue(key, out var value))
                {
                    return (T)value;
                }
            }
            catch { }           
            return default(T);
        }

    }
}
