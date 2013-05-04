using System;
using System.Text;

namespace MatrixTasks
{
    public class RotationWalkInMatrix 
    {
        static bool IsInRange(int arraySize, int x, int y)
        {
            return x >= 0 && x < arraySize && y >= 0 && y < arraySize;
        }

        static bool CalculateNextDirectionIfPossible( int[,] arr, Position position , DirectionVector currentDirection)
        {
            int nextX;
            int nextY;
            int arraySize = arr.GetLength(0);
            for (int i = 0; i < DirectionVector.DirectionsCount; i++)
            {
                nextX = position.X + currentDirection.Dx;
                nextY = position.Y + currentDirection.Dy;
                if (IsInRange(arraySize, nextX, nextY) && arr[nextX, nextY] == 0)
                {
                    return true;
                }
                currentDirection.Next();
            }

            return false;
        }

        static void FindCellToStartFrom ( int[,] arr, Position position )
        {
            position.X = 0;
            position.Y = 0;
            int arraySize = arr.GetLength(0);
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    if (arr[i, j] == 0)
                    {
                        position.X = i;
                        position.Y = j;
                        return;
                    }
                }
            }
        }

        static void PrintMatrixOnConsole(int[,] arr)
        {
            Console.WriteLine(GetMatrixForPrinting(arr));
        }

        public static string GetMatrixForPrinting(int[,] arr)
        {
            StringBuilder result = new StringBuilder();
            int arraySize = arr.GetLength(0);
            int offset = (arraySize * arraySize).ToString().Length + 1;
            string formatedPattern = "{0," + offset + "}";
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    result.AppendFormat(formatedPattern, arr[i, j]);
                }
                result.AppendLine();
            }

            return result.ToString();
        }

        static int GetMatrixSizeConsoleInput()
        {
            Console.Write("Enter a positive number in range [1,100]: ");
            string input = Console.ReadLine();
            int arraySize = 0;
            while (!int.TryParse(input, out arraySize) || arraySize < 1 || arraySize > 100)
            {
                Console.WriteLine("You haven't entered a correct positive number!");
                Console.Write("Enter a positive number in range [1,100]: ");
                input = Console.ReadLine();
            }
            return arraySize;
        }

        public static int[,] CalculateMatrix(int arraySize)
        {
            int[,] matrix = new int[arraySize, arraySize];
            int maxValue = arraySize * arraySize;
            Position position = new Position();
            DirectionVector direction = new DirectionVector();

            for (int value = 1; value <= maxValue; value++)
            {
                matrix[position.X, position.Y] = value;
                if (CalculateNextDirectionIfPossible(matrix, position, direction))
                {
                    position.X += direction.Dx;
                    position.Y += direction.Dy;
                }
                else
                {
                    FindCellToStartFrom(matrix, position);
                    direction.Reset();
                }
            }
            return matrix;
        }

        static void Main()
        {
            int arraySize = GetMatrixSizeConsoleInput();
            int[,] matrix = CalculateMatrix(arraySize);
            PrintMatrixOnConsole(matrix);
        }
    }
}
