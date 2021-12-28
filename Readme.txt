# 소코반 게임 구현하기

소코반 게임을 크게 3단계로 구현하였습니다.

1. txt파일에서 스테이지 읽어오기
2. Player를 다른 자료와 상호작용 하기
3. 실행 

## 소코반 게임 가정

1. move시에는 입력값이 ddd와 같이 긴 값이 아닌 오직 d 하나의 값만 들어간다고 가정하였습니다.
2. stage데이터는 존재한다고 가정하였습니다.
3. 공이 들어간 홀은 '@' 표시로 대체하였습니다.

## txt파일에서 스테이지 읽어오기

다음 코드는 path에 저장된 txt파일을 읽어와서 stage 별로 나누어 주는 역할을 하는 함수입니다. 

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
        
1. 만약 path에 파일이 존재 하지 않는다면 스테이지를 불러오지 않고 멈추게 됩니다.

## 읽어온 스테이지를 자료형으로 변환하기

1. 앞서 지도데이터 읽기와 플레이어 움직이기와 마찬가지로 읽어온 string데이터를 int 형 자료로 바꾸어 2차원 배열에 저장해 주었습니다.

        public static int[,] ConvertToData(string stages)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };

            string[] mapDataSplit = stages.Split("\r\n");
            string[] mapData = new string[mapDataSplit.Length - 1];
            Array.Copy(mapDataSplit, 1, mapData, 0, mapData.Length);

            int row = mapData.Length;
            int col = mapData[1].Length;
              
              
            // 이 부분은 string 자료를 split하는 과정에서 사라진 공백을 다시 메꾸어 자료형의 순서를 맞추기 위해서 
            공백을 추가해 주는 부분입니다.
            
            for (int i = 0; i < row; ++i)
            {
                if (mapData[i].Length != col)
                {
                    mapData[i] = RightLastString(mapData[i], col);
                }
            }

            //2차원 배열 에 자료형으로 대입해 주었습니다.
            
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
        
        
 ## Player 움직이기
 
 1. 플레이어를 움직이기 위해서 크게 2단계로 구성하였습니다. 
 
 ## 좌우 혹은 위아래 값인지 확인 단계
 
       public static int[,] MovingCharacter(int[,] map, List<List<int>> halls, string move)
        {
            List<char> signal = new List<char>() { '#', 'O', 'o', 'P', '=', ' ', '@' };
            List<char> moveCom = new List<char>() { 'w', 'a', 's', 'd', 'q' };

            //플레이어의 좌표를 찾습니다.
            var pIndex = FindUserIndex(map);
            
            int y = pIndex[0];
            int x = pIndex[1];

            int moveIndex = moveCom.IndexOf(move[0]);
            int[,] resultData = new int[map.GetLength(0), map.GetLength(1)];
              
            // 플레이어가 좌우 혹은 위아래로 움직임을 입력했을때 각각 에 맞는 함수가 실행됩니다.  
            switch (moveIndex)
            {
                // 위아래 움직임
                case 0:
                case 2:
                    resultData = MoveUpOrDown(map, x, y, moveIndex, halls);
                    break;
                    
                // 좌우 움직임
                case 1:
                case 3:
                    resultData = MoveLeftOrRight(map, x, y, moveIndex, halls);
                    break;


                default:
                    break;
            }

            return resultData;
        }
        
## 플레이어 및 다른 사물과 상호작용

다른 사물과 상호작용 하기 위해서 어떤 사물과 만나는지에 따라 다른 연산을 할 수 있도록 구성하였습니다.

1. 각각의 경우들을 모두 나누어 주었습니다.
2. 플레이어가 빈칸을 만나는 경우
3. 플레이어가 홀을 만나는 경우
4. 플레이어가 공을 움직이는 경우
5. 플레이어가 홀안에 있는 공을 만나는 경우 
6. 총 4가지로 구성하였습니다.
7. 플레이어가 구멍을 지나가는 경우에 다시 빈칸으로 처리되었기 때문에 빈칸의 좌표를 항상 기억하여 그 공간이 빈칸인 경우에 다시 홀 자료형으로 바꿀 수 있도록 해 주었습니다.

