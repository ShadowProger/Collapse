using UnityEngine;

namespace Manybits
{
    public enum LevelType { LT_DIRT, LT_NUMDIRT, LT_SCORE }
    public enum LevelAccess { LP_COMPLETED, LP_LOCKED, LP_UNLOCKED }

    public class Level
    {
        public int number;
        public int width;
        public int height;
        public LevelType type;
        public int score;
        public int[,] backCells;

        public LevelAccess access;
        public int steps;

        public int chipGen;
        public int chipStartGen;

        public Level(int width, int height, LevelType type)
        {
            number = 0;

            this.width = width;
            this.height = height;
            this.type = type;
            score = 0;
            backCells = new int[height, width];

            access = LevelAccess.LP_LOCKED;
            steps = 0;

            chipGen = 0;
            chipStartGen = 0;
    }

        public override string ToString()
        {
            string strBack = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    strBack += backCells[i, j] + " ";
                strBack += "\n";
            }
            string str = "width: " + width + "\n" +
                "height: " + height + "\n" +
                "type: " + type.ToString() + "\n" +
                "score: " + score + "\n" +
                "chipGen: " + chipGen + "\n" +
                "chipStartGen: " + chipStartGen + "\n" + strBack;
            return str;
        }
    }
}
