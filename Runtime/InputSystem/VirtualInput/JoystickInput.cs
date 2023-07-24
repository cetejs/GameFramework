using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal abstract class JoystickInput : VirtualInput
    {
        public int joystickNum;
        private Dictionary<string, UsedButton> usedButtons = new Dictionary<string, UsedButton>(8);

        public JoystickInput(int joystickNum)
        {
            this.joystickNum = joystickNum;
        }

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

        protected float GetAxis(string name)
        {
            string axisName = GetAxisName(name);
            return Input.GetAxis(axisName);
        }

        protected float GetAxisRaw(string name)
        {
            string axisName = GetAxisName(name);
            return Input.GetAxisRaw(axisName);
        }

        protected bool GetButton(string name, bool axis)
        {
            if (axis)
            {
                return Mathf.Abs(GetAxisRaw(name)) > 0.0f;
            }

            KeyCode keyCode = GetKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return false;
        }

        protected bool GetButtonDown(string name, bool axis)
        {
            if (axis)
            {
                if (!GetUsedButton(name).CanDown && Mathf.Abs(GetAxisRaw(name)) > 0.0f)
                {
                    GetUsedButton(name).CanDown = true;
                    return true;
                }

                if (GetUsedButton(name).CanDown && Mathf.Abs(GetAxisRaw(name)) <= 0.0f)
                {
                    GetUsedButton(name).CanDown = false;
                }

                return false;
            }

            KeyCode keyCode = GetKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyDown(keyCode);
            }

            return false;
        }

        protected bool GetButtonUp(string name, bool axis)
        {
            if (axis)
            {
                if (GetUsedButton(name).CanUp && Mathf.Abs(GetAxisRaw(name)) <= 0.0f)
                {
                    GetUsedButton(name).CanUp = false;
                    return true;
                }

                if (!GetUsedButton(name).CanUp && Mathf.Abs(GetAxisRaw(name)) > 0.0f)
                {
                    GetUsedButton(name).CanUp = true;
                }

                return false;
            }

            KeyCode keyCode = GetKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyUp(keyCode);
            }

            return false;
        }

        private KeyCode GetKeyCode(string name)
        {
            if (joystickNum == 0)
            {
                return ConvertToKeyCode(StringUtils.Concat("Joystick", name));
            }

            return ConvertToKeyCode(StringUtils.Concat("Joystick", joystickNum, name));
        }

        private string GetAxisName(string name)
        {
            if (joystickNum == 0)
            {
                return StringUtils.Concat("Joystick", " ", name);
            }

            return StringUtils.Concat("Joystick", joystickNum, " ", name);
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