using System;
using UnityEngine;

namespace GameFramework
{
    public class InputControl : MonoBehaviour
    {
        [SerializeField]
        private InputIdentity identity;
        [SerializeField]
        private InputSetting inputSetting;

        public InputIdentity InputIdentity
        {
            get { return identity; }
            set { identity = value; }
        }

        public InputSetting InputSetting
        {
            get { return inputSetting; }
            set
            {
                inputSetting = value;
                if (inputSetting != null)
                {
                    inputSetting.CollectInputMappings();
                }
            }
        }

        public InputDevice InputDevice
        {
            get { return InputManager.Instance.GetInputDevice(identity); }
        }

        private void Awake()
        {
            if (inputSetting != null)
            {
                inputSetting.CollectInputMappings();
            }
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

        public void ListenRebindInput(Action<InputDevice, int> callback)
        {
            InputManager.Instance.ListenRebindInput(callback, identity);
        }

        public void CancelRebindListening()
        {
            InputManager.Instance.CancelRebindListening();
        }

        public void ListenDeviceInput()
        {
            InputManager.Instance.ListenDeviceInput(identity);
        }

        public void CancelDeviceListening()
        {
            InputManager.Instance.CancelDeviceListening();
        }
    }
}