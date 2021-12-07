using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SokobanGame
{
    public static class Function
    {
        public static void StartGame()
        {
            const string STAGE_FILE_NAME = "StageFile.txt";
            List<string> stages = new List<string>();

            LoadStages(stages, STAGE_FILE_NAME);

            //foreach (var value in stages)
            //{
            //    Console.WriteLine(value);
            //}

            var data = ConvertToData(stages[1]);

            for (int i = 0; i < data.GetLength(0); ++i)
            {
                for (int j = 0; j < data.GetLength(1); ++j)
                {
                    Console.Write(data[i, j]);
                }
                Console.WriteLine();
            }

        }

        #region LoadingMap
        public static List<string> LoadStages (List<string> stages, string path)
        {
            const char DELIMETER = '=';

            if (!File.Exists(path))
            {
                return new List<string>();
            }

            string allStages = File.ReadAllText(path);
            string[] eachStages = allStages.Split(DELIMETER, StringSplitOptions.TrimEntries);

            stages.AddRange(eachStages);

            return stages;
        }

        public static int[,] ConvertToData (string stages)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ' };

            string[] mapDataSplit = stages.Split("\r\n");
            string[] mapData = new string[mapDataSplit.Length - 1];
            Array.Copy(mapDataSplit, 1, mapData, 0, mapData.Length);

            int row = mapData.Length;
            int col = mapData[0].Length;

            if(mapData[row - 1].Length != col)
            {
                mapData[row - 1]= RightLastString(mapData[row - 1], col);
            }

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


        #endregion

        public static void MovingCharacter(int[,] map, string move)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '0', ' ' };
            List<char> moveCom = new List<char>() { 'w', 'a', 's', 'd', 'q' };

            var pIndex = FindUserIndex(map);

            int y = pIndex[0];
            int x = pIndex[1];

            int loop = move.Length;

            for (int i = 0; i < loop; ++i)
            {
                int moveIndex = moveCom.IndexOf(move[i]);

                if ((move[i] == moveCom[0] || move[i] == moveCom[2]) && map[y - 1 + moveIndex, x] == 4)
                {
                    map[y, x] ^= map[y - 1 + moveIndex, x];
                    map[y - 1 + moveIndex, x] ^= map[y, x];
                    map[y, x] ^= map[y - 1 + moveIndex, x];

                    y = y - 1 + moveIndex;

                    Print(map);
                }
                else if ((move[i] == moveCom[1] || move[i] == moveCom[3]) && map[y, x - 2 + moveIndex] == 4)
                {
                    map[y, x] ^= map[y, x - 2 + moveIndex];
                    map[y, x - 2 + moveIndex] ^= map[y, x];
                    map[y, x] ^= map[y, x - 2 + moveIndex];

                    x = x - 2 + moveIndex;

                    Print(map);
                }
                else
                {
                    Print(map);
                }
            }
        }
        #region Helper

        public static string RightLastString(string lastRow, int length)
        {
            const char PADDING = ' ';

            StringBuilder addPadding = new StringBuilder(length);

            addPadding.Append(lastRow);
            int loop = length - lastRow.Length;

            for (int i = 0; i < loop; ++i)
            {
                addPadding.Append(PADDING);
            }
            lastRow = addPadding.ToString();

            return lastRow;
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

        #endregion 

    }
}
