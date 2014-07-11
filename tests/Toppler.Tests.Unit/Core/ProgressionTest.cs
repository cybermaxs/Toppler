using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;
using System.Linq;

namespace Toppler.Tests.Unit.Core
{
    [TestClass]
    public class ProgressionTest
    {

        [TestMethod]
        public void Progression_None()
        {
            var NbItems = 5;
            var actual = Enumerable.Range(0, NbItems).Select(k => WeightFunction.Constant.Weight(k, NbItems)).ToArray();

            double[] expected = new double[] { 1D, 1D, 1D, 1D, 1D };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Progression_StdGeometric()
        {
            var NbItems = 5;
            var actual = Enumerable.Range(0, NbItems).Select(k => WeightFunction.StdGeometric.Weight(k, NbItems)).ToArray();

            double[] expected = new double[] { 2D, 4D, 8D, 16D, 32D };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Progression_StdArithmetic()
        {
            var NbItems = 5;
            var actual = Enumerable.Range(0, NbItems).Select(k => WeightFunction.StdArithmetic.Weight(k, NbItems)).ToArray();

            double[] expected = new double[] { 1D, 2D, 3D, 4D, 5D };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Progression_InvStdGeometric()
        {
            var NbItems = 5;
            var actual = Enumerable.Range(0, NbItems).Select(k => WeightFunction.InvStdGeometric.Weight(k, NbItems)).ToArray();

            double[] expected = new double[] { 32D, 16D, 8D, 4D, 2D };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Progression_InvStdLinear()
        {
            var NbItems = 5;
            var actual = Enumerable.Range(0, NbItems).Select(k => WeightFunction.InvStdArithmetic.Weight(k, NbItems)).ToArray();

            double[] expected = new double[] {5D, 4D, 3D, 2D, 1D};
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
