using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;

namespace Toppler.Tests.Unit
{
    [TestClass]
    public class TopplerContextTest
    {
        [TestMethod]
        public void TopplerContext_WhenCtor_ShouldBeValid()
        {
            var ns = "blabla";
            TopplerContext context = new TopplerContext(ns, 0,  new Granularity[] { Granularity.Second, Granularity.Minute });

            Assert.IsNotNull(context);
            Assert.IsNotNull(context.KeyFactory);
            Assert.IsNotNull(context.Granularities);

            Assert.AreEqual(ns, context.Namespace);
            Assert.AreEqual(ns, context.KeyFactory.Namespace);
            Assert.AreEqual(2, context.Granularities.Count());
            Assert.IsTrue(context.Granularities.Contains(Granularity.Second));
            Assert.IsTrue(context.Granularities.Contains(Granularity.Minute));
        }
    }
}
