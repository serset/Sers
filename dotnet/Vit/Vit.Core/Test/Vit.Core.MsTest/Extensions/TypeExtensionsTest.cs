using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Extensions;

namespace Vit.Core.MsTest.Extensions
{

    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void GetUnderlyingTypeIfNullable_Test()
        {   

            Assert.AreEqual(typeof(int?).GetUnderlyingTypeIfNullable(), typeof(int));

            Assert.AreEqual(typeof(int).GetUnderlyingTypeIfNullable(), typeof(int));

            Assert.AreEqual(typeof(Object).GetUnderlyingTypeIfNullable(), typeof(Object));

        }





        [TestMethod]
        public void Convert_Test()
        {
           
            #region (x.1)string
            
            int int1 = "5".Convert<int>();
            Assert.AreEqual(int1, 5);

            int int2 = (int)("5.1".Convert<float>());
            Assert.AreEqual(int2, 5);

            int int3 =  "5.1".Convert<float>().Convert<int>();
            Assert.AreEqual(int3, 5);

            try
            {

                //将会抛异常           
                "5.1".Convert<int?>();

                "5.1".Convert<int>();             
            }
            catch (Exception ex)
            {                
            }


            double double1 = "5.123456".Convert<double>();
            Assert.AreEqual(double1, 5.123456d);

            bool bool1 = "false".Convert<bool>();
            Assert.AreEqual(bool1, false);

            bool bool2 = "False".Convert<bool>();
            Assert.AreEqual(bool2, false);

            bool bool3 = "true".Convert<bool>();
            Assert.AreEqual(bool3, true);

            bool bool4 = "True".Convert<bool>();
            Assert.AreEqual(bool4, true);

            #endregion


        }

    }
}
