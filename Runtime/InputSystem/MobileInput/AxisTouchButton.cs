using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private AxisOption axisType = AxisOption.Positive;
        [SerializeField]
        private string axisName = "Horizontal";
        [SerializeField]
        private float responseSpeed = 1.0f;

        private bool isPointerDown;
        private float axisValue;
        private InputControl input;

        private void Start()
        {
            input = GetComponentInParent<InputControl>();
        }

        private void Update()
        {
            if (isPointerDown)
            {
                if (axisType == AxisOption.Negative)
                {
                    UpdateAxis(-1);
                }
                else if (axisType == AxisOption.Positive)
                {
                    UpdateAxis(1);
                }
            }
        }

        private void UpdateAxis(float value)
        {
            if (!Mathf.Approximately(axisValue, value))
            {
                axisValue = Mathf.MoveTowards(axisValue, value, responseSpeed * Time.deltaTime);

                if (input != null)
                {
                    input.SetAxis(axisName, axisValue);
                }
                else
                {
                    InputManager.Instance.SetAxis(axisName, axisValue);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            axisValue = 0;

            if (input != null)
            {
                input.SetAxisZero(axisName);
            }
            else
            {
                InputManager.Instance.SetAxisZero(axisName, InputIdentity.Player1);
            }
        }

        private enum AxisOption
        {
            Positive,
            Negative
        }
    }
}