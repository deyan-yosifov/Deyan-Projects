using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixTasks;

namespace TestMatrixTasks
{
    [TestClass]
    public class TestDirectionVector
    {
        [TestMethod]
        public void TestInitialization()
        {
            DirectionVector direction = new DirectionVector();
            Assert.AreEqual(direction.Dx == 1 && direction.Dy == 1, true, "Direction Reset() doesn't work correctly!");
        }

        [TestMethod]
        public void TestNext()
        {
            DirectionVector direction = new DirectionVector();
            direction.Next();
            Assert.AreEqual(direction.Dx == 1 && direction.Dy == 0, true, "Direction Next doesn't work correctly!");
        }

        [TestMethod]
        public void TestNextRevertion()
        {
            DirectionVector direction = new DirectionVector();
            for (int i = 0; i < DirectionVector.DirectionsCount; i++)
            {
                direction.Next();
            }
            Assert.AreEqual(direction.Dx == 1 && direction.Dy == 1, true, "Direction Next doesn't reverts correctly!");
        }
    }
}
