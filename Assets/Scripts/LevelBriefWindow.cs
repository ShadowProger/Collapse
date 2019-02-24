using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class LevelBriefWindow : Window
    {
        public GameManager gameManager;
        public GameProcess gameProcess;

        [SerializeField]
        private Text txtTitle;
        [SerializeField]
        private Text txtMessage;

        public override void Open()
        {
            txtTitle.text = "Level " + (gameManager.currentLevel + 1);
            LevelType levelType = gameManager.levels[gameManager.currentLevel].type;
            if (levelType == LevelType.LT_DIRT || levelType == LevelType.LT_NUMDIRT)
            {
                txtMessage.text = "Clear " + gameProcess.GetWinSpotsCount() + " spots\n to complete level";
            }
            else
            {
                txtMessage.text = "Get " + gameProcess.GetWinScore() + " points\n to complete level";
            }
            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            transform.gameObject.SetActive(false);
        }

        public void OkButtonClick()
        {
            Close();
            gameProcess.gamesWindow = GamesWindow.GW_GAME;
        }
    }
}
