using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class LevelsWindow : Window
    {
        [SerializeField]
        private Scrolling scroll;
        public GameManager gameManager;
        public ScreenManager screenManager;
        public GameProcess gameProcess;



        public override void Open()
        {
            base.Open();
            transform.gameObject.SetActive(true);
            scroll.Refresh();
        }

        public override void Close()
        {
            base.Close();
            transform.gameObject.SetActive(false);
        }

        public void MenuButtonClick()
        {
            screenManager.levelsWindow.Close();
            screenManager.mainMenuWindow.Open();
        }

        public void SelectLevel(int levelNumber)
        {
            Debug.Log("[InterfaceEvents][SelectLevel] levelNumber: " + levelNumber);
            gameManager.gameMode = GameMode.GM_LEVEL;
            gameManager.currentLevel = levelNumber;
            screenManager.levelsWindow.Close();
            screenManager.gameWindow.Open();
            screenManager.OpenGameScreen();
            gameProcess.StartGame();
        }
    }
}
