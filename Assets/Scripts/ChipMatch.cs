using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class ChipMatch
    {
        public int value;
        public int count;
        public List<Chip> matchChips = new List<Chip>();
        public Vector2 mainPoint;
        public bool isMove;
        public int multiMergeBonus;
        public int chipScore;

        public ChipMatch(Match match, int chipScore, int bonus)
        {
            value = match.value;
            count = match.count;
            mainPoint = match.mainPoint;
            isMove = false;
            multiMergeBonus = bonus;
            this.chipScore = chipScore;
            /*
            for (int i = 0; i < match.points.Length; i++)
            {
                int x = (int)match.points[i].x;
                int y = (int)match.points[i].y;
                Chip chip = FindChip(x, y);
                if (chip != null)
                {
                    chip.match = this;
                    matchChips.Add(chip);
                }
            }
            */
        }
    }
}
