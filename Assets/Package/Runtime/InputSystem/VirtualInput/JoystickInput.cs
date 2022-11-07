using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.InputService
{
    public class JoystickInput : VirtualInput
    {
        private readonly Dictionary<string, UsedButton> usedButtons = new Dictionary<string, UsedButton>(8);

        public override float GetAxis(InputMapping input)
        {
            return 0.0f;
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return 0.0f;
        }

        public override bool GetButton(InputMapping input)
        {
            return false;
        }

        public override bool GetButtonDown(InputMapping input)
        {
            return false;
        }

        public override bool GetButtonUp(InputMapping input)
        {
            return false;
        }

        public override void SetAxis(string name, float value)
        {
            Debug.LogError($"This axis {name} is not possible to be called for standalone input");
        }

        public override void SetButtonDown(string name)
        {
            Debug.LogError($"This button down {name} is not possible to be called for standalone input");
        }

        public override void SetButtonUp(string name)
        {
            Debug.LogError($"This button up {name} is not possible to be called for standalone input");
        }

        protected float GetAxis(string name, bool isInvert = false)
        {
            if (isInvert)
            {
                return -Input.GetAxis(name);
            }

            return Input.GetAxis(name);
        }

        protected float GetAxisRaw(string name, bool isInvert = false)
        {
            if (isInvert)
            {
                return -Input.GetAxisRaw(name);
            }

            return Input.GetAxisRaw(name);
        }

        protected bool GetButton(string name, bool isAxis)
        {
            if (isAxis)
            {
                return Mathf.Abs(GetAxisRaw(name)) > 0.0f;
            }

            KeyCode keyCode = ConvertToKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return Input.GetButton(name);
        }

        protected bool GetButtonDown(string name, bool isAxis)
        {
            if (isAxis)
            {
                if (!GetUsedButton(name).isUsedDown && Mathf.Abs(GetAxisRaw(name)) > 0.0f)
                {
                    GetUsedButton(name).isUsedDown = true;
                    return true;
                }

                if (GetUsedButton(name).isUsedDown && Mathf.Abs(GetAxisRaw(name)) == 0.0f)
                {
                    GetUsedButton(name).isUsedDown = false;
                }

                return false;
            }

            KeyCode keyCode = ConvertToKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyDown(keyCode);
            }

            return Input.GetButtonDown(name);
        }

        protected bool GetButtonUp(string name, bool isAxis)
        {
            if (isAxis)
            {
                if (GetUsedButton(name).isUsedUp && Mathf.Abs(GetAxisRaw(name)) == 0.0f)
                {
                    GetUsedButton(name).isUsedUp = false;
                    return true;
                }

                if (!GetUsedButton(name).isUsedUp && Mathf.Abs(GetAxisRaw(name)) > 0.0f)
                {
                    GetUsedButton(name).isUsedUp = true;
                }

                return false;
            }

            KeyCode keyCode = ConvertToKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyUp(keyCode);
            }

            return Input.GetButtonUp(name);
        }

        private UsedButton GetUsedButton(string name)
        {
            if (!usedButtons.TryGetValue(name, out UsedButton button))
            {
                button = new UsedButton()
                {
                    buttonName = name
                };

                usedButtons.Add(name, button);
            }

            return button;
        }

        private class UsedButton
        {
            public string buttonName;
            public bool isUsedDown;
            public bool isUsedUp;
        }
    }
}