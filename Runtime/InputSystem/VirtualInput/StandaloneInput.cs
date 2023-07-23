using UnityEngine;

namespace GameFramework
{
    internal class StandaloneInput : VirtualInput
    {
        public override float GetAxis(InputMapping input)
        {
            string axisName = GetAxisName(input);
            return Input.GetAxis(axisName);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            string axisName = GetAxisName(input);
            return Input.GetAxisRaw(axisName);
        }

        public override bool GetButton(InputMapping input)
        {
            KeyCode keyCode = GetKeyCode(input);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return false;
        }

        public override bool GetButtonDown(InputMapping input)
        {
            KeyCode keyCode = GetKeyCode(input);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyDown(keyCode);
            }

            return false;
        }

        public override bool GetButtonUp(InputMapping input)
        {
            KeyCode keyCode = GetKeyCode(input);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyUp(keyCode);
            }

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

        private KeyCode GetKeyCode(InputMapping input)
        {
            return (KeyCode) (int) input.KeyCode;
        }

        private string GetAxisName(InputMapping input)
        {
            switch (input.KeyCode)
            {
                case MouseKeyCode.MouseX:
                    return "Mouse X";
                case MouseKeyCode.MouseY:
                    return "Mouse Y";
                case MouseKeyCode.MouseScrollWheel:
                    return "Mouse ScrollWheel";
            }

            return input.KeyCode.ToString();
        }
    }
}