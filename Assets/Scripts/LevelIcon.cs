using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class LevelIcon : MonoBehaviour
    {
        public GameObject goComplete;

        public Text txtLevelNumber;
        public Text txtStepsCount;

        public Sprite spLocked;
        public Sprite spUnlocked;
        public Sprite spCompleted;

        private Image image;
        private Button button;

        // Use this for initialization
        void Awake()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetProperty(Level level)
        {
            txtLevelNumber.text = "" + level.number;
            txtStepsCount.text = "steps: " + level.steps;
            switch (level.access)
            {
                case LevelAccess.LP_COMPLETED:
                    goComplete.SetActive(true);
                    image.sprite = spCompleted;
                    button.interactable = true;
                    break;
                case LevelAccess.LP_LOCKED:
                    goComplete.SetActive(false);
                    image.sprite = spLocked;
                    button.interactable = false;
                    break;
                case LevelAccess.LP_UNLOCKED:
                    goComplete.SetActive(false);
                    image.sprite = spUnlocked;
                    button.interactable = true;
                    break;
            }
        }
    }
}
