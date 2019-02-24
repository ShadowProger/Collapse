using UnityEngine;
using System.Collections;

namespace Manybits
{
    public class MovingObject : MonoBehaviour
    {

        public Vector3 pos;// = new Vector3(0, 0, -1);
        public Vector3 newPos;// = new Vector3(0, 0, -1);
        public float speedX = 0;
        public float speedY = 0;
        public bool isOnPlace = true;



        // Use this for initialization
        void Start()
        {

        }



        // Update is called once per frame
        void Update()
        {

        }



        public virtual void OnArrive() { }



        public void Move(float delta)
        {
            //Debug.Log("speedX = " + speedX + ", speedY = " + speedY);
            if (isOnPlace)
                return;

            if (newPos.x > pos.x)
            {
                pos.x += speedX * delta;
                if (pos.x > newPos.x)
                    pos.x = newPos.x;
            }
            else
            {
                if (newPos.x < pos.x)
                {
                    pos.x -= speedX * delta;
                    if (pos.x < newPos.x)
                        pos.x = newPos.x;
                }
            }
            if (newPos.y > pos.y)
            {
                pos.y += speedY * delta;
                if (pos.y > newPos.y)
                    pos.y = newPos.y;
            }
            else
            {
                if (newPos.y < pos.y)
                {
                    pos.y -= speedY * delta;
                    if (pos.y < newPos.y)
                        pos.y = newPos.y;
                }
            }

            //transform.position.Set(pos.x, pos.y, pos.z);
            transform.position = pos;

            if (pos.x == newPos.x && pos.y == newPos.y)
            {
                isOnPlace = true;
                OnArrive();
            }
            else
                isOnPlace = false;
        }



        public Vector3 GetPos()
        {
            Vector3 p = new Vector3(pos.x, pos.y, pos.z);
            return p;
        }



        public void SetPos(Vector3 pos)
        {
            this.pos.x = pos.x;
            this.pos.y = pos.y;
            newPos.x = pos.x;
            newPos.y = pos.y;
            transform.position = this.pos;
            isOnPlace = true;
        }



        public void SetPos(float x, float y)
        {
            this.pos.x = x;
            this.pos.y = y;
            newPos.x = pos.x;
            newPos.y = pos.y;
            transform.position = this.pos;
            isOnPlace = true;
        }



        public Vector3 GetNewPos()
        {
            Vector3 p = new Vector3(newPos.x, newPos.y, newPos.z);
            return p;
        }



        public void SetNewPos(Vector3 newPos)
        {
            this.newPos.Set(newPos.x, newPos.y, this.newPos.z);
            if (pos.x == newPos.x && pos.y == newPos.y)
                isOnPlace = true;
            else
                isOnPlace = false;
        }



        public void SetNewPos(float x, float y)
        {
            newPos.Set(x, y, this.newPos.z);
            if (pos.x == newPos.x && pos.y == newPos.y)
                isOnPlace = true;
            else
                isOnPlace = false;
        }



        public void SetSpeed(Vector2 speed)
        {
            speedX = speed.x;
            speedY = speed.y;
        }



        public void SetSpeed(float speedX, float speedY)
        {
            this.speedX = speedX;
            this.speedY = speedY;
        }



        public float GetSpeedX()
        {
            return speedX;
        }



        public void SetSpeedX(float speedX)
        {
            this.speedX = speedX;
        }



        public float GetSpeedY()
        {
            return speedY;
        }



        public void SetSpeedY(float speedY)
        {
            this.speedY = speedY;
        }



        public bool IsOnPlace()
        {
            return isOnPlace;
        }
    }
}
