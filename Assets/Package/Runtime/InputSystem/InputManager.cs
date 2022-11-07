using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GameFramework.Generic;
using UnityEngine.UI;

namespace GameFramework.InputService
{
    public class InputManager : PersistentService
    {
        private InputDevice inputDevice;
        private VirtualInput activeInput;
        private readonly InputData inputData = new InputData();
        private readonly Dictionary<int, VirtualInput> virtualInputs = new Dictionary<int, VirtualInput>();

        private readonly List<KeyCode> joystickButtons = new List<KeyCode>
        {
            KeyCode.Joystick1Button0,
            KeyCode.Joystick1Button1,
            KeyCode.Joystick1Button2,
            KeyCode.Joystick1Button3,
            KeyCode.Joystick1Button4,
            KeyCode.Joystick1Button5,
            KeyCode.Joystick1Button6,
            KeyCode.Joystick1Button7,
            KeyCode.Joystick1Button8,
            KeyCode.Joystick1Button9,
            KeyCode.Joystick1Button10,
            KeyCode.Joystick1Button11,
            KeyCode.Joystick1Button12,
            KeyCode.Joystick1Button13,
            KeyCode.Joystick1Button14,
            KeyCode.Joystick1Button15,
            KeyCode.Joystick1Button16,
            KeyCode.Joystick1Button17,
            KeyCode.Joystick1Button18,
            KeyCode.Joystick1Button19,
        };

        private readonly List<string> joystickAxes = new List<string>
        {
            "x axis",
            "y axis",
            "3rd axis",
            "4th axis",
            "5th axis",
            "6th axis",
            "7th axis",
            "8th axis",
            "9th axis",
            "10th axis"
        };

        private bool isForce = true;
        private int inputKey;
        private float inputAxis;
        private Action<InputDevice> onDeviceChanged;

        public event Action<InputDevice> OnDeviceChanged
        {
            add { onDeviceChanged += value; }
            remove { onDeviceChanged -= value; }
        }

        public InputDevice InputDevice
        {
            get { return inputDevice; }
        }

        public bool IsKeyboardDevice
        {
            get { return inputDevice == InputDevice.MouseKeyboard; }
        }

        public bool IsJoystickDevice
        {
            get
            {
                return inputDevice == InputDevice.XboxGamepad ||
                       inputDevice == InputDevice.Ps4Gamepad;
            }
        }

        public bool IsMobileDevice
        {
            get { return inputDevice == InputDevice.Mobile; }
        }

        public int InputKey
        {
            get { return inputKey; }
        }
        
        public string InputName
        {
            get
            {
                if (inputKey == 0)
                {
                    return null;
                }

                switch (inputDevice)
                {
                    case InputDevice.MouseKeyboard:
                        return ((Keyboard) inputKey).ToString();
                    case InputDevice.XboxGamepad:
                        return ((JoystickXbox) inputKey).ToString();
                    case InputDevice.Ps4Gamepad:
                        return ((JoystickPs4) inputKey).ToString();
                }

                return null;
            }
        }

        public float InputAxis
        {
            get { return inputAxis; }
        }

        protected override void Awake()
        {
            base.Awake();
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
                    inputKey = 0;
                    break;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            isForce = hasFocus;
        }

        public bool IsPointerOverGameObject()
        {
            if (inputDevice == InputDevice.Mobile)
            {
                return false;
            }

            return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        }

        public bool IsMouseInScreen()
        {
            if (inputDevice == InputDevice.Mobile)
            {
                return true;
            }

            return Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width &&
                   Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height;
        }

        public void SwitchDevice(InputDevice device)
        {
            if (device == inputDevice && activeInput != null)
            {
                return;
            }

            inputDevice = device;
            activeInput = virtualInputs[(int) device];
            onDeviceChanged?.Invoke(device);
        }

        public float GetAxis(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return 0.0f;
            }

            return activeInput.GetAxis(input);
        }

