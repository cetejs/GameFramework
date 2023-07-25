using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal abstract class JoystickInput : VirtualInput
    {
        private Dictionary<int, UsedButton> usedButtons = new Dictionary<int, UsedButton>(8);

        public int JoystickNum { get; private set; }

        public JoystickInput(int joystickNum)
        {
            JoystickNum = joystickNum;
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
            return Input.GetAxis(GetAxisName(name));
        }

        protected float GetAxisRaw(string name)
        {
            return Input.GetAxisRaw(GetAxisName(name));
        }

        protected bool GetButton(string name)
        {
            KeyCode keyCode = GetKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return false;
        }

        protected bool GetButtonDown(string name)
        {
            KeyCode keyCode = GetKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyDown(keyCode);
            }

            return false;
        }

        protected bool GetButtonUp(string name)
        {
            KeyCode keyCode = GetKeyCode(name);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyUp(keyCode);
            }

            return false;
        }

        protected bool GetAxisDown(InputMapping input)
        {
            UsedButton usedButton = GetUsedButton((int) input.KeyCode);
            if (!usedButton.CanDown && GetButton(input))
            {
                usedButton.CanDown = true;
                return true;
            }

            if (usedButton.CanDown && !GetButton(input))
            {
                usedButton.CanDown = false;
            }

            return false;
        }

        protected bool GetAxisUp(InputMapping input)
        {
            UsedButton usedButton = GetUsedButton((int) input.KeyCode);
            if (usedButton.CanUp && !GetButton(input))
            {
                usedButton.CanUp = true;
                return true;
            }

            if (!usedButton.CanUp && GetButton(input))
            {
                usedButton.CanUp = false;
            }

            return false;
        }

        protected KeyCode GetKeyCode(string name)
        {
            if (JoystickNum == 0)
            {
                return ConvertToKeyCode(StringUtils.Concat("Joystick", name));
            }

            return ConvertToKeyCode(StringUtils.Concat("Joystick", JoystickNum, name));
        }

        protected string GetAxisName(string name)
        {
            if (JoystickNum == 0)
            {
                return StringUtils.Concat("Joystick", " ", name);
            }

            return StringUtils.Concat("Joystick", JoystickNum, " ", name);
        }

        protected UsedButton GetUsedButton(int buttonCode)
        {
            if (!usedButtons.TryGetValue(buttonCode, out UsedButton button))
            {
                button = new UsedButton()
                {
                    ButtonCode = buttonCode
                };

                usedButtons.Add(buttonCode, button);
            }

            return button;
        }

        protected class UsedButton
        {
            public int ButtonCode;
            public bool CanDown;
            public bool CanUp;
        }
    }
}