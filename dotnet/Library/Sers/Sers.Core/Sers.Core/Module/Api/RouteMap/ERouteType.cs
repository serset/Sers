namespace Sers.Core.Module.Api.RouteMap
{
    public enum ERouteType : byte
    {
        /// <summary>
        /// 普通接口。例如： "/station1/fold2/api1"
        /// </summary>
        nomalRoute,
        /// <summary>
        /// 泛接口。例如： "/station1/fold2/*"
        /// </summary>
        genericRoute

    }
}
