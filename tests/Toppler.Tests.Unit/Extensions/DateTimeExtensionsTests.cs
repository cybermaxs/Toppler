using System;
using Toppler.Extensions;
using Xunit;

namespace Toppler.Tests.Unit.Extensions
{
    public class DateTimeExtensionsTests
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        [Fact]
        public void ToSecondsTimestamp_Epoch_ShoulBeZero()
        {
            Assert.Equal<double>(0, Epoch.ToSecondsTimestamp());
        }

        [Fact]
        public void ToTimestamp_Epoch_ShoulBeZero()
        {
            Assert.Equal<double>(0, Epoch.ToTimestamp());
        }

        [Fact]
        public void ToSecondsTimestamp_Now_ShoulBeEqual()
        {
            var now = DateTime.UtcNow;
            Assert.Equal<double>(Math.Floor(now.Subtract(Epoch).TotalSeconds), now.ToSecondsTimestamp());
        }

        [Fact]
        public void ToTimestamp_Now_ShoulBeEqual()
        {
            var now = DateTime.UtcNow;
            Assert.Equal<double>(Math.Floor(now.Subtract(Epoch).TotalMilliseconds), now.ToTimestamp());
        }

        [Fact]
        public void ToRoundedTimestamp_Now_ShoulBeEqual()
        {
            var now = DateTime.UtcNow;

            Assert.Equal<long>(now.ToSecondsTimestamp(), now.ToRoundedTimestamp(1));
            Assert.Equal<long>(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).ToSecondsTimestamp(), now.ToRoundedTimestamp(60));
            Assert.Equal<long>(new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).ToSecondsTimestamp(), now.ToRoundedTimestamp(3600));
            Assert.Equal<long>(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToSecondsTimestamp(), now.ToRoundedTimestamp(86400));

            Assert.Equal<double>(Math.Floor(now.Subtract(Epoch).TotalMilliseconds), now.ToTimestamp());
        }

    }
}
