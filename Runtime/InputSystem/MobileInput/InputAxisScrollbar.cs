using GameFramework.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.InputService
{
    [RequireComponent(typeof(Scrollbar))]
    public class InputAxisScrollbar : MonoBehaviour
    {
        [SerializeField]
        private string axisName = "Horizontal";

        private Scrollbar scrollbar;

        private void Awake()
        {
            scrollbar = GetComponent<Scrollbar>();
            scrollbar.value = (Global.GetService<InputManager>().GetAxis(axisName) + 1.0f) / 2.0f;
            scrollbar.onValueChanged.AddListener(OnValueChanged);
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
            Global.GetService<InputManager>().SetAxis(axisName, value * 2.0f - 1.0f);
        }
    }
}