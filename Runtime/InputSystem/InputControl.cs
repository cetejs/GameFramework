using System;
using UnityEngine;

namespace GameFramework
{
    public class InputControl : MonoBehaviour
    {
        [SerializeField]
        private InputIdentity identity;

        public InputIdentity InputIdentity
        {
            get { return identity; }
            set { identity = value; }
        }

        public float GetAxis(string name)
        {
            return InputManager.Instance.GetAxis(name, identity);
        }

        public float GetAxisRaw(string name)
        {
            return InputManager.Instance.GetAxisRaw(name, identity);
        }

        public bool GetButton(string name)
        {
            return InputManager.Instance.GetButton(name, identity);
        }

        public bool GetButtonDown(string name)
        {
            return InputManager.Instance.GetButtonDown(name, identity);
        }

        public bool GetButtonUp(string name)
        {
            return InputManager.Instance.GetButtonUp(name, identity);
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
            InputManager.Instance.RebindButton(name, bindCode, identity);
        }

        public void ResetButton(string name)
        {
            InputManager.Instance.ResetButton(name, identity);
        }

        private void OnGUI()
        {
            throw new NotImplementedException();
        }
    }

    public enum InputIdentity
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6,
        Player7,
        Player8
    }
}