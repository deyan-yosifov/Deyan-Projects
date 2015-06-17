using Deyo.Core.Mathematics.Algebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CoreTests.Mathematics.Algebra
{
    [TestClass]
    public class AlgebraExtensionTests
    {
        [TestMethod]
        public void TestMatrixTransformationsToTheRight()
        {
            Point actual = new Matrix(1, 2, 3, 4, 0, 0).Transform(new Point(1, 2));
            Point expected = new Point(7, 10);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMatrixMultiplicationsToTheRight()
        {
            Matrix a = new Matrix(1, 2, 3, 4, 0, 0);
            Matrix b = new Matrix(5, 6, 7, 8, 0, 0);

            Matrix actual = a.MultiplyBy(b);
            Matrix expected = new Matrix(19, 22, 43, 50, 0, 0);

            Assert.AreEqual(expected, actual);

            a.Append(b);
            Assert.AreEqual(a, actual);
        }
    }
}
