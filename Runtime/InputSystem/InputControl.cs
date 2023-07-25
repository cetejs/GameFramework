using UnityEngine;

namespace GameFramework
{
    public class InputControl : MonoBehaviour
    {
        [SerializeField]
        private InputIdentity identity;
        [SerializeField]
        [InputSettingName]
        private string inputSettingName = GameSettings.Instance.DefaultInputSettingName;
        private InputSetting inputSetting;

        public InputIdentity InputIdentity
        {
            get { return identity; }
            set { identity = value; }
        }

        public string InputSettingName
        {
            get { return inputSettingName; }
            set { inputSettingName = value; }
        }

        private void Awake()
        {
            string assetPath = PathUtils.Combine(GameSettings.Instance.InputSettingAssetName, inputSettingName);
            inputSetting = AssetManager.Instance.LoadAsset<InputSetting>(assetPath);
        }

        public float GetAxis(string name)
        {
            return InputManager.Instance.GetAxis(name, identity, inputSetting);
        }

        public float GetAxisRaw(string name)
        {
            return InputManager.Instance.GetAxisRaw(name, identity, inputSetting);
        }

        public bool GetButton(string name)
        {
            return InputManager.Instance.GetButton(name, identity, inputSetting);
        }

        public bool GetButtonDown(string name)
        {
            return InputManager.Instance.GetButtonDown(name, identity, inputSetting);
        }

        public bool GetButtonUp(string name)
        {
            return InputManager.Instance.GetButtonUp(name, identity, inputSetting);
        }

        public void SetAxisPositive(string name)
        {
            InputManager.Instance.SetAxisPositive(name, identity);
        }

        public void SetAxisNegative(string name)
        {
            InputManager.Instance.SetAxisNegative(name, identity);
        }

        public void SetAxisZero(string name)
        {
            InputManager.Instance.SetAxisZero(name, identity);
        }

        public void SetAxis(string name, float value)
        {
            InputManager.Instance.SetAxis(name, value, identity);
        }

        public void SetButtonDown(string name)
        {
            InputManager.Instance.SetButtonDown(name, identity);
        }

        public void SetButtonUp(string name)
        {
            InputManager.Instance.SetButtonUp(name, identity);
        }

        public void RebindButton(string name, int bindCode)
        {
            InputManager.Instance.RebindButton(name, bindCode, identity, inputSetting);
        }

        public void ResetButton(string name)
        {
            InputManager.Instance.ResetButton(name, identity, inputSetting);
        }
    }
}