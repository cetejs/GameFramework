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

        private void Awake()
        {
            scrollbar = GetComponent<Scrollbar>();
            scrollbar.value = (InputManager.Instance.GetAxis(axisName) + 1.0f) / 2.0f;
            scrollbar.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            InputManager.Instance.SetAxis(axisName, value * 2.0f - 1.0f);
        }
    }
}