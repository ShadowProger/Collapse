using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class RestartWindow : Window
    {
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

        public void YesButtonClick()
        {
            gameProcess.gamesWindow = GamesWindow.GW_GAME;
            screenManager.pauseWindow.Close();
            Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();

            gameProcess.NewGame();
        }

        public void NoButtonClick()
        {
            Close();
        }
    }
}
