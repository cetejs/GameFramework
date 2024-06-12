using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevLogCell : UICell
    {
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private TextMeshProUGUI text;

        private DevLogView logView;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (logView && isOn)
                {
                    logView.OnLogCellClicked(Index);
                }
            });
        }

        protected override void OnWakeUp()
        {
            if (toggle.group == null)
            {
                toggle.group = GetComponentInParent<ToggleGroup>();
            }
        }

        public void SetData(DevLogView logView, string text, bool isOn)
        {
            this.logView = logView;
            this.text.text = text;
            toggle.isOn = isOn;
        }
    }
}