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
            Console.WriteLine($"----------------");

            for (int i = 3; i < 5; ++i)
            {
                Console.WriteLine($"Stage{i + 1} 시작");

                var rawData = ConvertToData(stages[i]);
                int[,] data = new int[rawData.GetLength(0), rawData.GetLength(1)];
                Array.Copy(rawData, data, rawData.GetLength(0) * rawData.GetLength(1));

                var hallsIndex = FindHallIndex(data);

                Print(data);

                bool[] end = new bool[1];
                int turns = 0;

                FunctionRecursive(data, rawData, hallsIndex, end, turns);

                if (end[0])
                {
                    break;
                }

                if (i == stages.Count - 1)
                {
                    Console.WriteLine("축하드립니다 모든 스테이지를 클리어 하셨습니다");
                }
            }
        }

        #region Function

        public static void FunctionRecursive(int[,] data, int[,] rawData, List<List<int>> hallsIndex, bool[] end, int turns)
        {
            List<char> command = new List<char>() { 'w', 'a', 's', 'd' };
            //List<string> saveCommand = new List<string>() { "1S", "2S", "3S", "4S", "5S" };
            //List<string> loadCommand = new List<string>() { "1L", "2L", "3L", "4L", "5L" };

            Console.WriteLine($"SOKOBAN>");

            string move = Console.ReadLine();

            if (move == "q")
            {
                end[0] = true;
                return;
            }

            if (move[0] == 'r')
            {
                Array.Copy(rawData, data, rawData.GetLength(0) * rawData.GetLength(1));
            }
            else if (command.Contains(move[0]))
            {
                ++turns;
                data = MovingCharacter(data, hallsIndex, move);
            }
            else
            {
                Console.WriteLine($"실행할 수 없습니다");
            }

            Print(data);
            Console.WriteLine($"턴수: {turns}");

            if (StageClear(data, hallsIndex))
            {
                Console.WriteLine($"Stage를 클리어 하셨습니다");
                return;
            }

            FunctionRecursive(data, rawData, hallsIndex, end, turns);
        }

        public static int[,] SaveAndLoad(int[,] data, string command)
        {
            const int SAVE_NUMBER = 5;

            int saveIndex = command[0];

            bool save = false;

            if (command[1] == 'S')
            {
                save = true;
            }

            List<List<List<int>>> savedData = new List<List<List<int>>>(SAVE_NUMBER);
            for (int i = 0; i < SAVE_NUMBER; ++i)
            {
                savedData.Add(new List<List<int>>());
            }

            var eachSavedData = savedData[saveIndex];

            if (save)
            {
                for (int i = 0; i < data.GetLength(0); ++i)
                {
                    eachSavedData.Add(new List<int>());
                    for (int j = 0; j < data.GetLength(1); ++j)
                    {
                        eachSavedData[i].Add(data[i, j]);
                    }
                }
            }
            else
            {
                if (eachSavedData == null)
                {
                    Console.WriteLine("데이터 없음");
                    return data;
                }

                int[,] loadedData = new int[eachSavedData.Count(), eachSavedData[0].Count()];

                for (int i = 0; i < eachSavedData.Count(); ++i)
                {
                    var eachRowData = eachSavedData[i];
                    for (int j = 0; j < eachRowData.Count(); ++j)
                    {
                        loadedData[i, j] = eachRowData[j];
                    }
                }

                return loadedData;
            }

            return data;
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
            //List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };
            const int PLAYER_BALL_OFFSET = 1;
            const int PLAYER_BLANK_OFFSET = 2;
            const int BALL_BLANK_OFFSET = 3;
            const int BALL_HALLINBALL_OFFSET = 5;

            int upOrDown = direction - 1;
            int nextBall = upOrDown * 2;

            int move = map[y + upOrDown, x];

            switch (move)
            {
                case 1:
                    //P 위아래 홀인 경우
                    map[y, x] += PLAYER_BLANK_OFFSET;
                    map[y + upOrDown, x] += PLAYER_BLANK_OFFSET;
                    break;

                case 2:
                    //P 위아래 공 있을 경우
                    //P - 2 가 빈공간인 경우
                    if (map[y + nextBall, x] == 5)
                    {
                        map[y, x] += PLAYER_BLANK_OFFSET;
                        map[y + upOrDown, x] += PLAYER_BALL_OFFSET;
                        map[y + nextBall, x] -= BALL_BLANK_OFFSET;
                    }
                    //P - 2 가 홀인 경우
                    if (map[y + nextBall, x] == 1)
                    {
                        map[y, x] += PLAYER_BLANK_OFFSET;
                        map[y + upOrDown, x] += PLAYER_BALL_OFFSET;
                        map[y + nextBall, x] += BALL_HALLINBALL_OFFSET;
                    }
                    break;

                case 5:
                    // P위아래 빈공간 있을 경우
                    map[y, x] += PLAYER_BLANK_OFFSET;
                    map[y + upOrDown, x] -= PLAYER_BLANK_OFFSET;
                    break;

                case 6:
                    // P위아래 홀안에 공 있는 경우
                    if (map[y + nextBall, x] == 5)
                    {
                        map[y, x] += PLAYER_BLANK_OFFSET;
                        map[y + upOrDown, x] -= BALL_BLANK_OFFSET;
                        map[y + nextBall, x] -= BALL_BLANK_OFFSET;
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
                    // 숫자 1 은 구멍을 나타냄
                    map[halls[i][0], halls[i][1]] = 1;
                }
            }

            return map;
        }

        public static int[,] MoveLeftOrRight(int[,] map, int x, int y, int direction, List<List<int>> halls)
        {
            //List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };
            const int PLAYER_BALL_OFFSET = 1;
            const int PLAYER_BLANK_OFFSET = 2;
            const int BALL_BLANK_OFFSET = 3;
            const int BALL_HALLINBALL_OFFSET = 5;

            int leftOrRIght = direction - 2;
            int nextBall = leftOrRIght * 2;

            int move = map[y, x + leftOrRIght];

            switch (move)
            {
                case 1:
                    //P 좌우 홀일 경우
                    map[y, x] += PLAYER_BLANK_OFFSET;
                    map[y, x + leftOrRIght] += PLAYER_BLANK_OFFSET;
                    break;

                case 2:
                    //P 좌우 공 있을 경우
                    //P - 2 가 빈공간인 경우
                    if (map[y, x + nextBall] == 5)
                    {
                        map[y, x] += PLAYER_BLANK_OFFSET;
                        map[y, x + leftOrRIght] += PLAYER_BALL_OFFSET;
                        map[y, x + nextBall] -= BALL_BLANK_OFFSET;
                    }

                    //P - 2 가 홀인 경우
                    if (map[y, x + nextBall] == 1)
                    {
                        map[y, x] += PLAYER_BLANK_OFFSET;
                        map[y, x + leftOrRIght] += PLAYER_BALL_OFFSET;
                        map[y, x + nextBall] += BALL_HALLINBALL_OFFSET;
                    }
                    break;

                case 5:
                    //P 좌우 빈공간 경우
                    map[y, x] += PLAYER_BLANK_OFFSET;
                    map[y, x + leftOrRIght] -= PLAYER_BLANK_OFFSET;
                    break;

                case 6:
                    //P 좌우 홀안에 공있는 경우
                    if (map[y, x + nextBall] == 5)
                    {
                        map[y, x] += PLAYER_BLANK_OFFSET;
                        map[y, x + leftOrRIght] -= BALL_BLANK_OFFSET;
                        map[y, x + nextBall] -= BALL_BLANK_OFFSET;
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

        public static string RightLastString(string Row, int length)
        {
            const char PADDING = ' ';

            StringBuilder addPadding = new StringBuilder(length);

            addPadding.Append(Row);
            int loop = length - Row.Length;

            for (int i = 0; i < loop; ++i)
            {
                addPadding.Append(PADDING);
            }
            Row = addPadding.ToString();

            return Row;
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
