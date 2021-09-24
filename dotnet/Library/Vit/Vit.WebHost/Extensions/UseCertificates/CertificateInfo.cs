using Newtonsoft.Json;


namespace Vit.WebHost.Extensions.UseCertificates
{
    #region CertificateInfo
    [JsonObject(MemberSerialization.OptIn)]
    public class CertificateInfo
    {
      
        [JsonProperty]
        public string filePath { get; set; }

        [JsonProperty]
        public string password { get; set; }

    }
    #endregion
}
