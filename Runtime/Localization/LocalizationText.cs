using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationText : MonoBehaviour
    {
        [SerializeField]
        private string languageKey;
        private TextMeshProUGUI text;

        public string LanguageKey
        {
            get { return languageKey; }
        }

        public TextMeshProUGUI Text
        {
            get
            {
                if (text == null)
                {
                    text = GetComponent<TextMeshProUGUI>();
                }

                return text;
            }
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(languageKey))
            {
                GameLogger.LogError($"Localization text language key is invalid, {name}");
                return;
            }

            SetLanguage(LocalizationManager.Instance.GetLanguage(languageKey));
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(string type)
        {
            SetLanguage(LocalizationManager.Instance.GetLanguage(languageKey));
        }

        public void SetLanguage(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Text.text = value;
                Text.Rebuild(CanvasUpdate.PreRender);
            }
        }
    }
}