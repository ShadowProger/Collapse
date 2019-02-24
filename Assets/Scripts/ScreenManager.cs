using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class ScreenManager : MonoBehaviour
    {
        public GameManager gameManager;
        public Scrolling scrolling;

        public MainMenuWindow mainMenuWindow;
        public LevelsWindow levelsWindow;
        public GameWindow gameWindow;
        public PauseWindow pauseWindow;
        public LoseWindow loseWindow;
        public WinWindow winWindow;
        public LastWinWindow lastWinWindow;
        public GameOverWindow gameOverWindow;
        public RateUsWindow rateUsWindow;
        public LevelBriefWindow levelBriefWindow;
        public RestartWindow restartWindow;

        //public bool useInitScreen;
        //public GameObject initScreen;
        //public GameObject initGui;
        //public GameObject menuScreen;
        //public GameObject menuGui;
        public GameObject gameScreen;
        //public GameObject gameGui;
        //public GameObject levelScreen;
        //public GameObject levelGui;

        //public GameObject pauseWindow;
        //public GameObject winWindow;
        //public GameObject lastwinWindow;
        //public GameObject loseWindow;
        //public GameObject gameOverWindow;

        //private GameObject currentScreen;
        //private GameObject currentGui;


        public void Start()
        {
            //if (useInitScreen)
            //{
            //    CloseAll();
            //    OpenInitScreen();
            //}
        }

        public void OpenGameScreen()
        {
            gameScreen.SetActive(true);
        }

        public void CloseGameScreen()
        {
            gameScreen.SetActive(false);
        }

        //public void CloseAll()
        //{
        //    print("[ScreenManager] CloseAll");
        //    menuScreen.gameObject.SetActive(false);
        //    menuGui.SetActive(false);
        //    gameScreen.gameObject.SetActive(false);
        //    gameGui.SetActive(false);
        //}

        //public void CloseCurrentScreen()
        //{
        //    currentScreen.gameObject.SetActive(false);
        //    currentGui.SetActive(false);
        //    print("[ScreenManager] " + currentScreen.ToString() + " was closed");
        //}

        //public void OpenInitScreen()
        //{
        //    print("[ScreenManager] OpenInitScreen");
        //    initScreen.SetActive(true);
        //    initGui.SetActive(true);
        //    currentScreen = initScreen;
        //    currentGui = initGui;
        //}

        //public void OpenMenuScreen()
        //{
        //    CloseCurrentScreen();
        //    print("[ScreenManager] OpenMenuScreen");
        //    menuScreen.SetActive(true);
        //    menuGui.SetActive(true);
        //    currentScreen = menuScreen;
        //    currentGui = menuGui;
        //}

        //public void OpenGameScreen()
        //{
        //    CloseCurrentScreen();
        //    print("[ScreenManager] OpenGameScreen");
        //    gameScreen.SetActive(true);
        //    gameGui.SetActive(true);
        //    currentScreen = gameScreen;
        //    currentGui = gameGui;
        //}

        //public void OpenLevelScreen()
        //{
        //    CloseCurrentScreen();
        //    print("[ScreenManager] OpenLevelScreen");
        //    levelScreen.SetActive(true);
        //    levelGui.SetActive(true);
        //    currentScreen = levelScreen;
        //    currentGui = levelGui;

        //    scrolling.Refresh();
        //}

        //public void OpenPauseWindow()
        //{
        //    print("[ScreenManager] OpenPauseWindow");
        //    pauseWindow.SetActive(true);
        //}

        //public void ClosePauseWindow()
        //{
        //    print("[ScreenManager] ClosePauseWindow");
        //    pauseWindow.SetActive(false);
        //}

        //public void OpenLoseWindow()
        //{
        //    print("[ScreenManager] OpenLoseWindow");
        //    loseWindow.SetActive(true);
        //}

        //public void CloseLoseWindow()
        //{
        //    print("[ScreenManager] CloseLoseWindow");
        //    loseWindow.SetActive(false);
        //}

        //public void OpenWinWindow()
        //{
        //    print("[ScreenManager] OpenWinWindow");
        //    winWindow.SetActive(true);
        //}

        //public void CloseWinWindow()
        //{
        //    print("[ScreenManager] CloseWinWindow");
        //    winWindow.SetActive(false);
        //}

        //public void OpenLastWinWindow()
        //{
        //    print("[ScreenManager] OpenLastWinWindow");
        //    lastwinWindow.SetActive(true);
        //}

        //public void CloseLastWinWindow()
        //{
        //    print("[ScreenManager] CloseLastWinWindow");
        //    lastwinWindow.SetActive(false);
        //}

        //public void OpenGameOverWindow()
        //{
        //    print("[ScreenManager] OpenGameOverWindow");
        //    gameOverWindow.SetActive(true);
        //}

        //public void CloseGameOverWindow()
        //{
        //    print("[ScreenManager] CloseGameOverWindow");
        //    gameOverWindow.SetActive(false);
        //}
    }
}
