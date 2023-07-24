using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Slider))]
    internal class InputAxisSlider : MonoBehaviour
    {
        [SerializeField]
        private string axisName = "Horizontal";
        private InputControl input;

        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
            slider.minValue = -slider.maxValue;
            slider.onValueChanged.AddListener(OnValueChanged);

            input = GetComponentInParent<InputControl>();
            if (input != null)
            {
                slider.SetValueWithoutNotify(input.GetAxis(axisName));
            }
            else
            {
                slider.SetValueWithoutNotify(InputManager.Instance.GetAxis(axisName));
            }
        }

        private void OnValueChanged(float value)
        {
            if (input != null)
            {
                input.SetAxis(axisName, value);
            }
            else
            {
                InputManager.Instance.SetAxis(axisName, value);
            }
        }
    }
}