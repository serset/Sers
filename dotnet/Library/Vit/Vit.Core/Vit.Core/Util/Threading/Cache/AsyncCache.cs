#region << 版本注释 - v2 >>
/*
 * ========================================================================
 * 版本：v2
 * 时间：2023-10-23
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

        class DataWrap
        {
            internal DataType data;
        }
        public void CreateScope( )
        {
            _AsyncLocal.Value = new DataWrap();
        }
        public void CreateScope(DataType value)
        {
            _AsyncLocal.Value = new DataWrap { data = value };
        }

        public IDisposable NewScope()
        {
            _AsyncLocal.Value = new DataWrap();
            return new Disposable(DisposeScope);
        }
        public IDisposable NewScope(DataType value)
        {
            _AsyncLocal.Value = new DataWrap { data = value };
            return new Disposable(DisposeScope);
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
