package Sers.Core.Util.Threading;

/**
 * 多包裹一层的原因是 子异步任务结束时会还原子异步任务对AsyncLocal做的更改(即子异步任务对AsyncLocal做的更改不会保留到子异步任务结束后的父异步任务中)
 * @author help
 *
 * @param <T>
 */
public class AsyncCache<T> {

	 class CachedData
     {
         public T Cache;
     }
	 
	 
	 
	 ThreadLocal<CachedData> _AsyncLocal
     = new ThreadLocal<>();

	 
	 public T get() {
		 
		 CachedData cache=_AsyncLocal.get();
		 
		 if(null==cache) return null;
		 return cache.Cache;	 
	 } 
	 
	public void set(T value) {

		CachedData cache = _AsyncLocal.get();

		if (null == cache) {
			cache = new CachedData();
			_AsyncLocal.set(cache);
		}
		cache.Cache = value;
	}

}
