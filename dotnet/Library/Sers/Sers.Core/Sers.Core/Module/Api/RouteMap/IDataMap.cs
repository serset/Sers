using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Api.RouteMap
{
    internal interface IDataMap<T>
    {
        int Count { get; }
        IEnumerable<T> GetAll();
        T Remove(string route);
        /// <summary>
        /// 根据路由字符串直接查找
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        T Get(string route);

        /// <summary>
        /// 通过路由规则查找
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        T Routing(string route);
        void Set(string route, T apiService);
    }
}
