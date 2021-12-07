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

            Console.WriteLine($"소코반 게임 시작");

            for (int i = 1; i < 2; ++i)
            {
                Console.WriteLine($"Stage{i}");

                var rawData = ConvertToData(stages[i]);
                int[,] data = new int[rawData.GetLength(0), rawData.GetLength(1)];
                Array.Copy(rawData, data, rawData.GetLength(0) * rawData.GetLength(1));

                var hallsIndex = FindHallIndex(data);

                Print(data);

                bool keepContinue = FunctionRecursive(data, rawData, hallsIndex);

                if (keepContinue)
                {
                    break;
                }

                //    while (true)
                //    {
                //        Console.WriteLine("SOKOBAN>");

                //        string move = Console.ReadLine();

                //        if (move == "q")
                //        {
                //            goto EXIT;
                //        }

                //        if (move == "r")
                //        {
                //            data = rawData;
                //        }
                //        else
                //        {
                //            data = MovingCharacter(data, hallsIndex, move);
                //        }

                //        Print(data);

                //        if (StageClear(data, hallsIndex))
                //        {
                //            Console.WriteLine($"Stage{i} 클리어 하셨습니다");
                //            break;
                //        }
                //    }
                //}

                //EXIT:
                //Console.WriteLine("게임을 종료합니다");
            }
        }

        #region Function

        public static bool FunctionRecursive(int[,] data, int[,] rawData, List<List<int>> hallsIndex)
        {
            List<char> command = new List<char>() { 'w', 'a', 's', 'd', 'q', 'r', 'u', 'U' };

            int row = data.GetLength(0);
            int col = data.GetLength(1);

            //int[,] undo = new int[row, col];
            //Array.Copy(data, undo, row * col);

            //Print(undo);

            Console.WriteLine("SOKOBAN>");

            string move = Console.ReadLine();

            if (move == "q")
            {
                return true;
            }

            if (move == "r")
            {
                data = rawData;
            }
            else if(command.Contains(move[0]))
            {
                data = MovingCharacter(data, hallsIndex, move);
            }
            else
            {
                Console.WriteLine("실행할 수 없습니다");
            }

            //int[,] nowData = new int[row, col];
            //Array.Copy(data, nowData, row * col);

            //if (move == "u")
            //{
            //    Array.Copy(undo, data, row * col);
            //}
            //else if (move == "U")
            //{
            //    Array.Copy(nowData, data, row * col);
            //}

            Print(data);

            if (StageClear(data, hallsIndex))
            {
                Console.WriteLine($"Stage 클리어 하셨습니다");
                return false;
            }

            FunctionRecursive(data, rawData, hallsIndex);
            return false;
        }


        #endregion


        #region LoadingMap
        public static void LoadStages(List<string> stages, string path)
        {
            const char DELIMETER = '=';

            if (!File.Exists(path))
            {
                return;
            }

            string allStages = File.ReadAllText(path);
            string[] eachStages = allStages.Split(DELIMETER, StringSplitOptions.TrimEntries);

            stages.AddRange(eachStages);
        }

        public static int[,] ConvertToData(string stages)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };

            string[] mapDataSplit = stages.Split("\r\n");
            string[] mapData = new string[mapDataSplit.Length - 1];
            Array.Copy(mapDataSplit, 1, mapData, 0, mapData.Length);

            int row = mapData.Length;
            int col = mapData[1].Length;

            for (int i = 0; i < row; ++i)
            {
                if (mapData[i].Length != col)
                {
                    mapData[i] = RightLastString(mapData[i], col);
                }
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

        #region MovingFunction
        public static int[,] MovingCharacter(int[,] map, List<List<int>> halls, string move)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };
            List<char> moveCom = new List<char>() { 'w', 'a', 's', 'd', 'q' };

            var pIndex = FindUserIndex(map);

            int y = pIndex[0];
            int x = pIndex[1];

            int moveIndex = moveCom.IndexOf(move[0]);
            int[,] resultData = new int[map.GetLength(0), map.GetLength(1)];

            switch (moveIndex)
            {
                case 0:
                case 2:
                    resultData = MoveUpOrDown(map, x, y, moveIndex, halls);
                    break;

                case 1:
                case 3:
                    resultData = MoveLeftOrRight(map, x, y, moveIndex, halls);
                    break;


                default:
                    break;
            }

            return resultData;
        }

        public static int[,] MoveUpOrDown(int[,] map, int x, int y, int direction, List<List<int>> halls)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };

            int upOrDown = direction - 1;
            int nextBall = upOrDown * 2;

            int move = map[y + upOrDown, x];

            switch (move)
            {
                case 1:

                    map[y, x] += 2;
                    map[y + upOrDown, x] += 2;
                    break;

                case 2:
                    if (map[y + nextBall, x] == 5)
                    {
                        map[y, x] += 2;
                        map[y + upOrDown, x] += 1;
                        map[y + nextBall, x] -= 3;
                    }

                    if (map[y + nextBall, x] == 1)
                    {
                        map[y, x] += 2;
                        map[y + upOrDown, x] += 1;
                        map[y + nextBall, x] += 5;
                    }
                    break;

                case 5:
                    map[y, x] += 2;
                    map[y + upOrDown, x] -= 2;
                    break;

                case 6:
                    if (map[y + nextBall, x] == 5)
                    {
                        map[y, x] += 2;
                        map[y + upOrDown, x] -= 3;
                        map[y + nextBall, x] -= 3;
                    }
                    break;

                default:
                    break;
            }

            //구멍에 빈칸이 있는 경우 다시 복구
            for (int i = 0; i < halls.Count(); ++i)
            {
                if (map[halls[i][0], halls[i][1]] == 5)
                {
                    map[halls[i][0], halls[i][1]] = 1;
                }
            }

            return map;
        }

        public static int[,] MoveLeftOrRight(int[,] map, int x, int y, int direction, List<List<int>> halls)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };

            int leftOrRIght = direction - 2;
            int nextBall = leftOrRIght * 2;

            int move = map[y, x + leftOrRIght];

            switch (move)
            {
                case 1:
                    //P 좌우 공백일 경우
                    map[y, x] += 2;
                    map[y, x + leftOrRIght] += 2;
                    break;

                case 2:
                    //P 좌우 홀일 경우
                    if (map[y, x + nextBall] == 5)
                    {
                        map[y, x] += 2;
                        map[y, x + leftOrRIght] += 1;
                        map[y, x + nextBall] -= 3;
                    }

                    if (map[y, x + nextBall] == 1)
                    {
                        map[y, x] += 2;
                        map[y, x + leftOrRIght] += 1;
                        map[y, x + nextBall] += 5;
                    }
                    break;

                case 5:
                    //P 좌우 공 있을 경우
                    map[y, x] += 2;
                    map[y, x + leftOrRIght] -= 2;
                    break;

                case 6:
                    //P 좌우 홀안에 공있는 경우
                    if (map[y, x + nextBall] == 5)
                    {
                        map[y, x] += 2;
                        map[y, x + leftOrRIght] -= 3;
                        map[y, x + nextBall] -= 3;
                    }
                    break;

                default:
                    break;
            }

            //구멍에 빈칸이 있는 경우 다시 복구
            for (int i = 0; i < halls.Count(); ++i)
            {
                if (map[halls[i][0], halls[i][1]] == 5)
                {
                    map[halls[i][0], halls[i][1]] = 1;
                }
            }

            return map;
        }
        #endregion

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
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };

            for (int i = 0; i < stage.GetLength(0); ++i)
            {
                for (int j = 0; j < stage.GetLength(1); ++j)
                {
                    Console.Write(signal[stage[i, j]]);
                }
                Console.WriteLine();
            }

        }

        public static List<List<int>> FindHallIndex(int[,] stage)
        {
            List<List<int>> halls = new List<List<int>>();

            int index = 0;
            for (int i = 0; i < stage.GetLength(0); ++i)
            {
                for (int j = 0; j < stage.GetLength(1); ++j)
                {
                    if (stage[i, j] == 1 || stage[i, j] == 6)
                    {
                        halls.Add(new List<int>());
                        halls[index].Add(i);
                        halls[index].Add(j);
                        ++index;
                    }
                }
                Console.WriteLine();
            }

            return halls;
        }

        public static bool StageClear(int[,] data, List<List<int>> halls)
        {
            bool[] finish = new bool[halls.Count()];
            for (int i = 0; i < halls.Count(); ++i)
            {
                if (data[halls[i][0], halls[i][1]] == 6)
                {
                    finish[i] = true;
                }
            }

            if (finish.Contains(false))
            {
                return false;
            }

            return true;
        }

        #endregion 

    }
}
