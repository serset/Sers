using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Query
{
    public class SortItem
    {
        /// <summary>
        /// 字段名(可多级，例如 "parent.name")
        /// </summary>
        [SsExample("id")]
        [SsDescription("字段名(可多级，例如 \"parent.name\")")]
        public string field;

        /// <summary>
        /// 是否为正向排序
        /// </summary>
        [SsExample("true")]
        [SsDescription("是否为正向排序")]
        public bool asc;
    }
}
