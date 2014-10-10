using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;

namespace Toppler.Tests.Unit.Core
{
    [TestClass]
    public class GranularityProviderTest
    {
        [TestMethod]
        public void DefaultGranularityProvider_WhenCtor_ShouldReturn4Granularities()
        {
            var provider = new DefaultGranularityProvider();
            Assert.AreEqual(4, provider.GetGranularities().Count());
        }


        [TestMethod]
        public void DefaultGranularityProvider_WhenRegister_ShouldBeAdded()
        {
            var provider = new DefaultGranularityProvider();
            Granularity newgran = new Granularity("test");
            var res = provider.RegisterGranularity(newgran);
            Assert.IsTrue(res);
            Assert.IsTrue(provider.GetGranularities().Contains(newgran));
        }

        [TestMethod]
        public void DefaultGranularityProvider_WhenDuplicates_ShouldNotBeAdded()
        {
            var provider = new DefaultGranularityProvider();
            Granularity newgran = new Granularity("test");
            var res1 = provider.RegisterGranularity(newgran);
            var res2 = provider.RegisterGranularity(newgran);
            Assert.IsTrue(res1);
            Assert.IsFalse(res2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DefaultGranularityProvider_WhenInvalidGranularity_ShouldThrowException()
        {
            var provider = new DefaultGranularityProvider();
            Granularity newgran = new Granularity("test", 0, 0, 0);
            provider.RegisterGranularity(newgran);
        }


    }
}
