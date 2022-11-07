using GameFramework.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.InputService
{
    [RequireComponent(typeof(Slider))]
    public class InputAxisSlider : MonoBehaviour
    {
        [SerializeField]
        private string axisName = "Horizontal";

        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.minValue = -slider.maxValue;
            slider.value = Global.GetService<InputManager>().GetAxis(axisName);
            slider.onValueChanged.AddListener(OnValueChanged);
        }
        
        private void OnDisable()
        {
            if (!Global.IsApplicationQuitting)
            {
                Global.GetService<InputManager>().SetAxis(axisName, 0.0f);
            }
        }

        private void OnValueChanged(float value)
        {
            Global.GetService<InputManager>().SetAxis(axisName, value);
        }
    }
}