using Sers.Core.Module.Api.ApiDesc;


namespace Vit.Extensions
{
    public static partial class SsApiDescExtensions
    {

        #region ApiStationName     
        public static string ApiStationNameGet(this SsApiDesc data)
        {
            try
            {
                return data?.route?.Split('/')[1];
            }
            catch
            {
            }
            return null;
        }
        #endregion


        #region ServiceKey
        /// <summary>
        /// 为 route + "_"+  httpMethod
        /// httpMethod可能为空
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ServiceKeyGet(this SsApiDesc data)
        {
            try
            {
                return data.route+"_" + data.HttpMethodGet() ;
            }
            catch
            {
            }
            return null;
        }
        #endregion



        #region HttpMethod

        public static string HttpMethodGet(this SsApiDesc data)
        {
            return data?.extendConfig?.Value<string>("httpMethod");
        }


        public static void HttpMethodSet(this SsApiDesc data,string httpMethod)
        {
            if (data.extendConfig == null)
                data.extendConfig = new Newtonsoft.Json.Linq.JObject();

            data.extendConfig["httpMethod"] = httpMethod;
        }

        #endregion


        #region OriRoute

        public static string OriRouteGet(this SsApiDesc data)
        {
            return data?.extendConfig?.Value<string>("oriRoute");
        }


        public static void OriRouteSet(this SsApiDesc data, string oriRoute)
        {
            if (data.extendConfig == null)
                data.extendConfig = new Newtonsoft.Json.Linq.JObject();

            data.extendConfig["oriRoute"] = oriRoute;
        }
        #endregion



        #region sysDesc

        public static string SysDescGet(this SsApiDesc data)
        {
            return data?.extendConfig?.Value<string>("sysDesc");
        }

        public static void SysDescSet(this SsApiDesc data, string value)
        {
            if (data.extendConfig == null)
                data.extendConfig = new Newtonsoft.Json.Linq.JObject();

            data.extendConfig["sysDesc"] = value;
        }

        public static string SysDescAppend(this SsApiDesc data, string value)
        {
            value = SysDescGet(data) + value;
            SysDescSet(data,value);
            return value;
        }

        #endregion


    }
}
