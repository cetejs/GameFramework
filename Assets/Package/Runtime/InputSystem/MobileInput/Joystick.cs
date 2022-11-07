using GameFramework.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.InputService
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private Transform thumb;
        [SerializeField]
        private AxisOption axisType = AxisOption.Both;
        [SerializeField]
        private string horizontalAxisName = "Horizontal";
        [SerializeField]
        private string verticalAxisName = "Vertical";
        [SerializeField]
        private int movementRange = 100;

        private bool isUseHorizontal;
        private bool isUseVertical;
        private Vector2 startPos;

        private void Start()
        {
            if (thumb == null)
            {
                thumb = transform;
            }

            startPos = thumb.position;
        }

        private void OnDisable()
        {
            if (!Global.IsApplicationQuitting)
            {
                thumb.position = startPos;
                UpdateAxis(startPos);
            }
        }

        private void UpdateAxis(Vector2 value)
        {
            isUseHorizontal = axisType == AxisOption.Both || axisType == AxisOption.OnlyHorizontal;
            isUseVertical = axisType == AxisOption.Both || axisType == AxisOption.OnlyVertical;
            Vector2 delta = value - startPos;
            delta /= movementRange;
            if (isUseHorizontal)
            {
                Global.GetService<InputManager>().SetAxis(horizontalAxisName, delta.x);
            }

            if (isUseVertical)
            {
                Global.GetService<InputManager>().SetAxis(verticalAxisName, delta.y);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 newPos = Vector2.zero;
            if (axisType == AxisOption.OnlyHorizontal)
            {
                int delta = (int) (eventData.position.x - startPos.x);
                delta = Mathf.Clamp(delta, -movementRange, movementRange);
                newPos.x = delta;
            }
            else if (axisType == AxisOption.OnlyVertical)
            {
                int delta = (int) (eventData.position.y - startPos.y);
                delta = Mathf.Clamp(delta, -movementRange, movementRange);
                newPos.y = delta;
            }
            else
            {
                Vector2 delta = eventData.position - startPos;
                if (delta.sqrMagnitude > movementRange * movementRange)
                {
                    newPos = delta.normalized * movementRange;
                }
                else
                {
                    newPos = delta;
                }
            }

            thumb.position = startPos + newPos;
            UpdateAxis(thumb.position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            thumb.position = startPos;
            UpdateAxis(startPos);
        }

        private enum AxisOption : byte
        {
            Both,
            OnlyHorizontal,
            OnlyVertical
        }
    }
}