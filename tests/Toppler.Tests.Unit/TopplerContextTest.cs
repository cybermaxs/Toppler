using System.Linq;
using Toppler.Core;
using Xunit;

namespace Toppler.Tests.Unit
{
    public class TopplerContextTest
    {
        [Fact]
        public void TopplerContext_WhenCtor_ShouldBeValid()
        {
            var ns = "blabla";
            var context = new TopplerContext(ns, 0,  new Granularity[] { Granularity.Second, Granularity.Minute });

            Assert.NotNull(context);
            Assert.NotNull(context.KeyFactory);
            Assert.NotNull(context.Granularities);

            Assert.Equal(ns, context.Namespace);
            Assert.Equal(ns, context.KeyFactory.Namespace);
            Assert.Equal(2, context.Granularities.Count());
            Assert.True(context.Granularities.Contains(Granularity.Second));
            Assert.True(context.Granularities.Contains(Granularity.Minute));
        }
    }
}
