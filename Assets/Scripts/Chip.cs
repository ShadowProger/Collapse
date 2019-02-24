using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class Chip : MovingObject
    {
        public SpriteRenderer chipSprite;
        public Vector2 fieldPos = new Vector2(0, 0);
        public bool isOnPath;
        public bool isInDestination;
        public bool needToDelete;
        public int chipValue;
        private List<Vector2> path = new List<Vector2>();
        private int nextPathStep;
        public ChipMatch match = null;
        public bool isAnim = false;
        private Animator animator;



        // Use this for initialization
        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }



        public void Update(float delta)
        {
            Move(delta);
            if (isOnPath)
            {
                if (IsOnPlace())
                {
                    nextPathStep++;
                    if (nextPathStep >= path.Count)
                    {
                        isOnPath = false;
                        isInDestination = true;
                        //movingChip = this;
                        //movingChipIsOnPlace = true;
                    }
                    else
                    {
                        MoveTo(path[nextPathStep]);
                    }
                }
            }
        }



        public override void OnArrive()
        {
            if (match != null)
            {
                match.isMove = false;
                match = null;
                needToDelete = true;
                //chipsForDelete.add(this);
            }
        }



        public void FollowPath(Vector2[] path)
        {
            if (path.Length == 0)
                return;
            this.path.Clear();
            this.path.AddRange(path);
            nextPathStep = 0;
            isOnPath = true;
            isInDestination = false;
            MoveTo(path[0]);
        }



        public void MoveTo(Vector2 point)
        {
            float x = point.x;
            float y = point.y;
            SetNewPos(x, y);
        }



        public void SetSprite(Sprite sprite)
        {
            chipSprite.sprite = sprite;
        }



        public void SetFieldPos(Vector2 fieldPos)
        {
            this.fieldPos.x = fieldPos.x;
            this.fieldPos.y = fieldPos.y;
        }

        public void PlayAnimation(string animName)
        {
            isAnim = true;
            animator.Play(animName);
        }
    }
}
