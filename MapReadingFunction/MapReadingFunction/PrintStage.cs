using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MapReadingFunction
{
    public static class PrintStage
    {
        public static void SaveAndPrintStage(StreamReader input)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ' };

            string mapRawData = input.ReadToEnd();

            string[] mapData = mapRawData.Split("\r\n");

            int row = mapData.Length - 1;
            int col = mapData[1].Length;

            int[,] data = new int[mapData.Length, mapData[1].Length];

            int hole = 0;
            int ball = 0;
            int playerX = 0;
            int playerY = 0;

            for (int i = 1; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    int mappingData = signal.IndexOf(mapData[i + 1][j]);
                    data[i, j] = mappingData;

                    if (mappingData == 1)
                    {
                        hole++;
                    }
                    else if (mappingData == 2)
                    {
                        ball++;
                    }
                    else if (mappingData == 3)
                    {
                        playerX = j + 1;
                        playerY = i + 1;
                    }
                }
            }

            Console.WriteLine(mapData[0]);
            Console.WriteLine();
            for (int i = 1; i < mapData.Length; ++i)
            {
                Console.WriteLine(mapData[i]);
            }
            Console.WriteLine();
            Console.WriteLine($"가로크기: {col}");
            Console.WriteLine($"세로크기: {row}");
            Console.WriteLine($"구멍의 수: {hole}");
            Console.WriteLine($"공의 수: {ball}");
            Console.WriteLine($"플레이어 위치: ({playerY}, {playerX})");
            Console.WriteLine();

        }
    }
}
