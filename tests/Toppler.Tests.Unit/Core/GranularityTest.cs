using System;
using System.Linq;
using Toppler.Core;
using Toppler.Extensions;
using Xunit;

namespace Toppler.Tests.Unit.Core
{
    public class GranularityTest
    {
        private Granularity fakeGranularity = new Granularity("fake", 456, 10);

        #region Arguments

        [Fact]
        public void BuildFlatMap_WhenInvalidTimeRange_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => { new Granularity("test").BuildFlatMap(2, 1); });
        }
        #endregion

        #region Single Element

        [Fact]
        public void BuildFlatMap_WhenSame_ShouldReturnAsingleElement()
        {
            var map = fakeGranularity.BuildFlatMap(fakeGranularity.Factor, fakeGranularity.Factor);

            Assert.NotNull(map);
            Assert.Equal(1, map.Length);
            Assert.Equal(fakeGranularity.Factor, map.First());
        }
        #endregion

        #region Multiples Element

        [Fact]
        public void BuildFlatMap_WhenMultiples_ShouldReturnASingleElement()
        {
            var start = fakeGranularity.Factor;

            var map = fakeGranularity.BuildFlatMap(start, 3 * start);

            Assert.NotNull(map);
            Assert.Equal((2 * start)/fakeGranularity.Factor, map.Length);

            //values
            Assert.Equal(start, map.First());
           
        }
        #endregion

        #region Day
        [Fact]
        public void BuildFlatMapForDays_WhenSame_ShouldReturnThisDay()
        {
            var map = Granularity.Day.BuildFlatMap(DateTime.UtcNow, 0);

            Assert.NotNull(map);
            Assert.Equal(1, map.Length);
            Assert.Equal(DateTime.UtcNow.Date.ToSecondsTimestamp(), map.First());
        }

        [Fact]
        public void BuildFlatMapForDays_WhenSame_ShouldReturnTwoDays()
        {
            var map = Granularity.Day.BuildFlatMap(DateTime.UtcNow, 1);

            Assert.NotNull(map);
            Assert.Equal(2, map.Length);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(-1).ToSecondsTimestamp(), map.First());
            Assert.Equal(DateTime.UtcNow.Date.ToSecondsTimestamp(), map.Skip(1).First());
        }
        #endregion
    }
}
