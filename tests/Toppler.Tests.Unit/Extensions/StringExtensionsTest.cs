using Toppler.Extensions;
using Xunit;

namespace Monitoring.Api.Test.Extensions
{
    public class StringExtensionsTest
    {
        #region Strings
        [Fact]
        public void AlphaNumericString_WhenSimpleString_ShouldReturnTrue()
        {
            var mykey = "Adsdflk2314dfsf";
            Assert.True(mykey.AlphaNumericString(), "should be alphanum");
        }

        [Fact]
        public void AlphaNumericString_WhenStringAndUnderscore_ShouldReturnTrue()
        {
            var mykey = "Adsd_flk23_14dfsf";
            Assert.True(mykey.AlphaNumericString(), "should be alphanum");
        }

        [Fact]
        public void AlphaNumericString_WhenStringAndDots_ShouldReturnFalse()
        {
            var mykey = "Adsd:flk23:14dfsf";
            Assert.False(mykey.AlphaNumericString(), "should be alphanum");
        }

        [Fact]
        public void AlphaNumericString_WhenNonAlphaNumString_ShouldReturnFalse()
        {
            var mykey = "Ads dflk2314d\nfsf";
            Assert.False(mykey.AlphaNumericString(), "should not be alphanum");
        }
        #endregion

        #region ArrayOf Strings
        [Fact]
        public void RedisKey_WhenEmpty_ShouldReturnEmptyString()
        {
            Assert.Equal(string.Empty, new string[] { }.AsRedisKey());
        }

        [Fact]
        public void RedisKey_Default_ShouldReturnCompisiteKey()
        {
            Assert.Equal("part1:part2:part3", new string[] { "part1", "part2", "part3" }.AsRedisKey());
        }

        [Fact]
        public void RedisKey_WhenEmptyPart_ShouldReturnKey()
        {
            Assert.Equal("part1:part3", new string[] { "part1", "", "part3" }.AsRedisKey());
        }

        #endregion
    }
}
