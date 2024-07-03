using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sers.Core.Module.Api.RouteMap;

namespace Sers.Core.MsTest.Module.Api.RouteMap
{
    [TestClass]
    public class RouteMapTest
    {

        class Service
        {
            public string route;
        }

        [TestMethod]
        public void RouteMap_TestMethod1()
        {

            var map = new RouteMap<Service>();
            //var map = new Sers.ServiceCenter.ApiCenter.GenericApiMng.DataMap<Service>();


            string route;

            route = "/A/*";
            map.Set(route, new Service { route = route });

            route = "/A/B/*";
            map.Set(route, new Service { route = route });
            route = "/A/B/C/D/*";
            map.Set(route, new Service { route = route });
            route = "/A/B1/*";
            map.Set(route, new Service { route = route });


            //test


            Assert.AreEqual("/A/B/*", map.Routing("/A/B/a.html")?.route);

            Assert.AreEqual("/A/B/C/D/*", map.Routing("/A/B/C/D/a.html")?.route);

            Assert.AreEqual("/A/B1/*", map.Routing("/A/B1/a.html")?.route);


            map.Remove("/A/B/*");

            Assert.AreEqual("/A/*", map.Routing("/A/B/a.html")?.route);

            map.Remove("/A/B/C/D/*");




        }


    }
}
