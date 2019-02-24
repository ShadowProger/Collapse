using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class RateUsWindow : Window
    {
        public GameManager gameManager;
        public ShareAndRate shareAndRate;

        public override void Open()
        {
            transform.gameObject.SetActive(true);
        }

        public override void Close()
        {
            transform.gameObject.SetActive(false);
        }

        public void LaterButtonClick()
        {
            Close();
        }

        public void RateButtonClick()
        {
            shareAndRate.RateUs();
            gameManager.isRated = true;
            Close();
        }
    }
}
