
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Core.Util.ComponentModel.SsError.Extensions;
using Vit.Extensions;

namespace Vit.Core.MsTest.Util.Error
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

                throw new System.Exception().ErrorCode_Set(ErrorCode).ErrorMessage_Set(ErrorMessage).ErrorDetail_Set(nameof(ErrorDetail_Opt), ErrorDetail_Opt);
            }
            catch (System.Exception ex)
            {
                Assert.AreEqual(ErrorCode, ex.ErrorCode_Get());
                Assert.AreEqual(ErrorMessage, ex.ErrorMessage_Get());
                Assert.AreEqual(ErrorDetail_Opt, ex.ErrorDetail_Get(nameof(ErrorDetail_Opt)));

            }

        }



        [TestMethod]
        public void SersException_Test()
        {

            string msg = "";
            SsException.Event_OnCreateException += (ex) =>
            {
                msg += "Event_OnCreateException called";
            };

            try
            {

                throw new SsException();
            }
            catch (System.Exception ex)
            {
            }

            Assert.AreEqual(msg, "Event_OnCreateException called");

            SsException.Event_OnCreateException = null;

        }

    }
}
