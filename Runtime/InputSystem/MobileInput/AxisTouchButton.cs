using GameFramework.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.InputService
{
    public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private AxisOption axisType = AxisOption.Positive;
        [SerializeField]
        private string axisName = "Horizontal";
        [SerializeField]
        private float responseSpeed = 1.0f;

        private bool isPointerDown;
        private float axisValue;

        private void OnDisable()
        {
            if (!Global.IsApplicationQuitting)
            {
                UpdateAxis(0.0f);
            }
        }

        private void Update()
        {
            if (isPointerDown)
            {
                if (axisType == AxisOption.Negative)
                {
                    UpdateAxis(-1.0f);
                }
                else if (axisType == AxisOption.Positive)
                {
                    UpdateAxis(1.0f);
                }
            }
        }

        private void UpdateAxis(float value)
        {
            if (!Mathf.Approximately(axisValue, value))
            {
                axisValue = Mathf.MoveTowards(axisValue, value, responseSpeed * Time.deltaTime);
                Global.GetService<InputManager>().SetAxis(axisName, axisValue);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            axisValue = 0.0f;
            Global.GetService<InputManager>().SetAxisZero(axisName);
        }

        private enum AxisOption
        {
            Positive,
            Negative
        }
    }
}