using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class XboxInput : JoystickInput
    {
        private Dictionary<int, JoystickMapping> xboxMappings = new Dictionary<int, JoystickMapping>(32);

        public XboxInput()
        {
            CollectXboxMapping();
        }

        public override float GetAxis(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return 0.0f;
            }

            return GetAxis(xbox.Name, (int) input.JoystickNum);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return 0.0f;
            }

            return GetAxisRaw(xbox.Name, (int) input.JoystickNum);
        }

        public override bool GetButton(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return false;
            }

            return GetButton(xbox.Name, (int) input.JoystickNum, xbox.Type == JoystickType.Axis);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return false;
            }

            return GetButtonDown(xbox.Name, (int) input.JoystickNum, xbox.Type == JoystickType.Axis);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.XboxCode);
            if (xbox == null)
            {
                return false;
            }

            return GetButtonUp(xbox.Name, (int) input.JoystickNum, xbox.Type == JoystickType.Axis);
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
            if (xboxMappings.TryGetValue((int) code, out JoystickMapping xbox))
            {
                return xbox;
            }

            Debug.LogError($"XboxMapping is not exist key {code}");
            return null;
        }

        private void CollectXboxMapping()
        {
            foreach (JoystickMapping mapping in InputSetting.Instance.JoystickMappings)
            {
                if (mapping.XboxCode != XboxCode.Node)
                {
                    xboxMappings.Add((int) mapping.XboxCode, mapping);
                }
            }
        }
    }
}