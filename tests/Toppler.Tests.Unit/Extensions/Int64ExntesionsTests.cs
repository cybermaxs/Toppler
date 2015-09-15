using System;
using Toppler.Extensions;
using Xunit;

namespace Monitoring.Api.Test.Extensions
{
    public class Int64EntensionsTests
    {
        [Fact]
        public void ToDateTime_Epoch_ShoulBeEqual()
        {
            Assert.Equal<DateTime>(new DateTime(2013, 1, 1, 0, 0, 0), (1356998400L).ToDateTime());
        }

        [Fact]
        public void ToRoundedTimestamp_Epoch_ShoulBeEqual()
        {
            Assert.Equal<long>(1356998400L, (1356998450L).ToRoundedTimestamp(60L));
        }
    }
}
