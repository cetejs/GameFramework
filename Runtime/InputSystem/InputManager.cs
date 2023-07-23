using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        private InputDevice inputDevice;
        private VirtualInput activeInput;
        private InputData inputData = new InputData();
        private Dictionary<int, VirtualInput> virtualInputs = new Dictionary<int, VirtualInput>();
        private int inputCode;
        private float inputAxis;
        private bool applicationForce = true;

        public event Action<InputDevice> OnDeviceChanged;

        public string InputName
        {
            get
            {
                switch (inputDevice)
                {
                    case InputDevice.MouseKeyboard:
                        return ((KeyCode) inputCode).ToString();
                    case InputDevice.XboxGamepad:
                        return ((XboxCode) inputCode).ToString();
                    case InputDevice.Ps4Gamepad:
                        return ((Ps4Code) inputCode).ToString();
                }

                return null;
            }
        }

        private void Awake()
        {
            InitEventSystem();
            InitInputDevice();
            SelectDefaultDevice();
        }

        private void OnGUI()
        {
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    if (IsJoystickInput())
                    {
                        if (IsPs4Device())
                        {
                            SwitchDevice(InputDevice.Ps4Gamepad);
                        }
                        else
                        {
                            SwitchDevice(InputDevice.XboxGamepad);
                        }
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    else
                    {
                        UpdateMouseKeyboard();
                    }

                    break;
                case InputDevice.XboxGamepad:
                    if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    else if (IsPs4Device() && IsJoystickInput())
                    {
                        SwitchDevice(InputDevice.Ps4Gamepad);
                    }

                    break;
                case InputDevice.Ps4Gamepad:
                    if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    else if (!IsPs4Device() && IsJoystickInput())
                    {
                        SwitchDevice(InputDevice.XboxGamepad);
                    }

                    break;
                case InputDevice.Mobile:
                    if (IsJoystickInput())
                    {
                        if (IsPs4Device())
                        {
                            SwitchDevice(InputDevice.Ps4Gamepad);
                        }
                        else
                        {
                            SwitchDevice(InputDevice.XboxGamepad);
                        }
                    }
                    else if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }

                    break;
            }
        }

        private void Update()
        {
            switch (inputDevice)
            {
                case InputDevice.XboxGamepad:
                case InputDevice.Ps4Gamepad:
                    UpdateJoystickInput();
                    break;
                case InputDevice.Mobile:
                    inputCode = 0;
                    break;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            applicationForce = hasFocus;
        }

        public void SwitchDevice(InputDevice device)
        {
            if (device == inputDevice && activeInput != null)
            {
                return;
            }

            inputDevice = device;
            activeInput = virtualInputs[(int) device];
            OnDeviceChanged?.Invoke(device);
        }

        public float GetAxis(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return 0f;
            }

            return activeInput.GetAxis(input);
        }

        public float GetAxisRaw(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return 0f;
            }

            return activeInput.GetAxisRaw(input);
        }

        public bool GetButton(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButton(input);
        }

        public bool GetButtonDown(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButtonDown(input);
        }

        public bool GetButtonUp(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButtonUp(input);
        }

        public void SetAxisPositive(string name)
        {
            activeInput.SetAxis(name, 1);
        }

        public void SetAxisNegative(string name)
        {
            activeInput.SetAxis(name, -1);
        }

        public void SetAxisZero(string name)
        {
            activeInput.SetAxis(name, 0);
        }

        public void SetAxis(string name, float value)
        {
            activeInput.SetAxis(name, value);
        }

        public void SetButtonDown(string name)
        {
            activeInput.SetButtonDown(name);
        }

        public void SetButtonUp(string name)
        {
            activeInput.SetButtonUp(name);
        }

        private void InitEventSystem()
        {
            if (EventSystem.current == null)
            {
                EventSystem.current = new GameObject("EventSystem").AddComponent<EventSystem>();
                EventSystem.current.gameObject.AddComponent<StandaloneInputModule>();
            }

            StandaloneInputModule inputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            // var customInput = inputModule.gameObject.AddComponent<CustomInput>();
            // customInput.onGetButtonDown = GetButtonDown;
            // customInput.onGetAxisRaw = GetAxisRaw;
            // inputModule.inputOverride = customInput;
            inputModule.horizontalAxis = "HorizontalNav";
            inputModule.verticalAxis = "VerticalNav";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
        }

        private void InitInputDevice()
        {
            inputData.Init();
            virtualInputs.Add((int) InputDevice.MouseKeyboard, new StandaloneInput());
            virtualInputs.Add((int) InputDevice.XboxGamepad, new XboxInput());
            virtualInputs.Add((int) InputDevice.Ps4Gamepad, new Ps4Input());
            virtualInputs.Add((int) InputDevice.Mobile, new MobileInput());
        }

        private void SelectDefaultDevice()
        {
            if (IsXboxDevice())
            {
                SwitchDevice(InputDevice.XboxGamepad);
            }
            else if (IsPs4Device())
            {
                SwitchDevice(InputDevice.Ps4Gamepad);
            }
            else if (IsMobileInput())
            {
                SwitchDevice(InputDevice.Mobile);
            }
            else
            {
                SwitchDevice(InputDevice.MouseKeyboard);
            }
        }

        private bool IsMouseKeyboard()
        {
#if MOBILE_INPUT
            return false;
#endif
            if (Event.current.isKey || Event.current.isMouse)
            {
                return true;
            }

            if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f || Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                return true;
            }

            return false;
        }

        private bool IsJoystickInput()
        {
            for (int i = 0; i < inputData.JoystickButtons.Count; i++)
            {
                KeyCode button = inputData.JoystickButtons[i];
                if (Input.GetKey(button))
                {
                    return true;
                }
            }

            for (int i = 0; i < inputData.JoystickAxes.Count; i++)
            {
                string axis = inputData.JoystickAxes[i];
                if ((i == 3 || i == 4) && IsPs4Device() && applicationForce)
                {
                    if (Input.GetAxis(axis) > -1f)
                    {
                        return true;
                    }
                }
                else if (Input.GetAxis(axis) != 0f)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsMobileInput()
        {
#if MOBILE_INPUT
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject() && Event.current.isMouse || Input.touchCount > 0)
            {
                return true;
            }
#endif

            return false;
        }

        private bool IsXboxDevice()
        {
            string[] names = Input.GetJoystickNames();
            if (names.Length > 0)
            {
                return names[0].Length == 33;
            }

            return false;
        }

        private bool IsPs4Device()
        {
            string[] names = Input.GetJoystickNames();
            if (names.Length > 0)
            {
                return names[0].Length == 19;
            }

            return false;
        }

        private void UpdateMouseKeyboard()
        {
            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                inputCode = (int) Event.current.keyCode;
                inputAxis = 0f;
                return;
            }

            inputAxis = 0f;
        }

        private void UpdateJoystickInput()
        {
            for (int i = 0; i < inputData.JoystickButtons.Count; i++)
            {
                KeyCode button = inputData.JoystickButtons[i];
                if (Input.GetKeyDown(button))
                {
                    JoystickMapping joystick = inputData.GetJoystickMapping(button.ToString());
                    if (joystick != null)
                    {
                        inputCode = IsPs4Device() ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
                        inputAxis = 0f;
                        return;
                    }
                }
            }

            for (int i = 0; i < inputData.JoystickAxes.Count; i++)
            {
                string axis = inputData.JoystickAxes[i];
                float axisValue = Input.GetAxis(axis);
                if ((i == 3 || i == 4) && IsPs4Device() && applicationForce)
                {
                    if (axisValue > -1f)
                    {
                        JoystickMapping joystick = inputData.GetJoystickMapping(axis);
                        if (joystick != null)
                        {
                            inputCode = (int) joystick.Ps4Code;
                            inputAxis = 1f;
                            return;
                        }
                    }
                }
                else if (axisValue != 0f)
                {
                    JoystickMapping joystick = inputData.GetJoystickMapping(axis);
                    if (joystick != null)
                    {
                        inputCode = IsPs4Device() ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
                        inputAxis = axisValue;
                        return;
                    }
                }
            }

            inputCode = 0;
            inputAxis = 0f;
        }
    }
}