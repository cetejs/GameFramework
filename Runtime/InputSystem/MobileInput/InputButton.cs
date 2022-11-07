using GameFramework.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.InputService
{
    public class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string buttonName = "Button";

        private void OnDisable()
        {
            if (!Global.IsApplicationQuitting)
            {
                Global.GetService<InputManager>().SetButtonUp(buttonName);
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Global.GetService<InputManager>().SetButtonDown(buttonName);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Global.GetService<InputManager>().SetButtonUp(buttonName);
        }
    }
}