using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public enum BackType { BT_DIRT, BT_NUMDIRT }

    public class Back : MonoBehaviour
    {
        private SpriteRenderer backSprite;
        public BackType backType;
        public Sprite[] dirtSprites;
        public Sprite[] numDirtSprites;
        public Vector2 fieldPos;

        // Use this for initialization
        void Awake()
        {
            backSprite = GetComponent<SpriteRenderer>();
        }

        public void SetNumber(int number)
        {
            if (backType == BackType.BT_DIRT)
                backSprite.sprite = dirtSprites[number - 1];
            else
                backSprite.sprite = numDirtSprites[number - 2];
        }
    }
}
