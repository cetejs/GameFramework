using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class DevDragger : MonoBehaviour, IDragHandler
    {
        [SerializeField]
        private RectTransform mainPlane;
        [SerializeField]
        private bool isResetPos;
        private Vector3 originPos;

        private void Awake()
        {
            originPos = mainPlane.anchoredPosition;
        }

        private void OnDisable()
        {
            if (isResetPos)
            {
                mainPlane.anchoredPosition = originPos;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = mainPlane.position;
            pos += eventData.delta;
            mainPlane.position = pos;
        }

        public void ResetPosOnEnable(bool isReset)
        {
            isResetPos = isReset;
        }
    }
}