using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    [CreateAssetMenu(menuName = "Configs/InputSetting")]
    public class InputSetting : ScriptableObject
    {
        [SerializeField]
        private List<InputMapping> InputMappings = new List<InputMapping>()
        {
            new InputMapping()
            {
                Name = "Horizontal",
                Description = "Move",
                KeyCode = MouseKeyCode.Horizontal,
                XboxCode = XboxCode.LeftStickX,
                Ps4Code = Ps4Code.LeftStickX
            },
            new InputMapping()
            {
                Name = "Vertical",
                Description = "Move",
                KeyCode = MouseKeyCode.Horizontal,
                XboxCode = XboxCode.LeftStickY,
                Ps4Code = Ps4Code.LeftStickY
            },
            new InputMapping()
            {
                Name = "Mouse X",
                Description = "RotateView",
                KeyCode = MouseKeyCode.MouseX,
                XboxCode = XboxCode.RightStickX,
                Ps4Code = Ps4Code.RightStickX
            },
            new InputMapping()
            {
                Name = "Mouse Y",
                Description = "RotateView",
                KeyCode = MouseKeyCode.MouseY,
                XboxCode = XboxCode.RightStickY,
                Ps4Code = Ps4Code.RightStickY
            },
            new InputMapping()
            {
                Name = "Horizontal Nav",
                Description = "Navigation",
                KeyCode = MouseKeyCode.HorizontalArrow,
                XboxCode = XboxCode.DPadX,
                Ps4Code = Ps4Code.DPadX
            },
            new InputMapping()
            {
                Name = "Vertical Nav",
                Description = "Navigation",
                KeyCode = MouseKeyCode.VerticalArrow,
                XboxCode = XboxCode.DPadY,
                Ps4Code = Ps4Code.DPadY
            },
            new InputMapping()
            {
                Name = "Submit",
                Description = "Submit",
                KeyCode = MouseKeyCode.Return,
                XboxCode = XboxCode.A,
                Ps4Code = Ps4Code.Circle
            },
            new InputMapping()
            {
                Name = "Cancel",
                Description = "Cancel",
                KeyCode = MouseKeyCode.Return,
                XboxCode = XboxCode.B,
                Ps4Code = Ps4Code.Cross
            }
        };

        private Dictionary<string, InputMapping> inputMappings = new Dictionary<string, InputMapping>();
        private Dictionary<string, InputMapping> boundMappings = new Dictionary<string, InputMapping>();

        private void OnEnable()
        {
            CollectInputMappings();
        }

        internal int GetInputDeviceMapping(InputMapping input, InputDevice device)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    return (int) input.KeyCode;
                case InputDevice.XboxGamepad:
                    return (int) input.KeyCode;
                case InputDevice.Ps4Gamepad:
                    return (int) input.KeyCode;
            }

            return 0;
        }

        internal void SetInputDeviceMapping(InputMapping input, InputDevice device, InputMapping value)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    input.KeyCode = value.KeyCode;
                    break;
                case InputDevice.XboxGamepad:
                    input.XboxCode = value.XboxCode;
                    break;
                case InputDevice.Ps4Gamepad:
                    input.Ps4Code = value.Ps4Code;
                    break;
            }
        }

        internal void SetInputDeviceMapping(InputMapping input, InputDevice device, int value)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    input.KeyCode = (MouseKeyCode) value;
                    break;
                case InputDevice.XboxGamepad:
                    input.XboxCode = (XboxCode) value;
                    break;
                case InputDevice.Ps4Gamepad:
                    input.Ps4Code = (Ps4Code) value;
                    break;
            }
        }

        internal InputMapping GetInputMapping(string name)
        {
            if (inputMappings.TryGetValue(name, out InputMapping input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
        }

        internal InputMapping GetBoundMapping(string name, InputIdentity identity)
        {
            string boundName = StringUtils.Concat(name, (int) identity);
            if (boundMappings.TryGetValue(boundName, out InputMapping bound))
            {
                return bound;
            }

            if (inputMappings.TryGetValue(name, out InputMapping input))
            {
                bound = new InputMapping()
                {
                    Name = input.Name,
                    KeyCode = (MouseKeyCode) PlayerPrefs.GetInt(StringUtils.Concat(boundName, InputDevice.MouseKeyboard), (int) input.KeyCode),
                    XboxCode = (XboxCode) PlayerPrefs.GetInt(StringUtils.Concat(boundName, InputDevice.XboxGamepad), (int) input.XboxCode),
                    Ps4Code = (Ps4Code) PlayerPrefs.GetInt(StringUtils.Concat(boundName, InputDevice.Ps4Gamepad), (int) input.Ps4Code)
                };

                boundMappings.Add(boundName, bound);
                return bound;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
        }

        internal void RebindButton(string name, int bindCode, InputIdentity identity, InputDevice device)
        {
            string boundName = StringUtils.Concat(name, (int) identity);
            InputMapping input = GetBoundMapping(name, identity);
            if (input != null)
            {
                SetInputDeviceMapping(input, device, bindCode);
                PlayerPrefs.SetInt(StringUtils.Concat(boundName, device), bindCode);
            }
        }

        internal void ResetButton(string name, InputIdentity identity, InputDevice device)
        {
            string boundName = StringUtils.Concat(name, (int) identity);
            InputMapping inputMapping = GetInputMapping(name);
            InputMapping boundMapping = GetBoundMapping(name, identity);
            if (inputMapping == null || boundMapping == null)
            {
                return;
            }

            if (GetInputDeviceMapping(inputMapping, device) == GetInputDeviceMapping(boundMapping, device))
            {
                return;
            }

            SetInputDeviceMapping(boundMapping, device, inputMapping);
            PlayerPrefs.DeleteKey(StringUtils.Concat(boundName, device));
        }

        private void CollectInputMappings()
        {
            inputMappings.Clear();
            foreach (InputMapping input in InputMappings)
            {
                inputMappings.Add(input.Name, input);
            }
        }
    }
}