using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;

using Vit.Core.Util.Dynamic;
using Vit.Extensions.Object_Serialize_Extensions;

namespace Sers.SersLoader
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LocalApiNode : IApiNode
    {
        [JsonProperty]
        public SsApiDesc apiDesc { get; set; }

        protected DynamicMethodExecutor executor;

        protected MethodInfo apiController_Method;
        protected Object apiController_Obj;


        public static LocalApiNode CreateLocalApiNode(SsApiDesc apiDesc, MethodInfo apiController_Method, Object apiController_Obj)
        {
            if (apiController_Method.ReturnType == typeof(Task))
            {
                return new AsyncLocalApiNodeWithoutReturn(apiDesc, apiController_Method, apiController_Obj);
            }
            else if (apiController_Method.ReturnType.IsGenericType && apiController_Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return new AsyncLocalApiNode(apiDesc, apiController_Method, apiController_Obj);
            }

            return new LocalApiNode(apiDesc, apiController_Method, apiController_Obj);
        }

        protected LocalApiNode(SsApiDesc apiDesc, MethodInfo apiController_Method, Object apiController_Obj)
        {
            this.apiDesc = apiDesc;
            this.apiController_Method = apiController_Method;
            this.apiController_Obj = apiController_Obj;
            executor = new DynamicMethodExecutor(apiController_Method);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual byte[] Invoke(ArraySegment<byte> arg_OriData)
        {

            //(x.1)反序列化 请求参数
            var args = apiDesc.argType?.OnDeserialize?.Invoke(arg_OriData);

            //(x.2) Invoke
            //var returnValue = apiController_Method.Invoke(apiController_Obj, args);
            var returnValue = executor.Execute(apiController_Obj ?? Activator.CreateInstance(apiController_Method.DeclaringType), args);

            //(x.3) 序列化 返回数据
            return returnValue?.SerializeToBytes();
        }
    }

    class AsyncLocalApiNode : LocalApiNode
    {
        PropertyInfo taskResultProperty;
        public AsyncLocalApiNode(SsApiDesc apiDesc, MethodInfo apiController_Method, Object apiController_Obj) : base(apiDesc, apiController_Method, apiController_Obj)
        {
            taskResultProperty = apiController_Method.ReturnType.GetProperty("Result");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] Invoke(ArraySegment<byte> arg_OriData)
        {
            //(x.1)反序列化 请求参数
            var args = apiDesc.argType?.OnDeserialize?.Invoke(arg_OriData);

            //(x.2) Invoke
            //var returnValue = apiController_Method.Invoke(apiController_Obj, args);
            var returnValue = executor.Execute(apiController_Obj ?? Activator.CreateInstance(apiController_Method.DeclaringType), args);

            // returnValue is Task<>,so wait it, then get the unwrapped return value
            returnValue = taskResultProperty.GetValue(returnValue);

            //(x.3) 序列化 返回数据
            return returnValue?.SerializeToBytes();
        }
    }

    class AsyncLocalApiNodeWithoutReturn : LocalApiNode
    {
        public AsyncLocalApiNodeWithoutReturn(SsApiDesc apiDesc, MethodInfo apiController_Method, Object apiController_Obj) : base(apiDesc, apiController_Method, apiController_Obj)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] Invoke(ArraySegment<byte> arg_OriData)
        {

            //(x.1)反序列化 请求参数
            var args = apiDesc.argType?.OnDeserialize?.Invoke(arg_OriData);

            //(x.2) Invoke
            //var returnValue = apiController_Method.Invoke(apiController_Obj, args);
            var returnValue = executor.Execute(apiController_Obj ?? Activator.CreateInstance(apiController_Method.DeclaringType), args);

            // returnValue is Task,so wait it
            ((Task)returnValue).GetAwaiter().GetResult();
            returnValue = null;

            //(x.3) 序列化 返回数据
            return returnValue?.SerializeToBytes();
        }
    }
}
