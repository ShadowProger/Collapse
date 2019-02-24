using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Manybits
{
    public class TouchAreaCollider : MonoBehaviour, IPointerDownHandler
    {
        public GameProcess gameProcess;

#if UNITY_EDITOR
        public void OnMouseDown()
        {
            gameProcess.MouseDown();
        }
#endif

#if UNITY_ANDROID
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0)
                return;
            gameProcess.MouseDown();
        }
#endif
    }
}
