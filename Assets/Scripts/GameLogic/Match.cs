using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class Match
    {
        public int value;
        public int count;
        public Vector2[] points;
        public Vector2 mainPoint;
        public override string ToString()
        {
            string str = "Value: " + value + ", Count: " + count + ", " + mainPoint + ", { ";
            for (int i = 0; i < points.Length; i++)
                str += points[i].ToString() + " ";
            str += "}";
            return str;
        }
    }
}
