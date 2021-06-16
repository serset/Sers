#region << 版本注释 - v1 >>
/*
 * ========================================================================
 * 版本：v1
 * 时间：2021-03-19
 * 作者：Lith
 * 邮箱：serset@yeah.net
 * 
 * ========================================================================
*/
#endregion


using System;
using System.Collections.Generic;
using System.Text;
 

namespace Vit.Core.Util.Threading
{
    /// <summary>
    /// 切换线程时依然可以传递数据。(若切换线程不传递则可使用System.Threading.ThreadLocal ,或者使用[ThreadStatic]特性)
    /// 多包裹一层的原因是 子异步任务结束时会还原子异步任务对AsyncLocal做的更改(即子异步任务对AsyncLocal做的更改不会保留到子异步任务结束后的父异步任务中)
    /// 参见https://blog.csdn.net/kkfd1002/article/details/80102244
    /// </summary>
    public class AsyncCache<T>
    {

        readonly System.Threading.AsyncLocal<CachedData> _AsyncLocal = new System.Threading.AsyncLocal<CachedData>();

        class CachedData
        {
            public T Cache;
        }
        public T Value
        {
            get
            {
                if (null == _AsyncLocal.Value)
                    return default(T);
                return _AsyncLocal.Value.Cache;
            }
            set
            {
                var asyncLocal = _AsyncLocal.Value;
                if (null == asyncLocal) asyncLocal = _AsyncLocal.Value = new CachedData();
                asyncLocal.Cache = value;
            }
        }
    }
   
}
