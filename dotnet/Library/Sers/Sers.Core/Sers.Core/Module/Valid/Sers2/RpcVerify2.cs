using Newtonsoft.Json.Linq;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;
using Vit.Core.Util.SsExp;


namespace Sers.Core.Module.Valid.Sers2
{
    public class RpcVerify2
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool Verify(JObject rpcData, JObject ssExp, out SsError ssError)
        {
            ssError = null;
            if (ssExp.IsNull()) return true;

            var varifyResult = SsExpCalculator.Calculate(rpcData, ssExp);
            if (varifyResult.IsJObject())
            {
                ssError = varifyResult.Deserialize<SsError>();
                return false;
            }
            return true;
        }
    }
}
