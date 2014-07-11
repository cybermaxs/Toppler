using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;

namespace Toppler.Tests.Unit.Core
{
    [TestClass]
    public class KeyFactoryTest
    {
        [TestMethod]
        public void KeyFactory_NoNs_ShouldReturnReturnKey()
        {
            var keyFactory = new DefaultKeyFactory(string.Empty);

            Assert.AreEqual(string.Empty, keyFactory.Namespace);
            Assert.AreEqual("1:2:3", keyFactory.NsKey("1", "2", "3"));
            Assert.AreEqual("1:2:3", keyFactory.RawKey("1", "2", "3"));
        }

        [TestMethod]
        public void KeyFactory_Ns_ShouldReturnReturnKey()
        {
            var keyFactory = new DefaultKeyFactory("ns");

            Assert.AreEqual("ns", keyFactory.Namespace);
            Assert.AreEqual("ns:1:2:3", keyFactory.NsKey("1", "2", "3"));
            Assert.AreEqual("1:2:3", keyFactory.RawKey("1", "2", "3"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void KeyFactory_WhenInvalidNs_ShouldThrowExeption()
        {
            var keyFactory = new DefaultKeyFactory("ns:dfsdf:sdf");
        }
    }
}
