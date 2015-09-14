using System;
using Toppler.Core;
using Xunit;

namespace Toppler.Tests.Unit.Core
{
    public class KeyFactoryTest
    {
        [Fact]
        public void KeyFactory_NoNs_ShouldReturnReturnKey()
        {
            var keyFactory = new DefaultKeyFactory(string.Empty);

            Assert.Equal(string.Empty, keyFactory.Namespace);
            Assert.Equal("1:2:3", keyFactory.NsKey("1", "2", "3"));
            Assert.Equal("1:2:3", keyFactory.RawKey("1", "2", "3"));
        }

        [Fact]
        public void KeyFactory_Ns_ShouldReturnReturnKey()
        {
            var keyFactory = new DefaultKeyFactory("ns");

            Assert.Equal("ns", keyFactory.Namespace);
            Assert.Equal("ns:1:2:3", keyFactory.NsKey("1", "2", "3"));
            Assert.Equal("1:2:3", keyFactory.RawKey("1", "2", "3"));
        }

        [Fact]
        public void KeyFactory_WhenInvalidNs_ShouldThrowExeption()
        {
            Assert.Throws<ArgumentException>(() => { new DefaultKeyFactory("ns:dfsdf:sdf"); });
        }
    }
}
