using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class Scrolling : MonoBehaviour
    {
        public GameObject levelPanelPref;
        public CameraManager cameraManager;
        public ScrollRect scrollRect;
        public GameObject LevelIconPref;
        public GameManager gameManager;
        public LevelsWindow levelsWindow;

        [Range(0f, 20f)]
        public float scrollSpeed;
        [Range(0f, 1000f)]
        public float minScrollVelocity;

        private int panelsCount;
        private int levelsOnPanel = 30;
        private int levelsCount;
        private GameObject[] panels;
        private Vector2[] panelPositions;
        private LevelIcon[] levelIcons;

        private RectTransform contentRect;
        private int selectedPanelIndex;

        private bool isScrolling;

        // Use this for initialization
        void Awake()
        {
            levelsCount = gameManager.levels.Count;
            panelsCount = (levelsCount - 1) / levelsOnPanel + 1;
            Debug.Log("[Scrolling][Awake] panelsCount = " + panelsCount);

            levelIcons = new LevelIcon[levelsCount];
            panels = new GameObject[panelsCount];
            panelPositions = new Vector2[panelsCount];

            contentRect = GetComponent<RectTransform>();

            float panelWidth = levelPanelPref.GetComponent<RectTransform>().sizeDelta.x;
            float panelDistance = (cameraManager.ScreenWidth - panelWidth) * 0.5f + 50;

            for (int i = 0; i < panelsCount; i++)
            {
                panels[i] = Instantiate(levelPanelPref, transform, false);
                float y = panels[i].transform.localPosition.y;
                panels[i].transform.localPosition = new Vector2(i * (panelWidth + panelDistance), y);
                panelPositions[i] = -panels[i].transform.localPosition;
            }

            for (int i = 0; i < levelsCount; i++)
            {
                int panelIndex = i / levelsOnPanel;
                GameObject levelIcon = Instantiate(LevelIconPref, panels[panelIndex].transform, false);
                levelIcons[i] = levelIcon.GetComponent<LevelIcon>();
                Level level = gameManager.levels[i];
                levelIcons[i].SetProperty(level);
                Button button = levelIcon.GetComponent<Button>();
                int levelNumber = i;
                button.onClick.AddListener(() => { levelsWindow.SelectLevel(levelNumber); });
            }
        }

        // Update is called once per frame
        void Update()
        {
            if ((contentRect.anchoredPosition.x > panelPositions[0].x || contentRect.anchoredPosition.x < panelPositions[panelsCount - 1].x) && !isScrolling)
            {
                scrollRect.inertia = false;
            }

            float nearestPos = float.MaxValue;

            for (int i = 0; i < panelsCount; i++)
            {
                float distance = Mathf.Abs(contentRect.anchoredPosition.x - panelPositions[i].x);
                if (distance < nearestPos)
                {
                    nearestPos = distance;
                    selectedPanelIndex = i;
                }
            }

            float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);
            if (scrollVelocity < minScrollVelocity && !isScrolling)
                scrollRect.inertia = false;

            if (isScrolling || scrollVelocity > minScrollVelocity) return;
            contentRect.anchoredPosition = new Vector2(Mathf.SmoothStep(contentRect.anchoredPosition.x, panelPositions[selectedPanelIndex].x, scrollSpeed * Time.deltaTime), contentRect.anchoredPosition.y);
        }

        public void SetScrolling(bool scroll)
        {
            isScrolling = scroll;
            if (scroll)
                scrollRect.inertia = true;
        }

        public void Refresh()
        {
            for (int i = 0; i < levelIcons.Length; i++)
            {
                levelIcons[i].SetProperty(gameManager.levels[i]);
            }
        }
    }
}
