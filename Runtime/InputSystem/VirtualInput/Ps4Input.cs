using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class Ps4Input : JoystickInput
    {
        private static Dictionary<int, JoystickMapping> ps4Mappings;

        public Ps4Input(int joystickNum) : base(joystickNum)
        {
            CollectMappings();
        }

        public override float GetAxis(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return 0.0f;
            }

            return GetAxis(ps4.Name);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return 0.0f;
            }

            return GetAxisRaw(ps4.Name);
        }

        public override bool GetButton(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return false;
            }

            return GetButton(ps4.Name, ps4.Type == JoystickType.Axis);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return false;
            }

            return GetButtonDown(ps4.Name, ps4.Type == JoystickType.Axis);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return false;
            }

            return GetButtonUp(ps4.Name, ps4.Type == JoystickType.Axis);
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

        private JoystickMapping GetPs4Mapping(Ps4Code code)
        {
            if (ps4Mappings.TryGetValue((int) code, out JoystickMapping ps4))
            {
                return ps4;
            }

            Debug.LogError($"Ps4Mapping is not exist key {code}");
            return null;
        }

        private void CollectMappings()
        {
            if (ps4Mappings != null)
            {
                return;
            }

            ps4Mappings = new Dictionary<int, JoystickMapping>(32);
            foreach (JoystickMapping mapping in InputSetting.Instance.JoystickMappings)
            {
                if (mapping.Ps4Code != Ps4Code.Node)
                {
                    ps4Mappings.Add((int) mapping.Ps4Code, mapping);
                }
            }
        }
    }
}