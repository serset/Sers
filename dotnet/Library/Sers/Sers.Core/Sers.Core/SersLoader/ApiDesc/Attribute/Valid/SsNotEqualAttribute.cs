
namespace Sers.SersLoader.ApiDesc.Attribute.Valid
{
    /// <summary>
    /// demo:
    /// [SsNotEqual(path = "http.method", value = "PUT",errorMessage = "不可为PUT请求")]
    /// </summary>
    public class SsNotEqualAttribute : SsCmpAttribute
    {
        public SsNotEqualAttribute()
        {
            type = "!=";
        }
    }
}
