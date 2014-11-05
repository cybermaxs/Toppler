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
        private Granularity fakeGranularity = new Granularity("fake", 456, 10);

        #region Arguments

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildFlatMap_WhenInvalidTimeRange_ShouldThrowException()
        {
            new Granularity("test").BuildFlatMap(2, 1);
        }
        #endregion

        #region Single Element

        [TestMethod]
        public void BuildFlatMap_WhenSame_ShouldReturnAsingleElement()
        {
            var map = fakeGranularity.BuildFlatMap(fakeGranularity.Factor, fakeGranularity.Factor);

            Assert.IsNotNull(map);
            Assert.AreEqual(1, map.Length);
            Assert.AreEqual(fakeGranularity.Factor, map.First());
        }
        #endregion

        #region Multiples Element

        [TestMethod]
        public void BuildFlatMap_WhenMultiples_ShouldReturnASingleElement()
        {
            var start = fakeGranularity.Factor;

            var map = fakeGranularity.BuildFlatMap(start, 3 * start);

            Assert.IsNotNull(map);
            Assert.AreEqual((2 * start)/fakeGranularity.Factor, map.Length);

            //values
            Assert.AreEqual(start, map.First());
           
        }
        #endregion

        #region Day
        [TestMethod]
        public void BuildFlatMapForDays_WhenSame_ShouldReturnThisDay()
        {
            var map = Granularity.Day.BuildFlatMap(DateTime.UtcNow, 0);

            Assert.IsNotNull(map);
            Assert.AreEqual(1, map.Length);
            Assert.AreEqual(DateTime.UtcNow.Date.ToSecondsTimestamp(), map.First());
        }

        [TestMethod]
        public void BuildFlatMapForDays_WhenSame_ShouldReturnTwoDays()
        {
            var map = Granularity.Day.BuildFlatMap(DateTime.UtcNow, 1);

            Assert.IsNotNull(map);
            Assert.AreEqual(2, map.Length);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1).ToSecondsTimestamp(), map.First());
            Assert.AreEqual(DateTime.UtcNow.Date.ToSecondsTimestamp(), map.Skip(1).First());
        }
        #endregion
    }
}
