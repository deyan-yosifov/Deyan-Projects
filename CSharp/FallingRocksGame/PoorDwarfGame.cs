using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

class PoorDwarfGame
{
    public static int newWidth;
    public static int newHeight;
    public static int oldWindowSizeX;
    public static int oldWindowSizeY;
    public static int oldBufferSizeX;
    public static int oldBufferSizeY;

    public static StringBuilder randString;
    public static Random rand = new Random();
    public static ConsoleKey key = ConsoleKey.Spacebar;
    public static ConsoleKeyInfo keyInfo;
    public static int sleep = 150;

    public static int startCount = 2;
    public static int difficulty = 0;
    public static int playerX = 0;
    public static int playerY = 0;
    public static bool movePlayer = true;
    public static int score = 0;
    public static bool redrawScore = true;
    public static bool playGame = true;

    public static int lastDensity = 0;
    public static readonly char[] rockTypes = { '^', '@', '*', '&', '+', '%', '$', '#', '!', '.', ';', '-' };
    public static readonly char[] goodTypes = { '\u0001', '\u0002' };
    public static Queue<int> rockX = new Queue<int>();
    public static Queue<int> rockDensity = new Queue<int>();
    public static List<int> goodX = new List<int>();
    public static List<int> goodY = new List<int>();
    public static bool evenTurn = true;

    public static void Main()
    {
        WelcomeScreen();
        while (playGame)
        {
            StartGame();
            key = Console.ReadKey(true).Key;
            while (key != ConsoleKey.Escape)
            {
                if (Console.KeyAvailable)
                {
                    movePlayer = true;
                    keyInfo = Console.ReadKey(true);
                    key = keyInfo.Key;
                    ClearInputStreamBuffer();
                }
                if (key == ConsoleKey.Spacebar) { PauseGame(); }
                Console.MoveBufferArea(0, 2, newWidth, newHeight - 3, 0, 3);
                DrawNewRocks();
                GetNewData();
                RedrawGoods();
                CheckCollision();
                DrawScore();
                MovePlayer();
                Thread.Sleep(sleep);
                if (!playGame) { break; }
            }
            ResetGame();
        }
        EndGame();
    }

    public static void PauseGame()
    {
        Console.ReadKey(true);
        key = ConsoleKey.Enter;
    }

    public static void ClearInputStreamBuffer()
    {
        while (Console.KeyAvailable)
        {
            Console.ReadKey(true);
        }
    }