        public float GetAxisRaw(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return 0.0f;
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

        public bool IsRebindConflict(int bindKey, out string conflictName)
        {
            return inputData.IsRebindConflict(bindKey, inputDevice, out conflictName);
        }

        public void RebindButton(string name, int bindKey)
        {
            inputData.RebindButton(name, inputDevice, bindKey);
        }

        public void ResetButton(string name)
        {
            inputData.ResetButton(name, inputDevice);
        }

        public string GetDescription(string name)
        {
            InputMapping input = inputData.GetInputMapping(name);
            if (input != null)
            {
                string description = input.description;
                if (string.IsNullOrEmpty(description))
                {
                    description = input.buttonName;
                }

                return description;
            }

            return null;
        }

        public int GetActiveInputMapping(string name)
        {
            InputMapping input = inputData.GetInputMapping(name);
            if (input != null)
            {
                return inputData.GetInputDeviceMapping(input, inputDevice);
            }

            return 0;
        }

        public int GetActiveBoundMapping(string name)
        {
            InputMapping input = inputData.GetBoundMapping(name);
            if (input != null)
            {
                return inputData.GetInputDeviceMapping(input, inputDevice);
            }

            return 0;
        }

        public void SelectDefaultGo()
        {
            if (IsJoystickDevice)
            {
                if (!EventSystem.current.currentSelectedGameObject)
                {
                    Selectable selectable = FindObjectOfType<Selectable>();
                    if (selectable)
                    {
                        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                    }
                }
            }
        }

        private void InitEventSystem()
        {
            if (EventSystem.current == null)
            {
                EventSystem.current = new GameObject("EventSystem").AddComponent<EventSystem>();
                EventSystem.current.gameObject.AddComponent<StandaloneInputModule>();
            }

            StandaloneInputModule inputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            CustomInput customInput = inputModule.gameObject.AddComponent<CustomInput>();
            customInput.onGetButtonDown = GetButtonDown;
            customInput.onGetAxisRaw = GetAxisRaw;
            inputModule.inputOverride = customInput;
            inputModule.horizontalAxis = "HorizontalNav";
            inputModule.verticalAxis = "VerticalNav";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
            inputModule.transform.SetParent(transform);
        }

        private void InitInputDevice()
        {
            inputData.Init();
            virtualInputs.Add((int) InputDevice.MouseKeyboard, new StandaloneInput());
            virtualInputs.Add((int) InputDevice.XboxGamepad, new XboxInput(inputData));
            virtualInputs.Add((int) InputDevice.Ps4Gamepad, new Ps4Input(inputData));
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

            if (Input.GetAxis("MouseX") != 0.0f || Input.GetAxis("MouseY") != 0.0f)
            {
                return true;
            }

            return false;
        }

        private bool IsJoystickInput()
        {
            for (int i = 0; i < joystickButtons.Count; i++)
            {
                KeyCode button = joystickButtons[i];
                if (Input.GetKey(button))
                {
                    return true;
                }
            }

            for (int i = 0; i < joystickAxes.Count; i++)
            {
                string axis = joystickAxes[i];
                if ((i == 3 || i == 4) && IsPs4Device() && isForce)
                {
                    if (Input.GetAxis(axis) > -1.0f)
                    {
                        return true;
                    }
                }
                else if (Input.GetAxis(axis) != 0.0f)
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
                inputKey = (int) Event.current.keyCode;
                inputAxis = 0.0f;
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButton(i))
                {
                    inputKey = (int) Keyboard.Mouse0 + i;
                    inputAxis = 0.0f;
                    return;
                }
            }

            inputKey = 0;
            inputAxis = 0.0f;
        }

        private void UpdateJoystickInput()
        {
            for (int i = 0; i < joystickButtons.Count; i++)
            {
                KeyCode button = joystickButtons[i];
                if (Input.GetKeyDown(button))
                {
                    JoystickMapping joystick = inputData.GetJoystickMapping(button.ToString());
                    if (joystick != null)
                    {
                        inputKey = IsPs4Device() ? (int)joystick.ps4 : (int)joystick.xbox;
                        inputAxis = 0.0f;
                        return;
                    }
                }
            }

            for (int i = joystickAxes.Count - 1; i >= 0; i--)
            {
                string axis = joystickAxes[i];
                float axisValue = Input.GetAxis(axis);
                if ((i == 3 || i == 4) && IsPs4Device() && isForce)
                {
                    if (axisValue > -1.0f)
                    {
                        JoystickMapping joystick = inputData.GetJoystickMapping(axis);
                        if (joystick != null)
                        {
                            inputKey = (int)joystick.ps4;
                            inputAxis = 1.0f;
                            return;
                        }
                    }
                }
                else if (axisValue != 0.0f)
                {
                    JoystickMapping joystick = inputData.GetJoystickMapping(axis);
                    if (joystick != null)
                    {
                        inputKey = IsPs4Device() ? (int)joystick.ps4 : (int)joystick.xbox;
                        inputAxis = axisValue;
                        return;
                    }
                }
            }

            inputKey = 0;
            inputAxis = 0.0f;
        }
    }

    public enum InputDevice
    {
        MouseKeyboard,
        XboxGamepad,
        Ps4Gamepad,
        Mobile,
    }
}