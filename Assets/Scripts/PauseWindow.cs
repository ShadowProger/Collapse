using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class PauseWindow : Window
    {
        public GameManager gameManager;
        public ScreenManager screenManager;
        public GameProcess gameProcess;



        public override void Open()
        {
            base.Open();
            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            base.Close();
            transform.gameObject.SetActive(false);
        }

        public void ContinueButtonClick()
        {
            gameProcess.gamesWindow = GamesWindow.GW_GAME;
            screenManager.pauseWindow.Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();
        }

        public void RestartButtonClick()
        {
            screenManager.restartWindow.Open();

            //gameProcess.gamesWindow = GamesWindow.GW_GAME;
            //screenManager.pauseWindow.Close();
            //screenManager.gameWindow.Open();
            //screenManager.OpenGameScreen();

            //gameProcess.NewGame();
        }

        public void MenuButtonClick()
        {
            //gameProcess.ClearList();

            if (gameManager.gameMode == GameMode.GM_INFINITY)
            {
                screenManager.pauseWindow.Close();
                screenManager.CloseGameScreen();
                screenManager.mainMenuWindow.Open();
            }
            else
            {
                screenManager.pauseWindow.Close();
                screenManager.CloseGameScreen();
                screenManager.levelsWindow.Open();
            }
        }
    }
}
