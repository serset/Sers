using System;
using System.Reflection;
using Newtonsoft.Json;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Vit.Core.Util.Dynamic;
using Vit.Extensions;

namespace Sers.SersLoader
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LocalApiNode: IApiNode
    {
        [JsonProperty]
        public SsApiDesc apiDesc { get; set; }


        DynamicMethodExecutor executor;
        public LocalApiNode(SsApiDesc apiDesc,  MethodInfo apiController_Method,Object apiController_Obj)
        {
            this.apiDesc = apiDesc;
            //this.apiController_Method = apiController_Method;
            this.apiController_Obj = apiController_Obj;
            executor = new DynamicMethodExecutor(apiController_Method);
        }

        public /*virtual*/ byte[] Invoke(ArraySegment<byte> arg_OriData)
        {

            //(x.1)反序列化 请求参数
            var args = apiDesc.argType?.OnDeserialize?.Invoke(arg_OriData);

            //(x.2) Invoke
            //var returnValue = apiController_Method.Invoke(apiController_Obj, args);
            var returnValue = executor.Execute(apiController_Obj, args);

            //(x.3) 序列化 返回数据
            return returnValue?.SerializeToBytes();
        }
                          

        #region apiController        

        //private Type apiController_Type;
        //MethodInfo apiController_Method;
        Object apiController_Obj;
        #endregion


    }
}
