using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Extensions;

namespace Toppler.Tests.Unit.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        [TestMethod]
        public void ToSecondsTimestamp_Epoch_ShoulBeZero()
        {
            Assert.AreEqual<double>(0, Epoch.ToSecondsTimestamp(), "shoud be zero");
        }

        [TestMethod]
        public void ToTimestamp_Epoch_ShoulBeZero()
        {
            Assert.AreEqual<double>(0, Epoch.ToTimestamp(), "shoud be zero");
        }

        [TestMethod]
        public void ToSecondsTimestamp_Now_ShoulBeEqual()
        {
            DateTime now = DateTime.UtcNow;
            Assert.AreEqual<double>(Math.Floor(now.Subtract(Epoch).TotalSeconds), now.ToSecondsTimestamp(), "shoud be equal");
        }

        [TestMethod]
        public void ToTimestamp_Now_ShoulBeEqual()
        {
            DateTime now = DateTime.UtcNow;
            Assert.AreEqual<double>(Math.Floor(now.Subtract(Epoch).TotalMilliseconds), now.ToTimestamp(), "shoud be equal");
        }

        [TestMethod]
        public void ToRoundedTimestamp_Now_ShoulBeEqual()
        {
            DateTime now = DateTime.UtcNow;

            Assert.AreEqual<long>(now.ToSecondsTimestamp(), now.ToRoundedTimestamp(1), "seconds");
            Assert.AreEqual<long>(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).ToSecondsTimestamp(), now.ToRoundedTimestamp(60), "minutes");
            Assert.AreEqual<long>(new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).ToSecondsTimestamp(), now.ToRoundedTimestamp(3600), "hours");
            Assert.AreEqual<long>(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToSecondsTimestamp(), now.ToRoundedTimestamp(86400), "days");

            Assert.AreEqual<double>(Math.Floor(now.Subtract(Epoch).TotalMilliseconds), now.ToTimestamp(), "shoud be equal");
        }

    }
}
