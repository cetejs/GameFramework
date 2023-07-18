using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevLogCell : UICell
    {
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private Text text;

        private DevLogView logView;

        private void Awake()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (logView && isOn)
                {
                    logView.OnLogCellClicked(Index);
                }
            });
        }

        public void SetData(DevLogView logView, string text, bool isOn)
        {
            this.logView = logView;
            this.text.text = text;
            toggle.isOn = isOn;
        }
    }
}