(좌우 움직임도 위아래 움직임 함수와 비슷하기에 위아래 움직임 함수만 작성하였습니다)

       public static int[,] MoveUpOrDown(int[,] map, int x, int y, int direction, List<List<int>> halls)
        {
            //사물 자료형의 값은 signal index와 같습니다.
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
 
 ## 실행 함수 #1
 
 위의 두 함수를 실행하는 함수를 작성하였습니다.
 아래는 입력부분이 아닌 자료 불러오기 및 자료형을 입력값 실행함수로 넘겨주는 역할을 합니다.
 
       public static void StartGame()
        {
            const string STAGE_FILE_NAME = "StageFile.txt";
            List<string> stages = new List<string>();
              
            // txt파일에서 stage 정보를 불러옵니다.
            
            LoadStages(stages, STAGE_FILE_NAME);

            Console.WriteLine($"소코반 게임 시작");
              
              
            // 불러온 stage 수 만큼 실행 시킵니다.
            
            for (int i = 0; i < stages.Count(); ++i)
            {
                Console.WriteLine($"Stage{i + 1} 시작");
                
                
                // r 을 입력시에 stage 초기화를 하기 위해서 rawData를 생성해 주었습니다. 
                
                var rawData = ConvertToData(stages[i]);
                int[,] data = new int[rawData.GetLength(0), rawData.GetLength(1)];
                Array.Copy(rawData, data, rawData.GetLength(0) * rawData.GetLength(1));
                
                
                //홀의 위치를 파악하기 위해서 홀 좌표 찾는 함수를 호출하였습니다.
                
                var hallsIndex = FindHallIndex(data);
                
                
                // 스테이지를 먼저 보여줍니다.
                
                Print(data);
                     
                     
                // 'q'를 누를시에 for문을 탈출하기 위한 장치로 bool 배열을 선언하였습니다. 
                // 참조형으로 선언하여 end가 바뀔시에 바로 탈출 할 수 있도록 하였습니다.
                
                bool[] end = new bool[1];

                FunctionRecursive(data, rawData, hallsIndex, end);

                if (end[0])
                {
                    break;
                }
                
                
                //모든 스테이지 클리어시 나오는 문구입니다.
                
                if (i == stages.Count - 1)
                {
                    Console.WriteLine("축하드립니다 모든 스테이지를 클리어 하셨습니다");
                }
            }
        }
 
 ## 실행함수 2 - 입력값을 통한 실행
 
 입력값을 통해서 게임이 끝날 수 있을때까지 재귀를 돌도록 구성하였으며 'q'나 게임 클리어를 하게 되면 재귀문을 빠져나갈 수 있도록 하였습니다.
 
         public static void FunctionRecursive(int[,] data, int[,] rawData, List<List<int>> hallsIndex, bool[] end, int turns)
        {
            List<char> command = new List<char>() { 'w', 'a', 's', 'd' };
            //List<string> saveCommand = new List<string>() { "1S", "2S", "3S", "4S", "5S" };
            //List<string> loadCommand = new List<string>() { "1L", "2L", "3L", "4L", "5L" };

            Console.WriteLine($"SOKOBAN>");

            string move = Console.ReadLine();
            
            // q 를 입력받았을때 프로그램을 종료합니다.
            if (move == "q")
            {
                end[0] = true;
                return;
            }
              
            //r 을 입력하였을때 rawdata로 돌아갑니다. rawData에서 array copy를 통해 rawdata를 바꾸지 않습니다.
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
              
            // 입력한 값을 통해 움직인 자료를 다시 화면에 string으로 보여줍니다.
            Print(data);
            Console.WriteLine($"턴수: {turns}");

            if (StageClear(data, hallsIndex))
            {
                Console.WriteLine($"Stage를 클리어 하셨습니다");
                return;
            }
            
            //재귀를 통해 게임이 끝날때까지 돌도록 하였습니다.
            FunctionRecursive(data, rawData, hallsIndex, end, turns);
        }
 
 ## 도우미 함수들
 
 위 함수들을 돕는 도우미 함수로는 
 
 1. stage의 짤린 공백을 추가해주는 함수

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
        

 3. player의 좌표를 반환하는 함수

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
        
        
 5. 자료형으로 저장된 2차원 배열을 다시 string으로 표시해주는 함수

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
       
       
 7. hall 의 위치들을 반환하는 함수
       
    홀의 위치들을 list in list에 각각 좌표형식으로 저장하였습니다.
       
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
       
       
 8. 모든 hall에 공이 올라가있는지 확인하는 함수
       
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

총 5가지를 구현하였습니다. 

## 결과값 출력 

![이미지1](https://user-images.githubusercontent.com/83396157/145051341-3c5caef4-24e4-416d-9fe3-5fd93fe5c1a0.PNG)
![이미지2](https://user-images.githubusercontent.com/83396157/145051347-a52d5175-4e67-45a2-a997-947299a480ee.PNG)
![이미지3](https://user-images.githubusercontent.com/83396157/145051349-57f52ebd-af21-4092-bf91-6ff32876cb10.PNG)

