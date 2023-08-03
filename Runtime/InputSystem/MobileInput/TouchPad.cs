using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class TouchPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private AxisOption axisType = AxisOption.Both;
        [SerializeField]
        private string horizontalAxisName = "Mouse X";
        [SerializeField]
        private string verticalAxisName = "Mouse Y";
        [SerializeField]
        private float responseSpeed = 1f;

        private bool useHorizontal;
        private bool useVertical;
        private bool dragging;
        private Vector2 currentPos;
        private Vector2 previousPos;
        private InputControl input;

        private void Start()
        {
            input = GetComponentInParent<InputControl>();
        }

        private void Update()
        {
            if (dragging)
            {
                Vector2 delta = currentPos - previousPos;
                UpdateAxis(delta);
                previousPos = currentPos;
            }
        }

        private void UpdateAxis(Vector2 value)
        {
            useHorizontal = axisType == AxisOption.Both || axisType == AxisOption.OnlyHorizontal;
            useVertical = axisType == AxisOption.Both || axisType == AxisOption.OnlyVertical;

            Vector2 delta = value.normalized;
            if (useHorizontal)
            {
                if (input != null)
                {
                    input.SetAxis(horizontalAxisName, delta.x * responseSpeed);
                }
                else
                {
                    InputManager.Instance.SetAxis(horizontalAxisName, delta.x * responseSpeed);
                }
            }

            if (useVertical)
            {
                if (input != null)
                {
                    input.SetAxis(verticalAxisName, delta.y * responseSpeed);
                }
                else
                {
                    InputManager.Instance.SetAxis(verticalAxisName, delta.y * responseSpeed);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            previousPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            currentPos = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            UpdateAxis(Vector2.zero);
        }

        private enum AxisOption : byte
        {
            Both,
            OnlyHorizontal,
            OnlyVertical
        }
    }
}