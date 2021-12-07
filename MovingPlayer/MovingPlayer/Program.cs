using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MovingPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            string stage = "Stage2\r\n  #######  \r\n###  O  ###\r\n#    o    #\r\n# Oo P oO #\r\n###  o  ###\r\n #   O  #  \r\n ########  ";

            byte[] buffer = Encoding.UTF8.GetBytes(stage);

            using (StreamReader READER = new StreamReader(new MemoryStream(buffer)))
            using (StreamWriter WRITER = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true })
            {
                int[,] stageTable = new int[7, 11];
                stageTable = ReadStage(READER, WRITER);

                Console.WriteLine(stage);

                while (true)
                {
                    Console.WriteLine("SOKOBAN>");
                    string move = Console.ReadLine();

                    if (move == "q")
                    {
                        break;
                    }

                    MovingCharacter(stageTable, move);
                }
            }
        }

        public static int[,] ReadStage(StreamReader input, StreamWriter output)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ' };

            string mapRawData = input.ReadToEnd();

            string[] mapDataSplit = mapRawData.Split("\r\n");
            string[] mapData = new string[mapDataSplit.Length - 1];
            Array.Copy(mapDataSplit, 1, mapData, 0, mapData.Length);

            int row = mapData.Length;
            int col = mapData[0].Length;

            int[,] data = new int[row, col];

            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    int mappingData = signal.IndexOf(mapData[i][j]);
                    data[i, j] = mappingData;
                }
            }

            return data;
        }

        public static void MovingCharacter(int[,] map, string move)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ' };
            List<char> moveCom = new List<char>() { 'w', 'a', 's', 'd', 'q' };

            var pIndex = FindUserIndex(map);

            int y = pIndex[0];
            int x = pIndex[1];

            int loop = move.Length;

            for (int i = 0; i < loop; ++i)
            {
                int moveIndex = moveCom.IndexOf(move[i]);

                if ((move[i] == moveCom[0] || move[i] == moveCom[2]) && map[y - 1 + moveIndex, x] > 2)
                {
                    map[y, x] ^= map[y - 1 + moveIndex, x];
                    map[y - 1 + moveIndex, x] ^= map[y, x];
                    map[y, x] ^= map[y - 1 + moveIndex, x];

                    y = y - 1 + moveIndex;

                    Print(map);

                    if (move[i] == moveCom[0])
                    {
                        Console.WriteLine($"{move[i]}: 위로 이동합니다.");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine($"{move[i]}: 아래로 이동합니다.");
                        Console.WriteLine();
                    }
                }
                else if ((move[i] == moveCom[1] || move[i] == moveCom[3]) && map[y, x - 2 + moveIndex] > 2)
                {
                    map[y, x] ^= map[y, x - 2 + moveIndex];
                    map[y, x - 2 + moveIndex] ^= map[y, x];
                    map[y, x] ^= map[y, x - 2 + moveIndex];

                    x = x - 2 + moveIndex;

                    Print(map);

                    if (move[i] == moveCom[1])
                    {
                        Console.WriteLine($"{move[i]}: 왼쪽으로 이동합니다.");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine($"{move[i]}: 오른쪽으로 이동합니다.");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Print(map);

                    Console.WriteLine($"(경고!) 해당명령을 수행할 수 없습니다.");
                    Console.WriteLine();
                }
            }


        }

        public static int[] FindUserIndex(int[,] stage)
        {
            const int PLAYER_NUMBER = 3;

            int[] result = new int[2];

            for (int i = 0; i < stage.GetLength(0); ++i)
            {
                for (int j = 0; j < stage.GetLength(1); ++j)
                {
                    if (stage[i, j] == PLAYER_NUMBER)
                    {
                        result[0] = i; // y좌표
                        result[1] = j; // x좌표
                        break;
                    }
                }
            }

            return result;
        }

        public static void Print(int[,] stage)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ' };

            for (int i = 0; i < stage.GetLength(0); ++i)
            {
                for (int j = 0; j < stage.GetLength(1); ++j)
                {
                    Console.Write(signal[stage[i, j]]);
                }
                Console.WriteLine();
            }

        }

    }
}