using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixTasks;

namespace TestMatrixTasks
{
    [TestClass]
    public class TestPosition
    {
        [TestMethod]
        public void TestInitialization()
        {
            Position position = new Position();
            Assert.AreEqual(position.X == 0 && position.Y == 0, true, "Begin values not assigned correctly");
        }
    }
}
