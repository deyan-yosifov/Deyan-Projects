using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixTasks;

namespace TestMatrixTasks
{
    [TestClass]
    public class TestRotationWalkInMatrix
    {
        [TestMethod]
        public void TestMatrixCalculator()
        {
            int[,] matrix = RotationWalkInMatrix.CalculateMatrix(6);
            int[,] testMatrix = 
            {
                {1,  16,  17,  18,   19,  20},
                {15,   2,  27,  28,  29,  21},
                {14,  31,   3,  26,  30,  22},
                {13,  36,  32,   4,  25,  23},
                {12,  35,  34,  33,   5,  24},
                {11,  10,   9,   8,   7,   6}
            };

            string firstOutPut = RotationWalkInMatrix.GetMatrixForPrinting(matrix);
            string secondOutPut = RotationWalkInMatrix.GetMatrixForPrinting(testMatrix);

            Assert.AreEqual(firstOutPut, secondOutPut, "Sample output is not working!");
        }
    }
}
