﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;

using Vit.Core.Util.Dynamic;
using Vit.Extensions.Json_Extensions;

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
            this.apiController_Method = apiController_Method;
            this.apiController_Obj = apiController_Obj;
            executor = new DynamicMethodExecutor(apiController_Method);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public /*virtual*/ byte[] Invoke(ArraySegment<byte> arg_OriData)
        {

            //(x.1)反序列化 请求参数
            var args = apiDesc.argType?.OnDeserialize?.Invoke(arg_OriData);

            //(x.2) Invoke
            //var returnValue = apiController_Method.Invoke(apiController_Obj, args);
            var returnValue = executor.Execute(apiController_Obj ?? Activator.CreateInstance(apiController_Method.DeclaringType), args);

            //(x.3) 序列化 返回数据
            return returnValue?.SerializeToBytes();
        }
                          

        #region apiController        

        //private Type apiController_Type;
        MethodInfo apiController_Method;
        Object apiController_Obj;
        #endregion


    }
}
