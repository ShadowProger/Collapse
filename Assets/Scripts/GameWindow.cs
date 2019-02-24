using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class GameWindow : Window
    {
        public GameManager gameManager;
        public ScreenManager screenManager;
        public GameProcess gameProcess;

        public Text txtScore;
        public Text txtHighscore;
        public Transform pnlInfinity;
        public Transform pnlLevel;
        public Text txtLevelType;
        public Text txtProgress;
        public Text txtSteps;
        public Text txtLevelNumber;



        public override void Open()
        {
            base.Open();
            if (gameManager.gameMode == GameMode.GM_INFINITY)
            {
                pnlInfinity.gameObject.SetActive(true);
                pnlLevel.gameObject.SetActive(false);
            }
            else
            {
                pnlInfinity.gameObject.SetActive(false);
                pnlLevel.gameObject.SetActive(true);
                SetTextLevelNumber(gameManager.currentLevel + 1);
                SetTextSteps(0);
                Level level = gameManager.levels[gameManager.currentLevel];
                SetTextLevelType(level.type);
            }
            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            base.Close();
            transform.gameObject.SetActive(false);
        }

        public void PauseButtonClick()
        {
            if (gameProcess.gamesWindow == GamesWindow.GW_GAME)
            {
                gameProcess.gamesWindow = GamesWindow.GW_SETTINGS;
                screenManager.pauseWindow.Open();
                screenManager.gameWindow.Close();
                screenManager.CloseGameScreen();
            }
        }

        public void SetTextScore(int score)
        {
            txtScore.text = "" + score;
        }

        public void SetTextHighscore(int highscore)
        {
            txtHighscore.text = "" + highscore;
        }

        public void SetTextLevelType(LevelType levelType)
        {
            if (levelType == LevelType.LT_DIRT || levelType == LevelType.LT_NUMDIRT)
            {
                txtLevelType.text = "spots";
            }
            else
            {
                txtLevelType.text = "score";
            }
        }

        public void SetTextProgress(int value)
        {
            txtProgress.text = "" + value;
        }

        public void SetTextSteps(int steps)
        {
            txtSteps.text = "" + steps;
        }

        public void SetTextLevelNumber(int number)
        {
            txtLevelNumber.text = "" + number;
        }
    }
}
