using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.InputService
{
    public class XboxInput : JoystickInput
    {
        private readonly InputData inputData;
        private readonly Dictionary<int, JoystickMapping> xboxMappings = new Dictionary<int, JoystickMapping>(32);

        public XboxInput(InputData inputData)
        {
            this.inputData = inputData;
            CollectXboxMapping();
        }

        public override float GetAxis(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return 0.0f;
            }

            return GetAxis(xbox.joystick, xbox.isInvert);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return 0.0f;
            }

            return GetAxisRaw(xbox.joystick, xbox.isInvert);
        }

        public override bool GetButton(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return false;
            }

            return GetButton(xbox.joystick, xbox.isAxis);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return false;
            }

            return GetButtonDown(xbox.joystick, xbox.isAxis);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            JoystickMapping xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return false;
            }

            return GetButtonUp(xbox.joystick, xbox.isAxis);
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

        private JoystickMapping GetXboxMapping(JoystickXbox key)
        {
            if (xboxMappings.TryGetValue((int)key, out JoystickMapping xbox))
            {
                return xbox;
            }

            Debug.LogError($"XboxMapping is not exist key {key}");
            return null;
        }

        private void CollectXboxMapping()
        {
            inputData.ForeachJoystickMappings(joystick =>
            {
                if (joystick.xbox == JoystickXbox.None)
                {
                    return;
                }

                JoystickMapping mapping = joystick.Clone();

                if (mapping.xbox == JoystickXbox.RightStickVertical)
                {
                    mapping.isInvert = !mapping.isInvert;
                }

                xboxMappings.Add((int)joystick.xbox, mapping);
            });
        }
    }
}