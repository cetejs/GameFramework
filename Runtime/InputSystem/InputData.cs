using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.InputService
{
    public class InputData
    {
        private readonly Dictionary<string, InputMapping> inputMappings = new Dictionary<string, InputMapping>();
        private readonly Dictionary<string, InputMapping> boundMappings = new Dictionary<string, InputMapping>();
        private readonly Dictionary<string, JoystickMapping> joystickMappings = new Dictionary<string, JoystickMapping>();

        public void Init()
        {
            CollectInputMapping();
            CollectJoystickMapping();
            CollectBoundMapping();
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

        public int GetInputDeviceMapping(InputMapping input, InputDevice device)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    return (int)input.keyboard;
                case InputDevice.XboxGamepad:
                    return (int)input.xbox;
                case InputDevice.Ps4Gamepad:
                    return (int)input.ps4;
                // case InputDevice.Mobile:
                //     return input.mobile;
                default:
                    return (int)input.keyboard;
            }
        }

        public void SetInputDeviceMapping(InputMapping input, InputDevice device, int value)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    input.keyboard = (Keyboard)value;
                    break;
                case InputDevice.XboxGamepad:
                    input.xbox = (JoystickXbox)value;
                    break;
                case InputDevice.Ps4Gamepad:
                    input.ps4 = (JoystickPs4)value;
                    break;
                // case InputDevice.Mobile:
                //     input.mobile = value;
                //     break;
            }
        }

        public void SetInputDeviceMapping(InputMapping input, InputDevice device, InputMapping value)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    input.keyboard = value.keyboard;
                    break;
                case InputDevice.XboxGamepad:
                    input.xbox = value.xbox;
                    break;
                case InputDevice.Ps4Gamepad:
                    input.ps4 = value.ps4;
                    break;
                case InputDevice.Mobile:
                    input.mobile = value.mobile;
                    break;
            }
        }

        public bool IsRebindConflict(int bindKey, InputDevice device, out string conflictName)
        {
            conflictName = null;
            foreach (string name in inputMappings.Keys)
            {
                InputMapping input = GetBoundMapping(name);
                int key = GetInputDeviceMapping(input, device);
                if (key == bindKey)
                {
                    conflictName = name;
                    return true;
                }
            }

            return false;
        }

        public void RebindButton(string name, InputDevice device, int bindKey)
        {
            if (boundMappings.TryGetValue(name, out InputMapping input))
            {
                SetInputDeviceMapping(input, device, bindKey);
                PlayerPrefs.SetInt(string.Concat(name, "_", device), bindKey);
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

        public void ForeachJoystickMappings(Action<JoystickMapping> onForeach)
        {
            foreach (JoystickMapping joystick in joystickMappings.Values)
            {
                onForeach?.Invoke(joystick);
            }
        }

        private void CollectInputMapping()
        {
            InputConfig config = InputConfig.Get();
            if (!config)
            {
                return;
            }

            inputMappings.Clear();
            foreach (InputMapping mapping in config.inputMappings)
            {
                inputMappings.Add(mapping.buttonName, mapping);
            }
        }

        private void CollectJoystickMapping()
        {
            InputConfig config = InputConfig.Get();
            if (!config)
            {
                return;
            }

            joystickMappings.Clear();
            foreach (JoystickMapping mapping in config.joystickMappings)
            {
                joystickMappings.Add(mapping.joystick, mapping);
            }
        }

        private void CollectBoundMapping()
        {
            foreach (string name in inputMappings.Keys)
            {
                InputMapping rebind = new InputMapping()
                {
                    buttonName = name,
                    keyboard = (Keyboard)PlayerPrefs.GetInt(string.Concat(name, "_", InputDevice.MouseKeyboard), (int)inputMappings[name].keyboard),
                    xbox = (JoystickXbox)PlayerPrefs.GetInt(string.Concat(name, "_", InputDevice.XboxGamepad), (int)inputMappings[name].xbox),
                    ps4 = (JoystickPs4)PlayerPrefs.GetInt(string.Concat(name, "_", InputDevice.Ps4Gamepad), (int)inputMappings[name].ps4),
                    mobile = name
                };

                boundMappings.Add(name, rebind);
            }
        }
    }
}