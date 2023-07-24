using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
        private InputControl input;

        private void Start()
        {
            if (thumb == null)
            {
                thumb = transform;
            }

            startPos = thumb.position;
            input = GetComponentInParent<InputControl>();
        }

        private void UpdateAxis(Vector2 value)
        {
            isUseHorizontal = axisType == AxisOption.Both || axisType == AxisOption.OnlyHorizontal;
            isUseVertical = axisType == AxisOption.Both || axisType == AxisOption.OnlyVertical;
            Vector2 delta = value - startPos;
            delta /= movementRange;
            if (isUseHorizontal)
            {
                if (input != null)
                {
                    input.SetAxis(horizontalAxisName, delta.x);
                }
                else
                {
                    InputManager.Instance.SetAxis(horizontalAxisName, delta.x);
                }
            }

            if (isUseVertical)
            {
                if (input != null)
                {
                    input.SetAxis(verticalAxisName, delta.y);
                }
                else
                {
                    InputManager.Instance.SetAxis(verticalAxisName, delta.y);
                }
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
                newPos = Vector2.ClampMagnitude(delta, movementRange);
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