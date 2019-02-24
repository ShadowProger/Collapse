using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

namespace Manybits
{
    public enum GameMode { GM_INFINITY, GM_LEVEL };

    public class GameManager : MonoBehaviour
    {
        public ScreenManager screenManager;

        public GameMode gameMode;
        public int currentLevel;
        public int lastLevel;
        public List<Level> levels;

        public float gameTime;
        public float advertTime = 0.0f;

        public bool isRated = false;
        public bool isFirstRateShown = false;
        public bool isSecondRateShown = false;
        public bool isThirdRateShown = false;

        public int highScore;
        public bool continueInfinityGame;

        public string path;

        private string fileNameLevels = "GameProgress.xml";
        private string fileNameHighscores = "Highscores.bin";

        

        // Use this for initialization
        void Awake()
        {
            gameTime = 0;
            levels = new List<Level>();

#if UNITY_ANDROID
            path = Application.persistentDataPath;
#endif

#if UNITY_EDITOR
            path = Application.dataPath;
#endif

            highScore = 0;
            continueInfinityGame = false;

            bool isLoaded = LoadHighscore();

            Debug.Log("     isLoaded = " + isLoaded);

            LoadLevels();
            LoadProgress();

            Debug.Log("Highscore = " + highScore);

            screenManager.mainMenuWindow.Open();
        }



        private void LoadLevels()
        {
            Debug.Log("[GameManager] LoadLevels");
            XmlDocument xml = new XmlDocument();

            xml.LoadXml(Resources.Load("Levels").ToString());
            XmlNodeList dataList = xml.GetElementsByTagName("level");

            levels.Clear();

            int levelIndex = 1;
            foreach (XmlNode item in dataList)
            {
                int width = int.Parse(item.Attributes["width"].Value);
                int height = int.Parse(item.Attributes["height"].Value);
                string levelType = item.Attributes["type"].Value;

                LevelType type = LevelType.LT_DIRT;
                switch (levelType)
                {
                    case "dirt":
                        type = LevelType.LT_DIRT;
                        break;
                    case "num_dirt":
                        type = LevelType.LT_NUMDIRT;
                        break;
                    case "score":
                        type = LevelType.LT_SCORE;
                        break;
                }

                Level level = new Level(width, height, type);

                if (type == LevelType.LT_SCORE)
                {
                    level.score = int.Parse(item.Attributes["score"].Value);
                }
                else
                {
                    XmlNodeList itemContent = item.ChildNodes;
                    foreach (XmlNode itemItens in itemContent)
                    {
                        if (itemItens.Name == "cell")
                        {
                            int x = int.Parse(itemItens.Attributes["x"].Value);
                            int y = int.Parse(itemItens.Attributes["y"].Value);
                            level.backCells[y, x] = int.Parse(itemItens.InnerText);
                        }
                    }
                }

                level.number = levelIndex;

                if (item.Attributes["chipGen"] != null)
                    level.chipGen = int.Parse(item.Attributes["chipGen"].Value);
                else
                    level.chipGen = 8;

                if (item.Attributes["chipStartGen"] != null)
                    level.chipStartGen = int.Parse(item.Attributes["chipStartGen"].Value);
                else
                    level.chipStartGen = 15;

                levelIndex++;
                levels.Add(level);
            }

            if (levels.Count > 0)
            {
                levels[0].access = LevelAccess.LP_UNLOCKED;
            }

            lastLevel = levels.Count;
        }



        public void SaveHighscore()
        {
            Debug.Log("[GameManager] SaveHighscore");
            string path = Path.Combine(this.path, fileNameHighscores);

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(highScore);
                writer.Write(continueInfinityGame);
                writer.Write(isRated);
                writer.Write(isFirstRateShown);
                writer.Write(isSecondRateShown);
                writer.Write(isThirdRateShown);
            }
        }



        public bool LoadHighscore()
        {
            Debug.Log("[GameManager] LoadHighscore");
            string path = Path.Combine(this.path, fileNameHighscores);

            if (File.Exists(path))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    try
                    {
                        highScore = reader.ReadInt32();
                        continueInfinityGame = reader.ReadBoolean();
                        isRated = reader.ReadBoolean();
                        isFirstRateShown = reader.ReadBoolean();
                        isSecondRateShown = reader.ReadBoolean();
                        isThirdRateShown = reader.ReadBoolean();
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }



        public void SaveProgress()
        {
            Debug.Log("[GameManager] SaveProgress");
            XmlDocument xml = new XmlDocument();

            string filePath = Path.Combine(path, fileNameLevels);

            XmlNode root = xml.CreateElement("levels");
            xml.AppendChild(root);

            XmlElement level;
            for (int i = 0; i < levels.Count; i++)
            {
                level = xml.CreateElement("level");

                string levelAccess = "";
                switch (levels[i].access)
                {
                    case LevelAccess.LP_LOCKED:
                        levelAccess = "locked";
                        break;
                    case LevelAccess.LP_UNLOCKED:
                        levelAccess = "unlocked";
                        break;
                    case LevelAccess.LP_COMPLETED:
                        levelAccess = "completed";
                        break;
                }

                level.SetAttribute("access", levelAccess);
                level.SetAttribute("steps", levels[i].steps.ToString());

                root.AppendChild(level);
            }

            xml.Save(filePath);
        }



        public void LoadProgress()
        {
            Debug.Log("[GameManager] LoadProgress");

            XmlDocument xml = new XmlDocument();
            string filePath = Path.Combine(path, fileNameLevels);

            if (!File.Exists(filePath))
            {
                Debug.Log("File is not exist");
                return;
            }

            string xmlString = "";

            xmlString = File.ReadAllText(filePath);

            xml.LoadXml(xmlString);
            XmlNodeList dataList = xml.GetElementsByTagName("level");

            Debug.Log("levelsCount = " + dataList.Count);

            int levelIndex = 0;
            foreach (XmlNode item in dataList)
            {
                string levelAcces = item.Attributes["access"].Value;

                switch (levelAcces)
                {
                    case "locked":
                        levels[levelIndex].access = LevelAccess.LP_LOCKED;
                        break;
                    case "unlocked":
                        levels[levelIndex].access = LevelAccess.LP_UNLOCKED;
                        break;
                    case "completed":
                        levels[levelIndex].access = LevelAccess.LP_COMPLETED;
                        break;
                }

                levels[levelIndex].steps = int.Parse(item.Attributes["steps"].Value);
                levelIndex++;
            }
        }
    }
}
