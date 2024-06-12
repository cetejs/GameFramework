using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevLogFilter : MonoBehaviour
    {
        [SerializeField]
        private DevLogView logView;
        [SerializeField]
        private LogType logType;
        [SerializeField]
        private TextMeshProUGUI text;
        private Toggle toggle;
        private string typeColor;

        private void Awake()
        {
            switch (logType)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    typeColor = "<color=#FF0000>E</color>";
                    break;

                case LogType.Warning:
                    typeColor = "<color=#FFFF00>W</color>";
                    break;
                case LogType.Log:
                    typeColor = "I";
                    break;
            }

            text.text = $"{typeColor}({0})";
            toggle = GetComponent<Toggle>();
            logView.OnCountChanged += (type, count) =>
            {
                if (logType == type)
                {
                    string cnt = count > 999 ? "999+" : count.ToString();
                    text.text = $"{typeColor}({cnt})";
                }
            };
            toggle.onValueChanged.AddListener(isOn =>
            {
                logView.SetLogFilter(logType, isOn);
            });
        }

        private void Start()
        {
            logView.SetLogFilter(logType, toggle.isOn);
        }
    }
}