using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Query
{
    public class SortItem
    {
        /// <summary>
        /// field name(can be cascaded). demo "parent.id"
        /// </summary>
        [SsExample("id")]
        [SsDescription("field name(can be cascaded). demo \"parent.id\"")]
        public string field;

        /// <summary>
        /// whether is order by ascendin
        /// </summary>
        [SsExample("true")]
        [SsDescription("whether is order by ascending")]
        public bool asc;
    }
}
