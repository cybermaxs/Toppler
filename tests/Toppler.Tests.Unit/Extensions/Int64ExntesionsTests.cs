using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Extensions;

namespace Monitoring.Api.Test.Extensions
{
    [TestClass]
    public class Int64EntensionsTests
    {
        [TestMethod]
        public void ToDateTime_Epoch_ShoulBeEqual()
        {
            Assert.AreEqual<DateTime>(new DateTime(2013, 1, 1, 0, 0, 0), (1356998400L).ToDateTime(), "shoud be equal");
        }

        [TestMethod]
        public void ToRoundedTimestamp_Epoch_ShoulBeEqual()
        {
            Assert.AreEqual<long>(1356998400L, (1356998450L).ToRoundedTimestamp(60L), "shoud be equal");
        }
    }
}
