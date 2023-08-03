using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class XboxInput : JoystickInput
    {
        [RuntimeReload]
        private static Dictionary<int, JoystickMapping> xboxMappings;

        public XboxInput(int joystickNum) : base(joystickNum)
        {
            CollectMappings();
        }

        public override float GetAxis(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return 0f;
            }

            if (xbox.Type == JoystickType.Button)
            {
                return 0f;
            }

            if (xbox.XboxCode == XboxCode.LeftStickY || xbox.XboxCode == XboxCode.RightStickY)
            {
                return -GetAxis(xbox.Name);
            }

            return GetAxis(xbox.Name);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return 0f;
            }

            if (xbox.Type == JoystickType.Button)
            {
                return 0f;
            }

            if (xbox.XboxCode == XboxCode.LeftStickY || xbox.XboxCode == XboxCode.RightStickY)
            {
                return -GetAxisRaw(xbox.Name);
            }

            return GetAxisRaw(xbox.Name);
        }

        public override bool GetButton(InputMapping input)
        {
            if (input.XboxCode == XboxCode.DPadUp)
            {
                return GetAxisRaw(GetXboxMapping(XboxCode.DPadY).Name) > 0f;
            }

            if (input.XboxCode == XboxCode.DPadDown)
            {
                return GetAxisRaw(GetXboxMapping(XboxCode.DPadY).Name) < 0f;
            }

            if (input.XboxCode == XboxCode.DPadLeft)
            {
                return GetAxisRaw(GetXboxMapping(XboxCode.DPadX).Name) < 0f;
            }

            if (input.XboxCode == XboxCode.DPadRight)
            {
                return GetAxisRaw(GetXboxMapping(XboxCode.DPadX).Name) > 0f;
            }

            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return false;
            }

            if (xbox.Type == JoystickType.Axis)
            {
                return Mathf.Abs(GetAxisRaw(xbox.Name)) > 0f;
            }

            return GetButton(xbox.Name);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            if (input.XboxCode == XboxCode.DPadUp || input.XboxCode == XboxCode.DPadDown ||
                input.XboxCode == XboxCode.DPadLeft || input.XboxCode == XboxCode.DPadRight)
            {
                return GetAxisDown(input, (int) input.XboxCode);
            }

            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return false;
            }

            if (xbox.Type == JoystickType.Axis)
            {
                return GetAxisDown(input, (int) input.XboxCode);
            }

            return GetButtonDown(xbox.Name);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            if (input.XboxCode == XboxCode.DPadUp || input.XboxCode == XboxCode.DPadDown ||
                input.XboxCode == XboxCode.DPadLeft || input.XboxCode == XboxCode.DPadRight)
            {
                return GetAxisUp(input, (int) input.XboxCode);
            }

            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return false;
            }

            if (xbox.Type == JoystickType.Axis)
            {
                return GetAxisUp(input, (int) input.XboxCode);
            }

            return GetButtonUp(xbox.Name);
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

        private JoystickMapping GetXboxMapping(XboxCode code)
        {
            if (code == XboxCode.None)
            {
                return null;
            }

            if (xboxMappings.TryGetValue((int) code, out JoystickMapping xbox))
            {
                return xbox;
            }

            Debug.LogError($"XboxMapping is not exist key {code}");
            return null;
        }

        private void CollectMappings()
        {
            if (xboxMappings != null)
            {
                return;
            }

            xboxMappings = new Dictionary<int, JoystickMapping>(32);
            foreach (JoystickMapping mapping in JoystickMapping.Mappings)
            {
                if (mapping.XboxCode != XboxCode.None)
                {
                    xboxMappings.Add((int) mapping.XboxCode, mapping);
                }
            }
        }
    }
}