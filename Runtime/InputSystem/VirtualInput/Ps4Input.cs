using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.InputService
{
    public class Ps4Input : JoystickInput
    {
        private readonly InputData inputData;
        private readonly Dictionary<int, JoystickMapping> ps4Mappings = new Dictionary<int, JoystickMapping>(32);

        public Ps4Input(InputData inputData)
        {
            this.inputData = inputData;
            CollectPs4Mapping();
        }

        public override float GetAxis(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return 0.0f;
            }

            return GetAxis(ps4.joystick, ps4.isInvert);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return 0.0f;
            }

            return GetAxisRaw(ps4.joystick, ps4.isInvert);
        }

        public override bool GetButton(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return false;
            }

            return GetButton(ps4.joystick, ps4.isAxis);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return false;
            }

            return GetButtonDown(ps4.joystick, ps4.isAxis);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return false;
            }

            return GetButtonUp(ps4.joystick, ps4.isAxis);
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

        private JoystickMapping GetPs4Mapping(JoystickPs4 key)
        {
            if (ps4Mappings.TryGetValue((int)key, out JoystickMapping ps4))
            {
                return ps4;
            }

            Debug.LogError($"Ps4Mapping is not exist key {key}");
            return null;
        }

        private void CollectPs4Mapping()
        {
            inputData.ForeachJoystickMappings(joystick =>
            {
                if (joystick.ps4 == JoystickPs4.None)
                {
                    return;
                }

                JoystickMapping mapping = joystick.Clone();

                if (mapping.ps4 == JoystickPs4.RightStickVertical)
                {
                    mapping.isInvert = !mapping.isInvert;
                }

                ps4Mappings.Add((int)joystick.ps4, mapping);
            });
        }
    }
}