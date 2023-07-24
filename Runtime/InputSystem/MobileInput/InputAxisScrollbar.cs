using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Scrollbar))]
    internal class InputAxisScrollbar : MonoBehaviour
    {
        [SerializeField]
        private string axisName = "Horizontal";
        private Scrollbar scrollbar;
        private InputControl input;

        private void Start()
        {
            scrollbar = GetComponent<Scrollbar>();
            scrollbar.onValueChanged.AddListener(OnValueChanged);

            input = GetComponentInParent<InputControl>();
            if (input != null)
            {
                scrollbar.SetValueWithoutNotify((input.GetAxis(axisName) + 1.0f) / 2.0f);
            }
            else
            {
                scrollbar.SetValueWithoutNotify((InputManager.Instance.GetAxis(axisName) + 1.0f) / 2.0f);
            }
        }

        private void OnValueChanged(float value)
        {
            if (input != null)
            {
                input.SetAxis(axisName, value * 2.0f - 1.0f);
            }
            else
            {
                InputManager.Instance.SetAxis(axisName, value * 2.0f - 1.0f);
            }
        }
    }
}