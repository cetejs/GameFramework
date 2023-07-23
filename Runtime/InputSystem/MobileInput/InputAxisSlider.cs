using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Slider))]
    internal class InputAxisSlider : MonoBehaviour
    {
        [SerializeField]
        private string axisName = "Horizontal";

        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.minValue = -slider.maxValue;
            slider.value = InputManager.Instance.GetAxis(axisName);
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            InputManager.Instance.SetAxis(axisName, value);
        }
    }
}