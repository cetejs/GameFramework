using GameFramework.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.DevConsoleService
{
    internal class DevExtendHub : MonoBehaviour
    {
        [SerializeField]
        private InputField rateInput;
        private Text rateHolder;

        private void Awake()
        {
            rateInput.onSubmit.AddListener(ChangeFrameRate);
            rateHolder = rateInput.placeholder.GetComponent<Text>();
        }

        private void Start()
        {
            ResetRateDrawer();
        }

        private void ResetRateDrawer()
        {
            rateInput.text = string.Empty;
            rateHolder.text = $"Frame Rate : {Application.targetFrameRate}";
        }

        private void ChangeFrameRate(string text)
        {
            if (!int.TryParse(text, out int rate))
            {
                GameLogger.LogError($"Change frame rate failed, {text} is not int value");
                return;
            }

            Application.targetFrameRate = rate;
            ResetRateDrawer();
        }
    }
}