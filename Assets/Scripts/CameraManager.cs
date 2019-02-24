using UnityEngine;

namespace Manybits
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private bool isConstantWidth;
        [SerializeField]
        private int cameraWidth;
        [SerializeField]
        private int cameraHeight;

        private float leftBorder;
        private float rightBorder;
        private float topBorder;
        private float bottomBorder;
        private float screenHeight;
        private float screenWidth;
        private float scaleCoef;

        public bool IsConstantWidth { get { return isConstantWidth; } }
        public float LeftBorder { get { return leftBorder; } }
        public float RightBorder { get { return rightBorder; } }
        public float TopBorder { get { return topBorder; } }
        public float BottomBorder { get { return bottomBorder; } }
        public float ScreenHeight { get { return screenHeight; } }
        public float ScreenWidth { get { return screenWidth; } }
        public float ScaleCoef { get { return scaleCoef; } }


        void Awake()
        {
            SetCamSize(cameraWidth, cameraHeight, isConstantWidth);
        }

        public void SetCamSize(int newWidth, int newHeight, bool isConstantWidth)
        {
            this.isConstantWidth = isConstantWidth;
            if (isConstantWidth)
            {
                float ratio = (float)Screen.height / (float)Screen.width;
                float newSize = cameraWidth * ratio * 0.5f;
                Camera.main.orthographicSize = newSize;
            }
            else
            {
                Camera.main.orthographicSize = newHeight * 0.5f;
            }

            RecalcBorders();
            screenHeight = topBorder - bottomBorder;
            screenWidth = rightBorder - leftBorder;
            scaleCoef = screenHeight / (float)Screen.height;
        }

        public void RecalcBorders()
        {
            leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
            bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        }
    }
}
