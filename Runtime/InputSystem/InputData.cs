using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class InputData
    {
        private Dictionary<string, InputMapping> inputMappings = new Dictionary<string, InputMapping>();
        private Dictionary<string, InputMapping> boundMappings = new Dictionary<string, InputMapping>();
        private Dictionary<string, JoystickMapping> joystickMappings = new Dictionary<string, JoystickMapping>();

        public readonly List<KeyCode> JoystickButtons = new List<KeyCode>()
        {
            KeyCode.JoystickButton0,
            KeyCode.JoystickButton1,
            KeyCode.JoystickButton2,
            KeyCode.JoystickButton3,
            KeyCode.JoystickButton4,
            KeyCode.JoystickButton5,
            KeyCode.JoystickButton6,
            KeyCode.JoystickButton7,
            KeyCode.JoystickButton8,
            KeyCode.JoystickButton9,
            KeyCode.JoystickButton10,
            KeyCode.JoystickButton11,
            KeyCode.JoystickButton12,
            KeyCode.JoystickButton13,
            KeyCode.JoystickButton14,
            KeyCode.JoystickButton15,
            KeyCode.JoystickButton16,
            KeyCode.JoystickButton17,
            KeyCode.JoystickButton18,
            KeyCode.JoystickButton19
        };

        public readonly List<string> JoystickAxes = new List<string>
        {
            "Joystick0 X Axis",
            "Joystick0 Y Axis",
            "Joystick0 3rd Axis",
            "Joystick0 4th Axis",
            "Joystick0 5th Axis",
            "Joystick0 6th Axis",
            "Joystick0 7th Axis",
            "Joystick0 8th Axis",
            "Joystick0 9th Axis",
            "Joystick0 10th Axis"
        };

        public void Init()
        {
            CollectInputMapping();
            CollectBoundMapping();
            CollectJoystickMapping();
        }

        public int GetInputDeviceMapping(InputMapping input, InputDevice device)
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

        public void SetInputDeviceMapping(InputMapping input, InputDevice device, InputMapping value)
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

        public void SetInputDeviceMapping(InputMapping input, InputDevice device, int value)
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

        public InputMapping GetInputMapping(string name)
        {
            if (inputMappings.TryGetValue(name, out InputMapping input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
        }

        public InputMapping GetBoundMapping(string name)
        {
            if (boundMappings.TryGetValue(name, out InputMapping input))
            {
                return input;
            }

            if (inputMappings.TryGetValue(name, out input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
        }

        public JoystickMapping GetJoystickMapping(string name)
        {
            if (joystickMappings.TryGetValue(name, out JoystickMapping joystick))
            {
                return joystick;
            }

            Debug.LogError($"JoystickMapping is not exist key {name}");
            return null;
        }

        public void RebindButton(string name, InputDevice device, int bindCode)
        {
            if (boundMappings.TryGetValue(name, out InputMapping input))
            {
                SetInputDeviceMapping(input, device, bindCode);
                PlayerPrefs.SetInt(string.Concat(name, device), bindCode);
            }
            else
            {
                Debug.LogError($"InputMapping is not exist key {name}");
            }
        }

        public void ResetButton(string name, InputDevice device)
        {
            InputMapping inputMapping = GetInputMapping(name);
            InputMapping boundMapping = GetInputMapping(name);
            if (inputMapping == null || boundMapping == null)
            {
                return;
            }

            if (GetInputDeviceMapping(inputMapping, device) == GetInputDeviceMapping(boundMapping, device))
            {
                return;
            }

            SetInputDeviceMapping(boundMapping, device, inputMapping);
            PlayerPrefs.DeleteKey(string.Concat(name, "_", device));
        }

        private void CollectInputMapping()
        {
            foreach (InputMapping input in InputSetting.Instance.InputMappings)
            {
                inputMappings.Add(input.Name, input);
            }
        }

        private void CollectBoundMapping()
        {
            foreach (string name in inputMappings.Keys)
            {
                InputMapping rebind = new InputMapping()
                {
                    Name = name,
                    KeyCode = (MouseKeyCode) PlayerPrefs.GetInt(string.Concat(name, InputDevice.MouseKeyboard), (int) inputMappings[name].KeyCode),
                    XboxCode = (XboxCode) PlayerPrefs.GetInt(string.Concat(name, InputDevice.XboxGamepad), (int) inputMappings[name].XboxCode),
                    Ps4Code = (Ps4Code) PlayerPrefs.GetInt(string.Concat(name, InputDevice.Ps4Gamepad), (int) inputMappings[name].Ps4Code),
                };

                boundMappings.Add(name, rebind);
            }
        }

        private void CollectJoystickMapping()
        {
            foreach (JoystickMapping joystick in InputSetting.Instance.JoystickMappings)
            {
                joystickMappings.Add(joystick.Name, joystick);
            }
        }
    }
}