using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json.Linq;

using Vit.Extensions;

namespace Vit.Core.MsTest.Util.ComponentModel
{

    [TestClass]
    public class ExceptionTest
    {

        [TestMethod]
        public void ErrorSet_Test()
        {
            int ErrorCode = 452;
            string ErrorMessage = "error ";
            string ErrorDetail_Opt = "error ErrorDetail_Opt";
            try
            {
                throw new System.Exception().ErrorCode_Set(ErrorCode).ErrorMessage_Set(ErrorMessage).ErrorDetail_Set(new { ErrorDetail_Opt });
            }
            catch (System.Exception ex)
            {
                Assert.AreEqual(ErrorCode, ex.ErrorCode_Get());
                Assert.AreEqual(ErrorMessage, ex.ErrorMessage_Get());
                Assert.AreEqual(ErrorDetail_Opt, ex.ErrorDetail_Get<JObject>()[nameof(ErrorDetail_Opt)]);
            }

        }

    }
}
