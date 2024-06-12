using TMPro;
using UnityEngine;

namespace GameFramework
{
    internal class DevExtendHub : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField rateInput;
        [SerializeField]
        private TMP_InputField scaleInput;
        private TextMeshProUGUI rateHolder;
        private TextMeshProUGUI scaleHolder;

#if ENABLE_CONSOLE
        private void Awake()
        {
            rateInput.onSubmit.AddListener(ChangeFrameRate);
            rateHolder = rateInput.placeholder.GetComponent<TextMeshProUGUI>();

            scaleInput.onSubmit.AddListener(ChangeTimeScale);
            scaleHolder = scaleInput.placeholder.GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            ResetRateDrawer();
            ResetScaleDrawer();
        }

        private void ResetRateDrawer()
        {
            rateInput.text = string.Empty;
            rateHolder.text = $"Frame Rate : {Application.targetFrameRate}";
        }

        private void ResetScaleDrawer()
        {
            scaleInput.text = string.Empty;
            scaleHolder.text = $"Time Scale : {DevConsole.Instance.TimeScale}";
        }

        private void ChangeFrameRate(string text)
        {
            if (!int.TryParse(text, out int rate))
            {
                GameLogger.LogError($"Change frame rate failed, {text} is not int value");
            }
            else
            {
                Application.targetFrameRate = rate;
            }

            ResetRateDrawer();
        }

        private void ChangeTimeScale(string text)
        {
            if (!float.TryParse(text, out float scale))
            {
                GameLogger.LogError($"Change time scale failed, {text} is not float value");
            }
            else
            {
                DevConsole.Instance.TimeScale = scale;
            }

            ResetScaleDrawer();
        }
#endif
    }
}