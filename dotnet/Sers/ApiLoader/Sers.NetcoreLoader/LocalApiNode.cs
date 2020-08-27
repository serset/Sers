using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.Dynamic;
using Vit.Extensions;
using Vit.Ioc;

namespace Sers.NetcoreLoader
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LocalApiNode: IApiNode
    {
        [JsonProperty]
        public SsApiDesc apiDesc { get; set; }


        DynamicMethodExecutor executor;

        MethodInfo apiController_Method;

        Func<ArraySegment<byte>, object[]> Arg_Deserialize;

        Func<object, byte[]> ReturnValue_SerializeToBytes = ObjectSerializeExtensions.SerializeToBytes;

        public LocalApiNode(SsApiDesc apiDesc,  MethodInfo apiController_Method)
        {
            this.apiDesc = apiDesc;
            this.apiController_Method = apiController_Method;
            executor = new DynamicMethodExecutor(apiController_Method);

            var OnDeserialize=Arg_Deserialize = apiDesc.argType.OnDeserialize;

            #region returnType is ActionResult 、Task
            {
                var returnType = apiController_Method.ReturnType;

                //(x.x.1)ActionResult
                if (returnType.IsGenericType && typeof(ActionResult<>).IsAssignableFrom(returnType.GetGenericTypeDefinition()))
                {
                    var propertyInfo = returnType.GetProperty("Value");
                    ReturnValue_SerializeToBytes = (obj) => {

                        return propertyInfo.GetValue(obj).SerializeToBytes();
                    };
                }

                //(x.x.2)Task
                if (returnType.IsGenericType && typeof(Task<>).IsAssignableFrom(returnType.GetGenericTypeDefinition()))
                {
                    var propertyInfo = returnType.GetProperty("Result");

                    //是否为 ActionResult
                    var taskResultType = returnType.GetGenericArguments()[0];
                   
                    if (taskResultType.IsGenericType && typeof(ActionResult<>).IsAssignableFrom(taskResultType.GetGenericTypeDefinition()))
                    {
                        var propertyInfo_ActionResult = taskResultType.GetProperty("Value");
                        ReturnValue_SerializeToBytes = (obj) =>
                        {
                            (obj as Task)?.Wait();

                            var taskResult = propertyInfo.GetValue(obj);
                            var actionResult = propertyInfo_ActionResult.GetValue(taskResult);
                            return actionResult?.SerializeToBytes();
                        };
                    }
                    else
                    {
                      
                        ReturnValue_SerializeToBytes = (obj) => {
                            (obj as Task)?.Wait();

                            var taskResult = propertyInfo.GetValue(obj);
                            return taskResult?.SerializeToBytes();
                        };
                    }
                }
            }
            #endregion


            #region 转换 特殊 route  "/a/b/{id}/{name}"  为 "/a/b/*"
            ArgFromUrl();

            void ArgFromUrl()
            {
                var oriRoute = apiDesc.OriRouteGet();
                int index = oriRoute.IndexOf("/{");
                if (index < 0) return; 

                #region url 
                var routeArgNames = oriRoute.Substring(index+1).Replace("{", "").Replace("}", "");
                var urlArgNames = routeArgNames.Split('/');
                if (urlArgNames == null || urlArgNames.Length == 0) return;
                #endregion


                //arg
                ParameterInfo[] infos = apiController_Method.GetParameters();
                if (infos == null || infos.Length == 0) return;


                #region build  arg map                
                //(int urlIndex, int argIndex, Type argType)
                var maps = new List<(int, int, Type)>();

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];
                    for (int j = 0; j < urlArgNames.Length; j++)
                    {
                        if (info.Name == urlArgNames[j])
                        {
                            maps.Add((j, i, info.ParameterType));
                            break;
                        }
                    }
                }
                if (maps.Count == 0) return;
                #endregion



                #region Arg_Deserialize
                Arg_Deserialize = (arg_OriData) =>
                {
                    var args = OnDeserialize(arg_OriData);

                    var argsFromUrl = GetArgsFromUrl();

                    foreach ((int urlIndex, int argIndex, Type argType) in maps)
                    {
                        if (argsFromUrl.Length > urlIndex)
                        {
                            args[argIndex] = argsFromUrl[urlIndex].Deserialize(argType);
                        }
                    }

                    return args;
                };
                #endregion
            }

            string[] GetArgsFromUrl()
            {
                return RpcContext.RpcData.http_url_RelativeUrl_Get()?.Split('/');
            }
            #endregion
        }

        


        public /*virtual*/ byte[] Invoke(ArraySegment<byte> arg_OriData)
        {
            //(x.1)反序列化 请求参数
            //var args = apiDesc.argType?.Deserialize(arg_OriData);
            var args = Arg_Deserialize(arg_OriData);

            //(x.2) Invoke
            //var returnValue = apiController_Method.Invoke(IocHelp.Create(apiController_Method.DeclaringType), args);           
            var returnValue = executor.Execute(IocHelp.Create(apiController_Method.DeclaringType), args);

            //(x.3) 序列化 返回数据
            return ReturnValue_SerializeToBytes(returnValue);
        }       


    }
}
