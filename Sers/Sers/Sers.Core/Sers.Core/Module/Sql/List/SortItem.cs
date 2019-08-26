using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace Sers.Core.Module.Sql.List
{
    public class SortItem
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [SsExample("name")]
        public string fieldName;

        /// <summary>
        /// 是否为 正向排序
        /// </summary>
        [SsExample("true")]
        public bool isAsc;
    }
}
