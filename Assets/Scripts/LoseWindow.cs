using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class LoseWindow : Window
    {
        public GameManager gameManager;
        public ScreenManager screenManager;
        public GameProcess gameProcess;

        [SerializeField]
        private Text txtLevelNumber;
        [SerializeField]
        private Text txtSteps;
        [SerializeField]
        private Text txtGoalType;
        [SerializeField]
        private Text txtGoalProgress;

        public override void Open()
        {
            // #ff6b5eff
            // <color=#00ffffff>
            // <color=green>green</color>

            txtLevelNumber.text = "level " + (gameManager.currentLevel + 1);
            txtSteps.text = "" + gameProcess.GetStepsCount();
            LevelType levelType = gameManager.levels[gameManager.currentLevel].type;
            if (levelType == LevelType.LT_DIRT || levelType == LevelType.LT_NUMDIRT)
            {
                txtGoalType.text = "dirt";
                int winSpotsCount = gameProcess.GetWinSpotsCount();
                int spotsCount = winSpotsCount - gameProcess.GetSpotsCount();
                txtGoalProgress.text = "<color=#ff6b5eff>" + spotsCount + "</color> / " + winSpotsCount;
            }
            else
            {
                txtGoalType.text = "score";
                int winScore = gameProcess.GetWinScore();
                int score = gameProcess.GetScore();
                txtGoalProgress.text = "<color=#ff6b5eff>" + score + "</color> / " + winScore;
            }

            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            transform.gameObject.SetActive(false);
        }

        public void MenuButtonClick()
        {
            //gameProcess.ClearList();

            screenManager.loseWindow.Close();
            screenManager.CloseGameScreen();
            screenManager.levelsWindow.Open();
        }

        public void RestartButtonClick()
        {
            gameProcess.gamesWindow = GamesWindow.GW_GAME;
            screenManager.loseWindow.Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();

            gameProcess.NewGame();
        }
    }
}
