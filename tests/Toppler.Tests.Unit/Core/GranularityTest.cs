using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;
using Toppler.Extensions;
using StackExchange.Redis;

namespace Toppler.Tests.Unit.Core
{
    [TestClass]
    public class GranularityTest
    {
        private Granularity fakeGranularity = new Granularity("fake", 100, 456, 10);
        #region Arguments
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildMap_WhenInvalidTimeRange_ShouldThrowException()
        {
            new Granularity("test").BuildMap(2, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildFlatMap_WhenInvalidTimeRange_ShouldThrowException()
        {
            new Granularity("test").BuildFlatMap(2, 1);
        }
        #endregion

        #region Single Element
        [TestMethod]
        public void BuildMap_WhenSame_ShouldReturnAsingleElement()
        {
            var map = fakeGranularity.BuildMap(fakeGranularity.Factor * fakeGranularity.Size, fakeGranularity.Factor * fakeGranularity.Size);



            Assert.IsNotNull(map);
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(fakeGranularity.Factor * fakeGranularity.Size, map.Keys.First());
            //Assert.AreEqual(fakeGranularity.Factor * fakeGranularity.Size, map.Values.Cast<int>().First());
        }

        [TestMethod]
        public void BuildFlatMap_WhenSame_ShouldReturnAsingleElement()
        {
            var map = fakeGranularity.BuildFlatMap(fakeGranularity.Factor * fakeGranularity.Size, fakeGranularity.Factor * fakeGranularity.Size);


            Assert.IsNotNull(map);
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(fakeGranularity.Factor * fakeGranularity.Size, map.First().Key);
            Assert.AreEqual(fakeGranularity.Factor * fakeGranularity.Size, map.First().Value);
        }
        #endregion

        #region Multiples Element
        [TestMethod]
        public void BuildMap_WhenMultiples_ShouldReturnASingleElement()
        {
            var start = fakeGranularity.Size * fakeGranularity.Factor;

            var map = fakeGranularity.BuildMap(start, 3 * start);

            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);

            //keys
            Assert.AreEqual(start, (int)(map.Keys.First()));
            Assert.AreEqual(2*start, (int)(map.Keys.Skip(1).First()));
            Assert.AreEqual(3 * start, (int)(map.Keys.Skip(2).First()));

            //values
            Assert.AreEqual(start, (int)(map.First().Value.First()));
            Assert.AreEqual(start+5*fakeGranularity.Factor, (int)(map.First().Value.Skip(5).First()));
        }

        [TestMethod]
        public void BuildFlatMap_WhenMultiples_ShouldReturnASingleElement()
        {
            var start = fakeGranularity.Size * fakeGranularity.Factor;

            var map = fakeGranularity.BuildFlatMap(start, 3 * start);

            Assert.IsNotNull(map);
            Assert.AreEqual((2 * start)/fakeGranularity.Size*fakeGranularity.Factor+1, map.Count);

            //values
            Assert.AreEqual(start, map.First().Value);
           
        }
        #endregion
    }
}
