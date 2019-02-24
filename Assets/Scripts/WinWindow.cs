using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class WinWindow : Window
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
            txtLevelNumber.text = "level " + (gameManager.currentLevel + 1);
            txtSteps.text = "" + gameProcess.GetStepsCount();
            LevelType levelType = gameManager.levels[gameManager.currentLevel].type;
            if (levelType == LevelType.LT_DIRT || levelType == LevelType.LT_NUMDIRT)
            {
                txtGoalType.text = "dirt";
                int winSpotsCount = gameProcess.GetWinSpotsCount();
                int spotsCount = winSpotsCount - gameProcess.GetSpotsCount();
                txtGoalProgress.text = spotsCount + " / " + winSpotsCount;
            }
            else
            {
                txtGoalType.text = "score";
                int winScore = gameProcess.GetWinScore();
                int score = gameProcess.GetScore();
                txtGoalProgress.text = score + " / " + winScore;
            }
            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            transform.gameObject.SetActive(false);
        }

        public void NextButtonClick()
        {
            gameManager.currentLevel++;
            screenManager.winWindow.Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();
            gameProcess.NewGame();
        }

        public void RestartButtonClick()
        {
            screenManager.winWindow.Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();
            gameProcess.NewGame();
        }

        public void MenuButtonClick()
        {
            //gameProcess.ClearList();

            screenManager.winWindow.Close();
            screenManager.levelsWindow.Open();
            screenManager.CloseGameScreen();
        }
    }
}
