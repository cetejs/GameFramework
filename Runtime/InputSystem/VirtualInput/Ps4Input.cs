using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class Ps4Input : JoystickInput
    {
        [RuntimeReload]
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
                return 0f;
            }
            
            if ((ps4.Ps4Code == Ps4Code.L2 || ps4.Ps4Code == Ps4Code.R2) && Application.isFocused)
            {
                return (GetAxis(ps4.Name) + 1) / 2f;
            }

            return GetAxis(ps4.Name);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return 0f;
            }

            if ((ps4.Ps4Code == Ps4Code.L2 || ps4.Ps4Code == Ps4Code.R2) && Application.isFocused)
            {
                return (GetAxisRaw(ps4.Name) + 1) / 2f;
            }

            return GetAxisRaw(ps4.Name);
        }

        public override bool GetButton(InputMapping input)
        {
            if (input.Ps4Code == Ps4Code.DPadUp)
            {
                return GetAxisRaw(GetPs4Mapping(Ps4Code.DPadY).Name) > 0f;
            }

            if (input.Ps4Code == Ps4Code.DPadDown)
            {
                return GetAxisRaw(GetPs4Mapping(Ps4Code.DPadY).Name) < 0f;
            }

            if (input.Ps4Code == Ps4Code.DPadLeft)
            {
                return GetAxisRaw(GetPs4Mapping(Ps4Code.DPadX).Name) < 0f;
            }

            if (input.Ps4Code == Ps4Code.DPadRight)
            {
                return GetAxisRaw(GetPs4Mapping(Ps4Code.DPadX).Name) > 0f;
            }

            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return false;
            }

            if (ps4.Type == JoystickType.Axis)
            {
                if ((ps4.Ps4Code == Ps4Code.L2 || ps4.Ps4Code == Ps4Code.R2) && Application.isFocused)
                {
                    return Mathf.Abs(GetAxisRaw(ps4.Name)) > -1f;
                }

                return Mathf.Abs(GetAxisRaw(ps4.Name)) > 0f;
            }

            return GetButton(ps4.Name);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            if (input.Ps4Code == Ps4Code.DPadUp || input.Ps4Code == Ps4Code.DPadDown ||
                input.Ps4Code == Ps4Code.DPadLeft || input.Ps4Code == Ps4Code.DPadRight)
            {
                return GetAxisDown(input);
            }

            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return false;
            }

            if (ps4.Type == JoystickType.Axis)
            {
                return GetAxisDown(input);
            }

            return GetButtonDown(ps4.Name);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            if (input.Ps4Code is Ps4Code.DPadUp or Ps4Code.DPadDown ||
                input.Ps4Code == Ps4Code.DPadLeft || input.Ps4Code == Ps4Code.DPadRight)
            {
                return GetAxisUp(input);
            }

            JoystickMapping ps4 = GetPs4Mapping(input.Ps4Code);
            if (ps4 == null)
            {
                return false;
            }

            if (ps4.Type == JoystickType.Axis)
            {
                return GetAxisUp(input);
            }

            return GetButtonUp(ps4.Name);
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
            foreach (JoystickMapping mapping in JoystickMapping.Mappings)
            {
                if (mapping.Ps4Code != Ps4Code.Node)
                {
                    ps4Mappings.Add((int) mapping.Ps4Code, mapping);
                }
            }
        }
    }
}