using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Extensions;

namespace Monitoring.Api.Test.Extensions
{
   [TestClass]
    public class StringExtensionsTest
    {
        #region Strings
        [TestMethod]
        public void AlphaNumericString_WhenSimpleString_ShouldReturnTrue()
        {
            string mykey = "Adsdflk2314dfsf";
            Assert.IsTrue(mykey.AlphaNumericString(), "should be alphanum");
        }

        [TestMethod]
        public void AlphaNumericString_WhenStringAndUnderscore_ShouldReturnTrue()
        {
            string mykey = "Adsd_flk23_14dfsf";
            Assert.IsTrue(mykey.AlphaNumericString(), "should be alphanum");
        }

        [TestMethod]
        public void AlphaNumericString_WhenStringAndDots_ShouldReturnFalse()
        {
            string mykey = "Adsd:flk23:14dfsf";
            Assert.IsFalse(mykey.AlphaNumericString(), "should be alphanum");
        }

        [TestMethod]
        public void AlphaNumericString_WhenNonAlphaNumString_ShouldReturnFalse()
        {
            string mykey = "Ads dflk2314d\nfsf";
            Assert.IsFalse(mykey.AlphaNumericString(), "should not be alphanum");
        }
        #endregion

        #region ArrayOf Strings
        [TestMethod]
        public void RedisKey_WhenEmpty_ShouldReturnEmptyString()
        {
            Assert.AreEqual(string.Empty, new string[] { }.AsRedisKey());
        }

        [TestMethod]
        public void RedisKey_Default_ShouldReturnCompisiteKey()
        {
            Assert.AreEqual("part1:part2:part3", new string[] { "part1", "part2", "part3" }.AsRedisKey());
        }

        [TestMethod]
        public void RedisKey_WhenEmptyPart_ShouldReturnKey()
        {
            Assert.AreEqual("part1:part3", new string[] { "part1", "", "part3" }.AsRedisKey());
        }

        #endregion
    }
}
