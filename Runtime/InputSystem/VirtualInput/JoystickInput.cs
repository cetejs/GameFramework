using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class JoystickInput : VirtualInput
    {
        private Dictionary<string, UsedButton> usedButtons = new Dictionary<string, UsedButton>(8);

        public override float GetAxis(InputMapping input)
        {
            return 0f;
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return 0f;
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
        }

        public override void SetButtonDown(string name)
        {
        }

        public override void SetButtonUp(string name)
        {
        }

        protected float GetAxis(string name, int num)
        {
            string axisName = GetAxisName(name, num);
            return Input.GetAxis(axisName);
        }

        protected float GetAxisRaw(string name, int num)
        {
            string axisName = GetAxisName(name, num);
            return Input.GetAxisRaw(axisName);
        }

        protected bool GetButton(string name, int num, bool axis)
        {
            if (axis)
            {
                return Mathf.Abs(GetAxisRaw(name, num)) > 0.0f;
            }

            KeyCode keyCode = GetKeyCode(name, num);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return false;
        }

        protected bool GetButtonDown(string name, int num, bool axis)
        {
            if (axis)
            {
                if (!GetUsedButton(name).CanDown && Mathf.Abs(GetAxisRaw(name, num)) > 0.0f)
                {
                    GetUsedButton(name).CanDown = true;
                    return true;
                }

                if (GetUsedButton(name).CanDown && Mathf.Abs(GetAxisRaw(name, num)) <= 0.0f)
                {
                    GetUsedButton(name).CanDown = false;
                }

                return false;
            }

            KeyCode keyCode = GetKeyCode(name, num);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyDown(keyCode);
            }

            return false;
        }

        protected bool GetButtonUp(string name, int num, bool axis)
        {
            if (axis)
            {
                if (GetUsedButton(name).CanUp && Mathf.Abs(GetAxisRaw(name, num)) <= 0.0f)
                {
                    GetUsedButton(name).CanUp = false;
                    return true;
                }

                if (!GetUsedButton(name).CanUp && Mathf.Abs(GetAxisRaw(name, num)) > 0.0f)
                {
                    GetUsedButton(name).CanUp = true;
                }

                return false;
            }

            KeyCode keyCode = GetKeyCode(name, num);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyUp(keyCode);
            }

            return false;
        }

        private KeyCode GetKeyCode(string name, int num)
        {
            if (num == 0)
            {
                return ConvertToKeyCode(StringUtils.Concat("Joystick", name));
            }

            return ConvertToKeyCode(StringUtils.Concat("Joystick", num, name));
        }

        private string GetAxisName(string name, int num)
        {
            if (num == 0)
            {
                return StringUtils.Concat("Joystick", " ", name);
            }

            return StringUtils.Concat("Joystick", num, " ", name);
        }

        private UsedButton GetUsedButton(string name)
        {
            if (!usedButtons.TryGetValue(name, out UsedButton button))
            {
                button = new UsedButton()
                {
                    Name = name
                };

                usedButtons.Add(name, button);
            }

            return button;
        }

        private class UsedButton
        {
            public string Name;
            public bool CanDown;
            public bool CanUp;
        }
    }
}