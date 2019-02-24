using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class GameOverWindow : Window
    {
        public GameManager gameManager;
        public ScreenManager screenManager;
        public GameProcess gameProcess;
        public ShareAndRate shareAndRate;

        [SerializeField]
        private Text txtScore;
        [SerializeField]
        private Text txtHighscore;

        public override void Open()
        {
            Debug.Log("[GameOverWindow] Open");
            txtHighscore.text = "" + gameManager.highScore;
            txtScore.text = "" + gameProcess.GetScore();
            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            Debug.Log("[GameOverWindow] Close");
            transform.gameObject.SetActive(false);
        }

        public void RestartButtonClick()
        {
            gameProcess.gamesWindow = GamesWindow.GW_GAME;
            screenManager.gameOverWindow.Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();

            gameProcess.NewGame();
        }

        public void MenuButtonClick()
        {
            //gameProcess.ClearList();

            screenManager.gameOverWindow.Close();
            screenManager.CloseGameScreen();
            screenManager.mainMenuWindow.Open();
        }

        public void ShareButtonClick()
        {
            shareAndRate.OnAndroidTextSharingClick();
        }
    }
}
