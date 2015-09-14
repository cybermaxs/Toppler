using System;
using System.Linq;
using Toppler.Core;
using Xunit;

namespace Toppler.Tests.Unit.Core
{
    public class GranularityProviderTest
    {
        [Fact]
        public void DefaultGranularityProvider_WhenCtor_ShouldReturn4Granularities()
        {
            var provider = new DefaultGranularityProvider();
            Assert.Equal(5, provider.GetGranularities().Count());
        }

        [Fact]
        public void DefaultGranularityProvider_WhenRegister_ShouldBeAdded()
        {
            var provider = new DefaultGranularityProvider();
            var newgran = new Granularity("test") { Factor = 45656 };
            var res = provider.RegisterGranularity(newgran);
            Assert.True(res);
            Assert.True(provider.GetGranularities().Contains(newgran));
        }

        [Fact]
        public void DefaultGranularityProvider_WhenDuplicates_ShouldNotBeAdded()
        {
            var provider = new DefaultGranularityProvider();
            var newgran = new Granularity("test") { Factor = 7987654 };
            var res1 = provider.RegisterGranularity(newgran);
            var res2 = provider.RegisterGranularity(newgran);
            Assert.True(res1);
            Assert.False(res2);
        }

        [Fact]
        public void DefaultGranularityProvider_WhenInvalidGranularity_ShouldThrowException()
        {
            var provider = new DefaultGranularityProvider();
            var newgran = new Granularity("test", 0, 0);

            Assert.Throws<ArgumentException>(() => { provider.RegisterGranularity(newgran); });       
        }
    }
}
