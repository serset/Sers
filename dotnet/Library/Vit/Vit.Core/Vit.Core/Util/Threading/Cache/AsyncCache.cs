#region << Comment - v3 >>
/*
 * ========================================================================
 * Version: v3
 * Time   : 2023-11-15
 * Author : Lith
 * Email  : LithWang@outlook.com
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
    /// Allows passing data across thread switches. (If data passing is not required during thread switching, 
    ///   System.Threading.ThreadLocal can be used, or [ThreadStatic] attribute can be applied.)
    /// The reason for adding an additional layer is that child asynchronous tasks will restore changes made to AsyncLocal when they finish 
    ///  (i.e., changes made to AsyncLocal by child asynchronous tasks will not persist in the parent asynchronous tasks after the child asynchronous tasks finish).
    /// See https://blog.csdn.net/kkfd1002/article/details/80102244 for reference.
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
