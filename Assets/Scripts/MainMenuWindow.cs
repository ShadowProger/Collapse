using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class MainMenuWindow : Window
    {
        public GameManager gameManager;
        public ScreenManager screenManager;
        public GameProcess gameProcess;
        public ShareAndRate shareAndRate;
        public GpgsController gpgsController;



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

        public void PlayButtonClick()
        {
            screenManager.levelsWindow.Open();
            screenManager.mainMenuWindow.Close();
        }

        public void InfinityButtonClick()
        {
            gameManager.gameMode = GameMode.GM_INFINITY;
            screenManager.mainMenuWindow.Close();
            screenManager.OpenGameScreen();
            screenManager.gameWindow.Open();
            gameProcess.StartGame();
        }

        public void ShareButtonClick()
        {
            shareAndRate.OnAndroidTextSharingClick();
        }

        public void LeaderboardButtonClick()
        {
            gpgsController.OpenLeaderboard();
        }

        public void AchievementsButtonClick()
        {
            gpgsController.OpenAchievements();
        }
    }
}
