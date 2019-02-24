using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Manybits
{
    public enum GamesWindow { GW_NONE, GW_GAME, GW_GAME_OVER, GW_SETTINGS, GW_BRIEF };
    public enum GameState { GS_TURN, GS_CHIP_MOVE, GS_CHIP_MERGE, GS_CHIP_FALL, GS_GAME_OVER };

    public class GameProcess : MonoBehaviour
    {
        public GameManager gameManager;
        public CameraManager cameraManager;
        public ScreenManager screenManager;

        public AdsController adsController;
        public GpgsController gpgsController;

        public GamesWindow gamesWindow = GamesWindow.GW_NONE;
        public GameState gameState;

        private List<Back> backs = new List<Back>();
        private List<GameObject> points = new List<GameObject>();
        private List<GameObject> boardBacks = new List<GameObject>();
        private Stack<Chip> chipBuffer = new Stack<Chip>();
        private List<Chip> chips = new List<Chip>();
        private GameField field;
        public ChipSprites chipSprites;

        public GameObject selectBorder;

        public Transform chipBufferHolder;
        public Transform chipHolder;
        public Transform backHolder;
        public Transform boardBackHolder;
        public Transform pointHolder;

        public GameObject chipPref;
        public GameObject backPref;
        public GameObject boardBackPref;
        public GameObject pointPref;

        public Transform touchAreaCollider;

        private List<Chip> fallingChips = new List<Chip>();
        private List<ChipMatch> matches = new List<ChipMatch>();
        private List<Vector2> availableCells = new List<Vector2>();
        private List<Vector2> path = new List<Vector2>();
        private List<Vector2> generatedChips = new List<Vector2>();
        private bool isChipSelect;
        private Vector2 selectedChip;
        private int[] chipScores = { 0, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576 };

        private List<Chip> chipsForDelete = new List<Chip>();
        private Chip movingChip;
        private bool movingChipIsOnPlace;
        private Vector2 newFieldPos;

        private float cellHeight;
        private float chipSpeed;

        private int chipGenerateCount;
        private int startChipCount;

        private const int CHIP_SPEED = 1200;  //1200        
        private float MATCH_SCORE_COEF = 2.0f;
        private float ADVERT_MAX_TIME = 120.0f;
        private const float DELAY_TIME = 0.3f;

        private const int SCORE_ACHIEVEMENT_1 = 10000;
        private const int SCORE_ACHIEVEMENT_2 = 50000;
        private const int SCORE_ACHIEVEMENT_3 = 100000;

        private bool isLastTry = false;

        private Rect touchArea;
        private Rect fieldSpace; // Свободная область, в которой можно разместить поле
        private Vector2 chipsPoint;

        private int score = 0;
        private int stepsCount = 0;
        private bool addScore = true;
        private int winScore;

        private int spotsCount;
        private int winSpotsCount;

        private bool needSave = false;

        private bool isGameOver = false;
        private bool isWin = false;

        private int cachedLevel;
        private LevelType levelType;

        private float spriteWidth;
        private float spriteScale;
        private float backWidth;
        private float backScale;

        private string fileNameInfinity = "Infinity.bin";



        // Use this for initialization
        void Start()
        {
            spriteWidth = chipSprites.sprites[0].rect.width;
            backWidth = chipSprites.back.rect.width;
        }



        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;

            if (gamesWindow == GamesWindow.GW_GAME)
            {
                for (int i = 0; i < chips.Count; i++)
                {
                    chips[i].Update(delta);
                    if (chips[i].needToDelete)
                    {
                        chipsForDelete.Add(chips[i]);
                    }
                }

                if (gamesWindow == GamesWindow.GW_GAME)
                {
                    if (gameManager.advertTime > 0)
                    {
                        gameManager.advertTime -= delta;
                    }
                    else
                    {
                        ShowInterstitial();
                    }
                }

                if (chipsForDelete.Count > 0)
                {
                    for (int i = 0; i < chipsForDelete.Count; i++)
                    {
                        RemoveChip(chipsForDelete[i]);
                    }
                    chipsForDelete.Clear();
                }

                if (gameState == GameState.GS_CHIP_FALL)
                {
                    FallChips(delta);
                }

                // Движение фишки после хода игрока
                if (gameState == GameState.GS_CHIP_MOVE)
                {
                    MoveChips(delta);
                }

                if (gameState == GameState.GS_CHIP_MERGE)
                {
                    MergeChips(delta);
                }

                if (gameManager.gameMode == GameMode.GM_LEVEL)
                {
                    if (isWin)
                    {
                        needSave = true;

                        int nextLevel = gameManager.currentLevel + 1;
                        Level level = gameManager.levels[gameManager.currentLevel];
                        if (level.access == LevelAccess.LP_COMPLETED)
                        {
                            if (stepsCount < level.steps)
                                level.steps = stepsCount;
                        }
                        else
                        {
                            level.steps = stepsCount;
                        }
                        level.access = LevelAccess.LP_COMPLETED;

                        int curLevel = gameManager.currentLevel + 1;
                        if (curLevel == 10)
                            gpgsController.UnlockAchievement(GPGSIds.achievement_newbie);
                        if (curLevel == 30)
                            gpgsController.UnlockAchievement(GPGSIds.achievement_advanced);
                        if (curLevel == 60)
                            gpgsController.UnlockAchievement(GPGSIds.achievement_master);
                        if (curLevel == 90)
                            gpgsController.UnlockAchievement(GPGSIds.achievement_genius);
                        if (curLevel == 120)
                            gpgsController.UnlockAchievement(GPGSIds.achievement_the_overmind);

                        SetGameState(GameState.GS_GAME_OVER);
                        gamesWindow = GamesWindow.GW_GAME_OVER;

                        if (nextLevel >= gameManager.lastLevel)
                        {
                            StartCoroutine(OpenLastWinWindow());
                        }
                        else
                        {
                            if (gameManager.levels[nextLevel].access == LevelAccess.LP_LOCKED)
                                gameManager.levels[nextLevel].access = LevelAccess.LP_UNLOCKED;
                            StartCoroutine(OpenWinWindow());
                        }

                        gameManager.SaveProgress();
                        gameManager.SaveHighscore();
                    }
                }

                // ~~~saveGame
                if (needSave)
                {
                    if (gameManager.gameMode == GameMode.GM_INFINITY)
                    {
                        gameManager.continueInfinityGame = !isGameOver;
                        if (score > gameManager.highScore)
                            gameManager.highScore = score;
                        gameManager.SaveHighscore();
                        SaveGame();
                    }
                    needSave = false;
                }
            }
        }



        private void FallChips(float delta)
        {
            bool f = true;
            for (int i = 0; i < fallingChips.Count; i++)
            {
                if (fallingChips[i].isAnim)
                {
                    f = false;
                    break;
                }
            }
            if (f)
            {
                int sumScore = 0;
                for (int i = 0; i < fallingChips.Count; i++)
                {
                    fallingChips[i].SetSpeed(chipSpeed, chipSpeed);
                    sumScore += fallingChips[i].chipValue;
                }
                AddScore(sumScore);

                chips.AddRange(fallingChips);
                fallingChips.Clear();

                Vector2[] points = new Vector2[generatedChips.Count];
                points = generatedChips.ToArray();
                Match[] matchesArr = field.GetMatches(points);
                if (matchesArr != null)
                {
                    for (int i = 0; i < matchesArr.Length; i++)
                    {
                        AddMatch(matchesArr[i], 1);
                    }
                }

                if (matches.Count > 0)
                {
                    SetGameState(GameState.GS_CHIP_MERGE);
                }
                else
                {
                    if (field.GetEmptyCellCount() == 0)
                    {
                        SetGameState(GameState.GS_GAME_OVER);
                        gamesWindow = GamesWindow.GW_GAME_OVER;

                        Debug.Log("     End Level");

                        if (gameManager.gameMode == GameMode.GM_INFINITY)
                        {
                            StartCoroutine(OpenGameOverWindow());
                        }
                        else
                        {
                            StartCoroutine(OpenLoseWindow());
                        }
                    }
                    else
                    {
                        SetGameState(GameState.GS_TURN);
                        addScore = true;
                    }
                    needSave = true;
                }
            }
        }



        private void MoveChips(float delta)
        {
            if (movingChipIsOnPlace)
            {
                movingChipIsOnPlace = false;
                int fX = (int)movingChip.fieldPos.x;
                int fY = (int)movingChip.fieldPos.y;
                field.cells[((int)newFieldPos.y), ((int)newFieldPos.x)] = field.cells[fY, fX];
                field.cells[fY, fX] = 0;
                movingChip.SetFieldPos(newFieldPos);

                // Проверка, есть ли совпадение
                // Если есть, то запустить процесс слияния фишек
                // Если нет, то сгенерировать новые фишки
                Match match = field.GetMatch(newFieldPos);
                if (match != null)
                {
                    AddMatch(match, 1);
                    addScore = true;
                    SetGameState(GameState.GS_CHIP_MERGE);
                }
                else
                {
                    addScore = false;
                    // генерация
                    int genCount = chipGenerateCount;
                    int emptyCellCount = field.GetEmptyCellCount();
                    if (isLastTry)
                    {
                        if (emptyCellCount <= chipGenerateCount)
                        {
                            genCount = emptyCellCount;
                        }
                        else
                        {
                            genCount = chipGenerateCount;
                            isLastTry = false;
                        }
                    }
                    else
                    {
                        if (emptyCellCount <= chipGenerateCount)
                        {
                            if (emptyCellCount > 1)
                            {
                                genCount = emptyCellCount - 1;
                            }
                            else
                            {
                                genCount = 1;
                            }
                            isLastTry = true;
                        }
                    }

                    Vector2[] points = field.Generate(genCount, false);
                    generatedChips.Clear();
                    generatedChips.AddRange(points);
                    fallingChips.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                        int x = (int)points[i].x;
                        int y = (int)points[i].y;
                        int chipScore = chipScores[field.cells[y, x]];
                        Chip chip = GetNewChip(new Vector2(x, y), chipScore, chipSprites.sprites[field.cells[y, x] - 1]);
                        fallingChips.Add(chip);
                        chip.PlayAnimation("ChipCreate");
                    }

                    SetGameState(GameState.GS_CHIP_FALL);
                }

                stepsCount++;
                screenManager.gameWindow.SetTextSteps(stepsCount);
            }
            else
            {
                if (movingChip.isInDestination)
                {
                    movingChipIsOnPlace = true;
                    movingChip.isInDestination = false;
                }
            }
        }



        private void MergeChips(float delta)
        {
            if (matches.Count > 0)
            {
                for (int k = 0; k < matches.Count;)
                {
                    ChipMatch chipMatch = matches[k];
                    if (!chipMatch.isMove)
                    {
                        int mX;
                        int mY;
                        if (chipMatch.matchChips.Count == 0)
                        {
                            mX = (int)chipMatch.mainPoint.x;
                            mY = (int)chipMatch.mainPoint.y;
                            field.cells[mY, mX] = chipMatch.value + 1;
                            Chip mainChip = FindChip(mX, mY);
                            int chipScore = chipScores[field.cells[mY, mX]];
                            mainChip.chipValue = chipScore;
                            mainChip.SetSprite(chipSprites.sprites[field.cells[mY, mX] - 1]);

                            int newChip = field.cells[mY, mX];
                            if (newChip == 5)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_5);
                            if (newChip == 6)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_6);
                            if (newChip == 7)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_7);
                            if (newChip == 8)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_8);
                            if (newChip == 9)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_9);
                            if (newChip == 10)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_10);
                            if (newChip == 11)
                                gpgsController.UnlockAchievement(GPGSIds.achievement_tile_11);

                            //~addscore
                            // Добавляются очки за матч
                            if (addScore)
                            {
                                int matchScore = GetMatchScore(chipMatch.count, chipMatch.chipScore);
                                AddScore(matchScore * chipMatch.multiMergeBonus);

                                // Очистка грязи происходит только после хода игрока
                                // После генерации такого произойти не может
                                if (gameManager.gameMode == GameMode.GM_LEVEL)
                                {
                                    if (levelType == LevelType.LT_DIRT)
                                    {
                                        if (field.back[mY, mX] > 0)
                                        {
                                            field.back[mY, mX]--;
                                            Back back = FindBack(mX, mY);
                                            if (field.back[mY, mX] > 0)
                                            {
                                                back.SetNumber(field.back[mY, mX]);
                                            }
                                            else
                                            {
                                                back.gameObject.SetActive(false);
                                            }
                                            spotsCount--;
                                            if (spotsCount < 0)
                                                spotsCount = 0;
                                            screenManager.gameWindow.SetTextProgress(spotsCount);
                                        }
                                    }
                                    else if (levelType == LevelType.LT_NUMDIRT)
                                    {
                                        if (field.back[mY, mX] > 0 && field.cells[mY, mX] == field.back[mY, mX])
                                        {
                                            field.back[mY, mX] = 0;
                                            Back back = FindBack(mX, mY);
                                            back.gameObject.SetActive(false);
                                            spotsCount--;
                                            if (spotsCount < 0)
                                                spotsCount = 0;
                                            screenManager.gameWindow.SetTextProgress(spotsCount);
                                        }
                                    }
                                }

                                if (levelType == LevelType.LT_DIRT || levelType == LevelType.LT_NUMDIRT)
                                {
                                    bool isZeroCount = true;
                                    for (int i = 0; i < field.height; i++)
                                    {
                                        for (int j = 0; j < field.width; j++)
                                        {
                                            if (field.back[i, j] != 0)
                                            {
                                                isZeroCount = false;
                                                break;
                                            }
                                        }
                                        if (!isZeroCount)
                                            break;
                                    }

                                    if (isZeroCount)
                                    {
                                        isWin = true;
                                    }
                                }
                            }

                            mainChip.PlayAnimation("ChipUpgrade");

                            Match match = field.GetMatch(new Vector2(mX, mY));
                            if (match != null)
                            {
                                AddMatch(match, chipMatch.multiMergeBonus + 1);
                            }

                            matches.Remove(chipMatch);

                            continue;
                        }
                        // Находит крайние фишки и передвигает их на соседние
                        mX = (int)chipMatch.mainPoint.x;
                        mY = (int)chipMatch.mainPoint.y;
                        bool isExtr = false;
                        for (int i = 0; i < chipMatch.matchChips.Count;)
                        {
                            Chip chip = chipMatch.matchChips[i];
                            Vector2 target = new Vector2();
                            int count = 0;
                            int aX = (int)chip.fieldPos.x;
                            int aY = (int)chip.fieldPos.y;
                            int dX = mX - aX;
                            int dY = mY - aY;
                            if (Mathf.Abs(dX) + Mathf.Abs(dY) == 1)
                            {
                                count++;
                                target = chipMatch.mainPoint;
                            }
                            for (int j = 0; j < chipMatch.matchChips.Count; j++)
                            {
                                Chip chip2 = chipMatch.matchChips[j];
                                if (chip == chip2)
                                    continue;
                                int cX = (int)chip2.fieldPos.x;
                                int cY = (int)chip2.fieldPos.y;
                                dX = cX - aX;
                                dY = cY - aY;
                                if (Mathf.Abs(dX) + Mathf.Abs(dY) == 1)
                                {
                                    count++;
                                    target = chip2.fieldPos;
                                }
                            }
                            if (count == 1)
                            {
                                isExtr = true;
                                chip.MoveTo(CellPointToPoint(target));
                                chipMatch.matchChips.Remove(chip);
                                chipMatch.isMove = true;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (!isExtr)
                        {
                            // Если крайних фишек нет
                            for (int i = chipMatch.matchChips.Count - 1; i > 0; i--)
                            {
                                Chip chip1 = chipMatch.matchChips[i];
                                Chip chip2 = chipMatch.matchChips[i - 1];
                                int aX = (int)chip1.fieldPos.x;
                                int aY = (int)chip1.fieldPos.y;
                                int cX = (int)chip2.fieldPos.x;
                                int cY = (int)chip2.fieldPos.y;
                                int dX = cX - aX;
                                int dY = cY - aY;
                                if (Mathf.Abs(dX) + Mathf.Abs(dY) == 1)
                                {
                                    chip1.MoveTo(CellPointToPoint(new Vector2(cX, cY)));
                                    chipMatch.matchChips.Remove(chip1);
                                    chipMatch.isMove = true;
                                    break;
                                }
                            }
                        }
                    }
                    k++;
                }
            }
            else
            {
                needSave = true;
                SetGameState(GameState.GS_TURN);
            }
        }



        private int GetMatchScore(int count, int chipScore)
        {
            if (count < 3)
                return 0;
            float matchScore = 0;
            if (count > 3)
            {
                matchScore = chipScore * count * (count - 3) * MATCH_SCORE_COEF;
            }
            else
                matchScore = chipScore * 3;
            return (int)matchScore;
        }



        public void MouseDown()
        {
            if (gamesWindow == GamesWindow.GW_GAME && gameState == GameState.GS_TURN)
            {
                Vector2 mousePos = Input.mousePosition;
                mousePos *= cameraManager.ScaleCoef;
                mousePos.Set(mousePos.x - cameraManager.ScreenWidth / 2, mousePos.y - cameraManager.ScreenHeight / 2);
                int x = (int)((mousePos.x - touchArea.xMin) / cellHeight);
                int y = (int)((touchArea.yMax - mousePos.y) / cellHeight);
                x = x < 0 ? 0 : x;
                x = x >= field.width ? field.width - 1 : x;
                y = y < 0 ? 0 : y;
                y = y >= field.height ? field.height - 1 : y;

                Vector2 v = new Vector2(x, y);

                // Выбор фишки
                if (isChipSelect)
                {
                    if (!field.IsEmptyCell(v))
                    {
                        // выбрать другую фишку
                        if (v != selectedChip)
                        {
                            availableCells.Clear();
                            Vector2[] points = field.GetAvailableCells(v);
                            availableCells.AddRange(points);
                        }
                        selectedChip = v;
                        isChipSelect = true;
                        selectBorder.SetActive(true);
                        selectBorder.transform.position = CellPointToPoint(v);
                    }
                    else
                    {
                        if (availableCells.Contains(v))
                        {
                            // переместить фишку в выбранную позицию
                            path.Clear();
                            Vector2[] points = field.GetPath(selectedChip, v);
                            path.AddRange(points);
                            for (int i = 0; i < points.Length; i++)
                                points[i] = CellPointToPoint(points[i]);

                            availableCells.Clear();
                            isChipSelect = false;

                            Chip chip = FindChip((int)selectedChip.x, (int)selectedChip.y);
                            if (chip != null)
                            {
                                chip.FollowPath(points);
                                movingChip = chip;
                                movingChipIsOnPlace = false;
                                newFieldPos = v;
                                SetGameState(GameState.GS_CHIP_MOVE);
                            }

                            selectBorder.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (!field.IsEmptyCell(v))
                    {
                        // выбрать фишку
                        availableCells.Clear();
                        Vector2[] points = field.GetAvailableCells(v);
                        availableCells.AddRange(points);
                        selectedChip = v;
                        isChipSelect = true;
                        selectBorder.SetActive(true);
                        selectBorder.transform.position = CellPointToPoint(v);
                    }
                }

                for (int i = 0; i < points.Count; i++)
                {
                    points[i].SetActive(false);
                }
                if (isChipSelect)
                {
                    for (int i = 0; i < availableCells.Count; i++)
                    {
                        points[i].SetActive(true);
                        points[i].transform.position = CellPointToPoint(availableCells[i]);
                    }
                }
            }
        }



        public void DrawRect(Rect r, Color c)
        {
            Debug.DrawLine(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), c);
            Debug.DrawLine(new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), c);
            Debug.DrawLine(new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax), c);
            Debug.DrawLine(new Vector2(r.xMin, r.yMax), new Vector2(r.xMin, r.yMin), c);
        }

        public void DrawPoint(Vector2 v, Color c)
        {
            Vector2 vec = CellPointToPoint(v);
            Rect r = new Rect(vec.x - 10, vec.y - 10, 20, 20);
            DrawRect(r, c);
        }

        public void DrawWeb(Rect r, int fieldWidth, int fieldHeight, Color c)
        {
            float ch = r.width / fieldWidth;

            for (int i = 1; i < fieldWidth; i++)
                Debug.DrawLine(new Vector2(r.xMin + ch * i, r.yMax), new Vector2(r.xMin + ch * i, r.yMin), c);
            for (int i = 1; i < fieldHeight; i++)
                Debug.DrawLine(new Vector2(r.xMin, r.yMax - ch * i), new Vector2(r.xMax, r.yMax - ch * i), c);
        }



        public Rect NewRect(float x, float y, float width, float height)
        {
            return new Rect(x, y - height, width, height);
        }



        private void PrepareStage()
        {
            float width = cameraManager.ScreenWidth - 40;
            float height = cameraManager.ScreenHeight - 200;
            Vector2 screenTL = new Vector2(-cameraManager.ScreenWidth / 2, cameraManager.ScreenHeight / 2);

            fieldSpace = NewRect(screenTL.x + 20, screenTL.y - 180, width, height);

            float cellW = fieldSpace.width / field.width;
            float cellH = fieldSpace.height / field.height;

            cellHeight = Mathf.Min(cellW, cellH);

            spriteScale = cellHeight / spriteWidth * 0.8f;
            backScale = cellHeight / backWidth * 0.9f;

            float scale = cellHeight / spriteWidth;
            chipSpeed = scale * CHIP_SPEED;

            selectBorder.transform.localScale = new Vector3(backScale, backScale, 1);

            width = field.width * cellHeight;
            height = field.height * cellHeight;

            touchArea = NewRect(fieldSpace.xMin + (fieldSpace.width - width) / 2, fieldSpace.yMax - (fieldSpace.height - height) / 2, width, height);
            chipsPoint.Set(touchArea.xMin + cellHeight / 2, touchArea.yMax - cellHeight / 2);

            screenManager.gameWindow.SetTextScore(0);
            screenManager.gameWindow.SetTextHighscore(gameManager.highScore);

            if (boardBacks.Count != 0)
            {
                for (int i = 0; i < boardBacks.Count; i++)
                    Destroy(boardBacks[i]);
                boardBacks.Clear();
            }
            for (int x = 0; x < field.width; x++)
                for (int y = 0; y < field.height; y++)
                {
                    GameObject boardBackObj = Instantiate(boardBackPref) as GameObject;
                    boardBackObj.transform.position = CellPointToPoint(new Vector2(x, y));
                    boardBackObj.transform.SetParent(boardBackHolder, false);
                    boardBackObj.transform.localScale = new Vector3(backScale, backScale, 1);
                    boardBacks.Add(boardBackObj);
                }

            if (points.Count != 0)
            {
                for (int i = 0; i < points.Count; i++)
                    Destroy(points[i]);
                points.Clear();
            }
            int pointsCount = field.width * field.height;
            for (int i = 0; i < pointsCount; i++)
            {
                GameObject pointObj = Instantiate(pointPref) as GameObject;
                pointObj.transform.SetParent(pointHolder, false);
                pointObj.transform.localScale = new Vector3(spriteScale, spriteScale, 1);
                pointObj.SetActive(false);
                points.Add(pointObj);
            }

            if (backs.Count != 0)
            {
                for (int i = 0; i < backs.Count; i++)
                    Destroy(backs[i].gameObject);
                backs.Clear();
            }
            if (levelType == LevelType.LT_DIRT || levelType == LevelType.LT_NUMDIRT)
            {
                for (int i = 0; i < field.height; i++)
                    for (int j = 0; j < field.width; j++)
                        if (field.back[i, j] > 0)
                        {
                            GameObject backObj = Instantiate(backPref) as GameObject;
                            Back back = backObj.GetComponent<Back>();
                            if (levelType == LevelType.LT_DIRT)
                                back.backType = BackType.BT_DIRT;
                            else
                                back.backType = BackType.BT_NUMDIRT;
                            back.SetNumber(field.back[i, j]);
                            back.fieldPos = new Vector2(j, i);
                            backObj.transform.position = CellPointToPoint(new Vector2(j, i));
                            backObj.transform.SetParent(backHolder, false);
                            backObj.transform.localScale = new Vector3(backScale, backScale, 1);
                            backs.Add(back);
                        }
            }

            touchAreaCollider.position = new Vector3(touchArea.x + touchArea.width / 2, touchArea.y + touchArea.height / 2, 0);
            touchAreaCollider.localScale = new Vector3(touchArea.width, touchArea.height, 1);

            if (fallingChips.Count != 0)
            {
                for (int i = 0; i < fallingChips.Count; i++)
                {
                    Chip chip = fallingChips[i];
                    Destroy(chip.gameObject);
                }
                fallingChips.Clear();
            }

            ClearList();
            if (chipBuffer.Count != 0)
            {
                Chip[] chipsArray = chipBuffer.ToArray();
                for (int i = 0; i < chipsArray.Length; i++)
                {
                    Chip chip = chipsArray[i];
                    Destroy(chip.gameObject);
                }
                chipBuffer.Clear();
            }

            {
                int newChipsCount = field.width * field.height;

                for (int i = 0; i < newChipsCount; i++)
                {
                    GameObject chipObj = Instantiate(chipPref, chipBufferHolder, false) as GameObject;
                    Chip chip = chipObj.GetComponent<Chip>();
                    chip.transform.localScale = new Vector3(spriteScale, spriteScale, 1);
                    chip.gameObject.SetActive(false);
                    
                    chipBuffer.Push(chip);
                }
            }
        }


        public void StartGame()
        {
            gameManager.advertTime = 60;

            if (gameManager.gameMode == GameMode.GM_INFINITY)
            {
                if (gameManager.continueInfinityGame)
                {
                    if (!LoadGame())
                    {
                        NewGame();
                    }
                }
                else
                {
                    NewGame();
                }
            }
            else
            {
                NewGame();
            }
        }



        public void NewGame()
        {
            Debug.Log("[GameProcess] New Game");

            field = new GameField();

            if (gameManager.gameMode == GameMode.GM_INFINITY)
            {
                field.StartInfinityGame(5, 5);
                chipGenerateCount = 8;
                startChipCount = 15;
            }
            else
            {
                Debug.Log("[GameProcess][New Game] Start Level " + gameManager.currentLevel);

                field.StartLevel(gameManager.levels[gameManager.currentLevel]);
                Level level = gameManager.levels[gameManager.currentLevel];
                levelType = level.type;

                Debug.Log("[GameProcess][New Game] levelType = " + levelType);

                spotsCount = 0;
                if (levelType == LevelType.LT_DIRT)
                {
                    for (int i = 0; i < field.height; i++)
                        for (int j = 0; j < field.width; j++)
                            spotsCount += field.back[i, j];
                    winSpotsCount = spotsCount;

                    screenManager.gameWindow.SetTextProgress(spotsCount);

                    Debug.Log("[GameProcess][New Game] spotsCount = " + spotsCount);
                }

                if (levelType == LevelType.LT_NUMDIRT)
                {
                    for (int i = 0; i < field.height; i++)
                        for (int j = 0; j < field.width; j++)
                            if (field.back[i, j] > 0)
                                spotsCount++;
                    winSpotsCount = spotsCount;

                    screenManager.gameWindow.SetTextProgress(spotsCount);

                    Debug.Log("[GameProcess][New Game] spotsCount = " + spotsCount);
                }

                if (levelType == LevelType.LT_SCORE)
                {
                    winScore = gameManager.levels[gameManager.currentLevel].score;

                    screenManager.gameWindow.SetTextProgress(winScore);

                    Debug.Log("[GameProcess][New Game] winScore = " + winScore);
                }

                chipGenerateCount = level.chipGen;
                startChipCount = level.chipStartGen;

                Debug.Log(level.ToString());
            }

            PrepareStage();

            gamesWindow = GamesWindow.GW_GAME;
            SetGameState(GameState.GS_CHIP_FALL);
            matches.Clear();
            //ClearList();

            selectBorder.SetActive(false);
            for (int i = 0; i < points.Count; i++)
            {
                points[i].SetActive(false);
            }

            score = 0;
            isGameOver = false;
            isWin = false;
            isLastTry = false;
            stepsCount = 0;
            isChipSelect = false;

            cachedLevel = gameManager.currentLevel;

            FirstChipsCreate();

            addScore = true;
            needSave = false;

            if (gameManager.gameMode == GameMode.GM_LEVEL)
            {
                gamesWindow = GamesWindow.GW_BRIEF;
                screenManager.levelBriefWindow.Open();
            }
        }



        public void SaveGame()
        {
            Debug.Log("[GameProcess] SaveGame");
            string path = Path.Combine(gameManager.path, fileNameInfinity);

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(field.width);
                writer.Write(field.height);
                for (int i = 0; i < field.height; i++)
                    for (int j = 0; j < field.width; j++)
                        writer.Write(field.cells[i, j]);
                writer.Write(score);
                writer.Write(isLastTry);
            }
        }



        public bool LoadGame()
        {
            Debug.Log("[GameProcess] LoadGame");
            string path = Path.Combine(gameManager.path, fileNameInfinity);

            if (File.Exists(path))
            {
                field = new GameField();

                chipGenerateCount = 8;
                startChipCount = 15;

                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    field.StartInfinityGame(width, height);
                    for (int i = 0; i < field.height; i++)
                        for (int j = 0; j < field.width; j++)
                            field.cells[i, j] = reader.ReadInt32();
                    score = reader.ReadInt32();
                    isLastTry = reader.ReadBoolean();
                }

                PrepareStage();

                gamesWindow = GamesWindow.GW_GAME;
                SetGameState(GameState.GS_TURN);
                matches.Clear();

                selectBorder.SetActive(false);
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].SetActive(false);
                }

                isGameOver = false;
                isWin = false;
                stepsCount = 0;
                isChipSelect = false;

                cachedLevel = gameManager.currentLevel;

                for (int i = 0; i < field.height; i++)
                    for (int j = 0; j < field.width; j++)
                        if (field.cells[i, j] != 0)
                        {
                            int chipScore = chipScores[field.cells[i, j]];
                            AddNewChip(new Vector2(j, i), chipScore, chipSprites.sprites[field.cells[i, j] - 1]);
                        }

                addScore = true;
                needSave = false;
                screenManager.gameWindow.SetTextScore(score);
                screenManager.gameWindow.SetTextHighscore(gameManager.highScore);

                return true;
            }
            else
            {
                return false;
            }
        }



        private void FirstChipsCreate()
        {
            Vector2[] points = field.Generate(startChipCount, true);
            generatedChips.Clear();
            generatedChips.AddRange(points);
            fallingChips.Clear();
            for (int i = 0; i < points.Length; i++)
            {
                int x = (int)points[i].x;
                int y = (int)points[i].y;
                int chipScore = chipScores[field.cells[y, x]];
                Chip chip = GetNewChip(new Vector2(x, y), chipScore, chipSprites.sprites[field.cells[y, x] - 1]);
                fallingChips.Add(chip);
                chip.PlayAnimation("ChipCreate");
            }
        }



        private void AddMatch(Match match, int bonus)
        {
            field.cells[((int)match.mainPoint.y), ((int)match.mainPoint.x)] = 0;

            for (int i = 0; i < match.points.Length; i++)
            {
                int x = (int)match.points[i].x;
                int y = (int)match.points[i].y;
                field.cells[y, x] = 0;
            }

            Chip chip = FindChip((int)match.points[0].x, (int)match.points[0].y);
            int chipScore;
            if (chip != null)
                chipScore = chip.chipValue;
            else
                chipScore = 0;

            ChipMatch newMatch = new ChipMatch(match, chipScore, bonus);

            for (int i = 0; i < match.points.Length; i++)
            {
                int x = (int)match.points[i].x;
                int y = (int)match.points[i].y;
                chip = FindChip(x, y);
                if (chip != null)
                {
                    chip.match = newMatch;
                    newMatch.matchChips.Add(chip);
                }
            }
            matches.Add(newMatch);
        }



        private void SetGameState(GameState gState)
        {
            switch (gState)
            {
                case GameState.GS_TURN:
                    addScore = true;
                    break;
                case GameState.GS_CHIP_MOVE:
                    break;
                case GameState.GS_CHIP_MERGE:
                    break;
                case GameState.GS_CHIP_FALL:
                    break;
                case GameState.GS_GAME_OVER:
                    if (gameManager.gameMode == GameMode.GM_LEVEL)
                    {
                        if (isWin)
                        {
                            Debug.Log("[GameProcess][SetGameState] Win level " + cachedLevel);
                        }
                        else
                        {
                            Debug.Log("[GameProcess][SetGameState] Lose level " + cachedLevel);
                        }
                    }
                    isGameOver = true;
                    break;
                default:
                    return;
            }
            gameState = gState;
        }



        public Vector2 CellPointToPoint(Vector2 cellPoint)
        {
            float x = cellPoint.x * cellHeight + chipsPoint.x;
            float y = chipsPoint.y - cellPoint.y * cellHeight;
            Vector2 vec = new Vector2(x, y);
            return vec;
        }



        //region Buffer_Region
        private Chip FindChip(int x, int y)
        {
            bool f = false;
            int i;
            for (i = 0; i < chips.Count; i++)
                if (chips[i].fieldPos.x == x && chips[i].fieldPos.y == y)
                {
                    f = true;
                    break;
                }
            if (f)
                return chips[i];
            else
                return null;
        }



        private void RemoveChip(Chip chip)
        {
            chipBuffer.Push(chip);
            chips.Remove(chip);
            chip.transform.SetParent(chipBufferHolder);
            chip.gameObject.SetActive(false);
        }



        private void RemoveChip(int index)
        {
            Chip chip = chips[index];
            chips.RemoveAt(index);
            chipBuffer.Push(chip);
            chip.transform.SetParent(chipBufferHolder);
            chip.gameObject.SetActive(false);
        }



        public void ClearList()
        {
            for (int i = 0; i < chips.Count; i++)
            {
                Chip chip = chips[i];
                chipBuffer.Push(chip);
                chip.transform.SetParent(chipBufferHolder, false);
                chip.gameObject.SetActive(false);
            }
            chips.Clear();
        }



        private Chip AddNewChip(Vector2 fieldPos, int chipValue, Sprite sprite)
        {
            Chip chip = GetNewChip(fieldPos, chipValue, sprite);
            chips.Add(chip);

            return chip;
        }



        private Chip GetNewChip(Vector2 fieldPos, int chipValue, Sprite sprite)
        {
            Chip chip;
            if (chipBuffer.Count != 0)
            {
                chip = chipBuffer.Pop();
                chip.transform.SetParent(chipHolder);
                chip.gameObject.SetActive(true);

                chip.isOnPath = false;
                chip.match = null;
            }
            else
            {
                GameObject chipObj = Instantiate(chipPref, chipHolder, false) as GameObject;
                chip = chipObj.GetComponent<Chip>();
            }
            chip.SetFieldPos(fieldPos);
            chip.SetSprite(sprite);
            chip.SetPos(CellPointToPoint(new Vector2(fieldPos.x, fieldPos.y)));
            chip.SetSpeed(chipSpeed, chipSpeed);
            chip.chipValue = chipValue;
            chip.match = null;
            chip.needToDelete = false;
            chip.transform.localScale = new Vector3(spriteScale, spriteScale, 1);

            return chip;
        }
        //endregion



        public void AddScore(int addedScore)
        {
            score += addedScore;

            if (gameManager.gameMode == GameMode.GM_INFINITY)
            {
                gpgsController.SetHighscore(score);

                if (score >= SCORE_ACHIEVEMENT_1)
                {
                    gpgsController.UnlockAchievement(GPGSIds.achievement_score_10k);
                }
                if (score >= SCORE_ACHIEVEMENT_2)
                {
                    gpgsController.UnlockAchievement(GPGSIds.achievement_score_50k);
                }
                if (score >= SCORE_ACHIEVEMENT_3)
                {
                    gpgsController.UnlockAchievement(GPGSIds.achievement_score_100k);
                }
            }

            if (gameManager.gameMode == GameMode.GM_INFINITY && score > gameManager.highScore)
            {
                gameManager.highScore = score;
            }

            if (gameManager.gameMode == GameMode.GM_LEVEL && levelType == LevelType.LT_SCORE && score >= winScore)
            {
                isWin = true;
                score = winScore;
            }

            if (gameManager.gameMode == GameMode.GM_LEVEL && levelType == LevelType.LT_SCORE)
            {
                screenManager.gameWindow.SetTextProgress(winScore - score);
            }

            screenManager.gameWindow.SetTextScore(score);
            screenManager.gameWindow.SetTextHighscore(gameManager.highScore);
        }



        private Back FindBack(int x, int y)
        {
            bool f = false;
            int i;
            for (i = 0; i < backs.Count; i++)
                if (backs[i].fieldPos.x == x && backs[i].fieldPos.y == y)
                {
                    f = true;
                    break;
                }
            if (f)
                return backs[i];
            else
                return null;
        }



        private IEnumerator OpenGameOverWindow()
        {
            yield return new WaitForSeconds(DELAY_TIME);

            screenManager.gameOverWindow.Open();
            screenManager.gameWindow.Close();
            screenManager.CloseGameScreen();

        }



        private IEnumerator OpenLoseWindow()
        {
            yield return new WaitForSeconds(DELAY_TIME);

            screenManager.loseWindow.Open();
            screenManager.gameWindow.Close();
            screenManager.CloseGameScreen();
        }



        private IEnumerator OpenWinWindow()
        {
            yield return new WaitForSeconds(DELAY_TIME);

            screenManager.winWindow.Open();
            screenManager.gameWindow.Close();
            screenManager.CloseGameScreen();

            if (!gameManager.isRated)
            {
                int curLevel = gameManager.currentLevel + 1;
                if (curLevel == 10 && !gameManager.isFirstRateShown)
                {
                    screenManager.rateUsWindow.Open();
                    gameManager.isFirstRateShown = true;
                }
                if (curLevel == 30 && !gameManager.isSecondRateShown)
                {
                    screenManager.rateUsWindow.Open();
                    gameManager.isSecondRateShown = true;
                }
                if (curLevel == 60 && !gameManager.isThirdRateShown)
                {
                    screenManager.rateUsWindow.Open();
                    gameManager.isThirdRateShown = true;
                }
            }
            gameManager.SaveHighscore();
        }



        private IEnumerator OpenLastWinWindow()
        {
            yield return new WaitForSeconds(DELAY_TIME);

            screenManager.lastWinWindow.Open();
            screenManager.gameWindow.Close();
            screenManager.CloseGameScreen();
        }



        public int GetScore()
        {
            return score;
        }



        public int GetWinScore()
        {
            return winScore;
        }



        public int GetStepsCount()
        {
            return stepsCount;
        }



        public int GetSpotsCount()
        {
            return spotsCount;
        }



        public int GetWinSpotsCount()
        {
            return winSpotsCount;
        }



        public void ShowInterstitial()
        {
            Debug.Log("ShowInterstitial");
            if (adsController.ShowInterstitial())
            {
                gameManager.advertTime = ADVERT_MAX_TIME;
            }
        }
    }
}