    public static void ResetGame()
    {
        if (key != ConsoleKey.Escape)
        {
            redrawScore = true;
            DrawScore();
            Console.Beep(100, 1000);
            Console.ForegroundColor = ConsoleColor.Cyan;

            //---Draw Game Over Screen
            int i = -3;
            Console.SetCursorPosition(newWidth / 2 - 19, newHeight / 2 + ++i);
            Console.Write("-------------------------------------");
            Console.SetCursorPosition(newWidth / 2 - 19, newHeight / 2 + ++i);
            Console.Write("---Sorry! You have been badly hit!---");
            Console.SetCursorPosition(newWidth / 2 - 19, newHeight / 2 + ++i);
            Console.Write("---Your score is: {0}---------", score.ToString().PadRight(10, '-'));
            Console.SetCursorPosition(newWidth / 2 - 19, newHeight / 2 + ++i);
            Console.Write("---To play again press Enter.--------");
            Console.SetCursorPosition(newWidth / 2 - 19, newHeight / 2 + ++i);
            Console.Write("---To quit press Escape.-------------");
            Console.SetCursorPosition(newWidth / 2 - 19, newHeight / 2 + ++i);
            Console.Write("-------------------------------------");

            while (key != ConsoleKey.Enter && key != ConsoleKey.Escape)
            {
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter) { playGame = true; }
                if (key == ConsoleKey.Escape) { playGame = false; }
            }
        }
        else
        {
            playGame = false;
        }


    }

    public static void WelcomeScreen()
    {
        Console.CursorVisible = true;
        Console.Title = "POOR DWARF GAME by Deyan Yosifov";
        //------Set Colors
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Clear();

        //-----Get Original Window Size
        oldWindowSizeX = Console.WindowWidth;
        oldWindowSizeY = Console.WindowHeight;
        oldBufferSizeX = Console.BufferWidth;
        oldBufferSizeY = Console.BufferHeight;

        //-----Set Program New Window Size
        newWidth = Math.Min(Console.LargestWindowWidth, 80);
        newHeight = Math.Min(Console.LargestWindowHeight, 40);
        Console.SetWindowSize(newWidth, newHeight);
        Console.BufferWidth = newWidth;
        Console.BufferHeight = newHeight;

        //-----Set Difficulty
        difficulty = Math.Max(2, newWidth / 20);

        //-----Welcome Screen
        int i = -8;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("--------------------------------------------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---POOR DWARF GAME--------------------------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("-------------------------------------------------by Deyan Yosifov---");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("--------------------------------------------------------------------");

        Console.ForegroundColor = ConsoleColor.Magenta;
        i++;
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("--------------------------------------------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---Move left and right with the arrow keys.-------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---Hold Shift to move faster.---------------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---Press Spacebar to Pause the game.--------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---Beware of the yellow falling rocks.------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---Collect the blinking red faces, to gain points for your score!---");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("---Good Luck!-------------------------------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("--------------------------------------------------------------------");
        Console.SetCursorPosition(newWidth / 2 - 34, newHeight / 2 + ++i);
        Console.Write("Press any key to continue ...");
        Console.ReadKey(true);
    }

    public static void RedrawGoods()
    {
        if (evenTurn)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        for (int i = 0; i < goodX.Count; i++)
        {
            Console.SetCursorPosition(goodX[i], goodY[i]);
            if (evenTurn)
            {
                Console.Write(goodTypes[0]);
            }
            else
            {
                Console.Write(goodTypes[1]);
            }
            if (goodY[i] < newHeight - 1) { goodY[i]++; }
        }
        evenTurn = !evenTurn;
    }

    public static void CheckCollision()
    {
        if (startCount < newHeight - 1)
        {
            startCount++;
        }
        else
        {
            for (int i = 0; i < rockDensity.Peek(); i++)
            {
                if (rockX.Peek() % newWidth >= (playerX - 1) && rockX.Peek() % newWidth <= (playerX + 1))
                {
                    if (rockX.Peek() < newWidth)
                    {
                        playGame = false;
                    }
                    else
                    {
                        score++;
                        Console.Beep(1000, 20);
                        redrawScore = true;
                    }
                }
                if (rockX.Peek() >= newWidth)
                {
                    goodX.RemoveAt(0);
                    goodY.RemoveAt(0);
                }
                rockX.Dequeue();
            }
            rockDensity.Dequeue();
        }
    }

    public static void GetNewData()
    {
        for (int i = 0; i < randString.Length; i++)
        {
            if (randString[i] != ' ')
            {
                if (randString[i] != goodTypes[0])
                {
                    rockX.Enqueue(i);
                }
                else
                {
                    rockX.Enqueue(i + newWidth);
                    goodX.Add(i);
                    goodY.Add(2);
                }
            }
        }
        //randString.Clear();
    }

    public static void DrawNewRocks()
    {
        lastDensity = rand.Next(difficulty);
        rockDensity.Enqueue(lastDensity);
        randString = new StringBuilder(lastDensity);
        for (int i = 0; i < (newWidth - lastDensity - 1); i++)
        {
            randString.Append(' ');
        }
        for (int i = 0; i < lastDensity; i++)
        {
            randString.Insert(1 + rand.Next(randString.Length - 1), rand.Next(rockTypes.Length + 1) < rockTypes.Length ? rockTypes[rand.Next(rockTypes.Length)] : goodTypes[0]);
        }
        Console.SetCursorPosition(0, 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(randString.ToString());
    }

    public static void MovePlayer()
    {
        if (movePlayer)
        {
            movePlayer = false;
            if (key == ConsoleKey.LeftArrow && playerX > 2)
            {
                //ErasePlayer();
                if (keyInfo.Modifiers != ConsoleModifiers.Shift)
                {
                    playerX--;
                }
                else
                {
                    playerX = playerX - 2;
                }
            }
            else if (key == ConsoleKey.RightArrow && playerX < (newWidth - 3))
            {
                //ErasePlayer();
                if (keyInfo.Modifiers != ConsoleModifiers.Shift)
                {
                    playerX++;
                }
                else
                {
                    playerX = playerX + 2;
                }
            }
        }
        if (playGame) { DrawPlayer(); }
    }

    public static void DrawPlayer()
    {
        Console.SetCursorPosition(playerX - 1, playerY);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("(0)");
    }

    public static void ErasePlayer()
    {
        Console.SetCursorPosition(playerX - 1, playerY);
        Console.Write("   ");
    }

    public static void DrawScore()
    {
        if (redrawScore)
        {
            redrawScore = false;
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Score: {0} ", score);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(goodTypes[0]);
        }
    }

    public static void SetBeginValues()
    {
        startCount = 2;
        score = 0;
        redrawScore = true;
        rockX.Clear();
        rockDensity.Clear();
        goodX.Clear();
        goodY.Clear();
        evenTurn = true;
    }

    public static void StartGame()
    {
        Console.CursorVisible = false;
        SetBeginValues();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;

        //------First Drawings
        for (int i = 0; i < newWidth; i++)
        {
            Console.SetCursorPosition(i, 1);
            Console.Write('=');
        }
        Console.SetCursorPosition(newWidth - 30, 0);
        Console.Write("Space = Pause  Escape = Quit");
        DrawScore();
        playerX = newWidth / 2;
        playerY = newHeight - 1;
        DrawPlayer();
    }

    public static void EndGame()
    {
        Console.CursorVisible = true;
        //-----Restore Original Window Size And Colors
        Console.ResetColor();
        Console.Clear();
        Console.SetWindowSize(oldWindowSizeX, oldWindowSizeY);
        Console.BufferWidth = oldBufferSizeX;
        Console.BufferHeight = oldBufferSizeY;
    }

}