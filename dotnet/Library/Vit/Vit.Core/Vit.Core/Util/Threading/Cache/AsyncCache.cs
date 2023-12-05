#region << Comment - v3 >>
/*
 * ========================================================================
 * 版本：v3
 * 时间：2023-11-15
 * 作者：Lith
 * 邮箱：serset@yeah.net
 * 
 * ========================================================================
*/
#endregion


using System;

namespace Vit.Core.Util.Threading.Cache
{
    public abstract class AsyncCacheScope<ScopeType> : IDisposable
        where ScopeType : AsyncCacheScope<ScopeType>
    {
        public AsyncCacheScope()
        {
            Instance_AsyncCache.CreateScope(this as ScopeType);
        }
        public virtual void Dispose()
        {
            Instance_AsyncCache.DisposeScope();
        }

        static AsyncCache<ScopeType> Instance_AsyncCache = new AsyncCache<ScopeType>();

        public static ScopeType Instance => Instance_AsyncCache.Value;
    }


    /// <summary>
    /// 切换线程时依然可以传递数据。(若切换线程不传递则可使用System.Threading.ThreadLocal ,或者使用[ThreadStatic]特性)
    /// 多包裹一层的原因是 子异步任务结束时会还原子异步任务对AsyncLocal做的更改(即子异步任务对AsyncLocal做的更改不会保留到子异步任务结束后的父异步任务中)
    /// 参见 https://blog.csdn.net/kkfd1002/article/details/80102244
    /// </summary>
    public class AsyncCache<DataType>
    {

        readonly System.Threading.AsyncLocal<DataWrap> _AsyncLocal = new System.Threading.AsyncLocal<DataWrap>();

        class DataWrap : IDisposable
        {
            internal Action OnDispose;
            internal DataType data;

            public void Dispose()
            {
                OnDispose?.Invoke();
            }
        }
        public void CreateScope()
        {
            _AsyncLocal.Value = new DataWrap();
        }
        public void CreateScope(DataType value)
        {
            _AsyncLocal.Value = new DataWrap { data = value };
        }

        public IDisposable NewScope()
        {
            var wrap = new DataWrap { OnDispose = DisposeScope };
            _AsyncLocal.Value = wrap;
            return wrap;
        }
        public IDisposable NewScope(DataType value)
        {
            var wrap = new DataWrap { OnDispose = DisposeScope, data = value };
            _AsyncLocal.Value = wrap;
            return wrap;
        }


        public void DisposeScope()
        {
            _AsyncLocal.Value = null;
        }
        public bool SetValue(DataType value)
        {
            var dataWrap = _AsyncLocal.Value;
            if (dataWrap == null) return false;
            dataWrap.data = value;
            return true;
        }
        public DataType Value
        {
            get
            {
                var dataWrap = _AsyncLocal.Value;
                if (dataWrap == null) return default;
                return dataWrap.data;
            }
            set
            {
                SetValue(value);
            }
        }
    }

}
