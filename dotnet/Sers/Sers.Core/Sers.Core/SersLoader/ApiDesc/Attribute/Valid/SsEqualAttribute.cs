namespace Sers.SersLoader.ApiDesc.Attribute.Valid
{
    /// <summary>
    /// demo:
    ///   [SsEqual(path = "http.method", value = "PUT")]
    /// </summary>
    public class SsEqualAttribute : SsCmpAttribute
    {
        public SsEqualAttribute()
        {
            type = "==";
        }
    }
}
