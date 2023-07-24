using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string buttonName = "Button";
        private InputControl input;

        private void Start()
        {
            input = GetComponentInParent<InputControl>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (input != null)
            {
                input.SetButtonDown(buttonName);
            }
            else
            {
                InputManager.Instance.SetButtonDown(buttonName, InputIdentity.Player1);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (input != null)
            {
                input.SetButtonUp(buttonName);
            }
            else
            {
                InputManager.Instance.SetButtonUp(buttonName, InputIdentity.Player1);
            }
        }
    }
}