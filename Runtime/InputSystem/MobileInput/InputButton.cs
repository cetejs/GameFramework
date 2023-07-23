using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string buttonName = "Button";

        public void OnPointerDown(PointerEventData eventData)
        {
            InputManager.Instance.SetButtonDown(buttonName);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputManager.Instance.SetButtonUp(buttonName);
        }
    }
}