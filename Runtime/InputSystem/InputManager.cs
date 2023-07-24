using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        private bool listenJoysticks;
        private InputIdentity? listenDeviceIdentity;
        private InputIdentity? listenRebindIdentity;
        private InputData inputData = new InputData();
        private InputListener listener = new InputListener();
        private HashSet<int> usedInputDevices = new HashSet<int>();
        private Dictionary<int, VirtualInput> virtualInputs = new Dictionary<int, VirtualInput>();

        private Action<InputDevice, int> onRebindInput;
        public event Action<InputIdentity, InputDevice> OnInputDeviceChanged;

        private void OnGUI()
        {
            OnListenDeviceInput();
            OnListenRebindInput();
        }

        private void OnListenDeviceInput()
        {
            if (listenDeviceIdentity != null)
            {
                if (!usedInputDevices.Contains((int) InputDeviceNum.MouseKeyboard))
                {
                    if (listener.IsMouseKeyboardInput())
                    {
                        SetInput(listenDeviceIdentity.Value, InputDevice.MouseKeyboard, 0);
                        CancelDeviceListening();
                        return;
                    }
                }

                if (!usedInputDevices.Contains((int) InputDeviceNum.Mobile))
                {
                    if (listener.IsMobileInput())
                    {
                        SetInput(listenDeviceIdentity.Value, InputDevice.Mobile, 0);
                        CancelDeviceListening();
                        return;
                    }
                }

                if (listenJoysticks)
                {
                    int joystickNum = (int) InputDeviceNum.Joystick;
                    if (!usedInputDevices.Contains(joystickNum))
                    {
                        if (listener.IsJoystickInput(joystickNum))
                        {
                            if (listener.IsPs4Device(joystickNum))
                            {
                                SetInput(listenDeviceIdentity.Value, InputDevice.Ps4Gamepad, joystickNum);
                            }
                            else
                            {
                                SetInput(listenDeviceIdentity.Value, InputDevice.XboxGamepad, joystickNum);
                            }

                            CancelDeviceListening();
                        }
                    }
                }
                else
                {
                    for (int i = (int) InputDeviceNum.Joystick1; i <= (int) InputDeviceNum.Joystick8; i++)
                    {
                        int joystickNum = i;
                        if (!usedInputDevices.Contains(joystickNum))
                        {
                            if (listener.IsJoystickInput(joystickNum))
                            {
                                if (listener.IsPs4Device(joystickNum))
                                {
                                    SetInput(listenDeviceIdentity.Value, InputDevice.Ps4Gamepad, joystickNum);
                                }
                                else
                                {
                                    SetInput(listenDeviceIdentity.Value, InputDevice.XboxGamepad, joystickNum);
                                }

                                CancelDeviceListening();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void OnListenRebindInput()
        {
            if (listenRebindIdentity != null)
            {
                int inputCode;
                VirtualInput input = GetInput(listenRebindIdentity.Value);
                InputDevice inputDevice = GetInputDevice(input);
                switch (inputDevice)
                {
                    case InputDevice.MouseKeyboard:
                        if (listener.ListenMouseKeyboardInput(out inputCode))
                        {
                            onRebindInput?.Invoke(inputDevice, inputCode);
                            CancelRebindListening();
                        }

                        break;
                    case InputDevice.XboxGamepad:
                    case InputDevice.Ps4Gamepad:
                        if (listener.ListenJoystickInput(((JoystickInput) input).joystickNum, out inputCode))
                        {
                            onRebindInput?.Invoke(inputDevice, inputCode);
                            CancelRebindListening();
                        }

                        break;
                    default:
                        CancelRebindListening();
                        break;
                }
            }
        }

        public float GetAxis(string name, InputIdentity identity)
        {
            InputMapping input = inputData.GetBoundMapping(name, identity);
            if (input == null)
            {
                return 0f;
            }

            return GetInput(identity).GetAxis(input);
        }

        public float GetAxisRaw(string name, InputIdentity identity)
        {
            InputMapping input = inputData.GetBoundMapping(name, identity);
            if (input == null)
            {
                return 0f;
            }

            return GetInput(identity).GetAxisRaw(input);
        }

        public bool GetButton(string name, InputIdentity identity)
        {
            InputMapping input = inputData.GetBoundMapping(name, identity);
            if (input == null)
            {
                return false;
            }

            return GetInput(identity).GetButton(input);
        }

        public bool GetButtonDown(string name, InputIdentity identity)
        {
            InputMapping input = inputData.GetBoundMapping(name, identity);
            if (input == null)
            {
                return false;
            }

            return GetInput(identity).GetButtonDown(input);
        }

        public bool GetButtonUp(string name, InputIdentity identity)
        {
            InputMapping input = inputData.GetBoundMapping(name, identity);
            if (input == null)
            {
                return false;
            }

            return GetInput(identity).GetButtonUp(input);
        }

        public void SetAxisPositive(string name, InputIdentity identity)
        {
            GetInput(identity).SetAxis(name, 1);
        }

        public void SetAxisNegative(string name, InputIdentity identity)
        {
            GetInput(identity).SetAxis(name, -1);
        }

        public void SetAxisZero(string name, InputIdentity identity)
        {
            GetInput(identity).SetAxis(name, 0);
        }

        public void SetAxis(string name, float value, InputIdentity identity)
        {
            GetInput(identity).SetAxis(name, value);
        }

        public void SetButtonDown(string name, InputIdentity identity)
        {
            GetInput(identity).SetButtonDown(name);
        }

        public void SetButtonUp(string name, InputIdentity identity)
        {
            GetInput(identity).SetButtonUp(name);
        }

        public void RebindButton(string name, int bindCode, InputIdentity identity)
        {
            inputData.RebindButton(name, bindCode, identity, GetInputDevice(identity));
        }

        public void ResetButton(string name, InputIdentity identity)
        {
            inputData.ResetButton(name, identity, GetInputDevice(identity));
        }

        public void ListenDeviceInput(InputIdentity identity, bool listenJoysticks)
        {
            if (listenDeviceIdentity != null)
            {
                GameLogger.LogError($"Input {identity} device listening already exists");
                return;
            }

            listenDeviceIdentity = identity;
            this.listenJoysticks = listenJoysticks;
        }

        public void CancelDeviceListening()
        {
            listenDeviceIdentity = null;
            listenJoysticks = false;
        }

        public void ListenRebindInput(InputIdentity identity, Action<InputDevice, int> callback)
        {
            if (listenRebindIdentity != null)
            {
                GameLogger.LogError($"Input {identity} rebind listening already exists");
                return;
            }

            listenRebindIdentity = identity;
            onRebindInput += callback;
        }

        public void CancelRebindListening()
        {
            listenRebindIdentity = null;
            onRebindInput = null;
        }

        internal InputDevice GetInputDevice(InputIdentity identity)
        {
            return GetInputDevice(GetInput(identity));
        }

        internal InputDevice GetInputDevice(VirtualInput input)
        {
            if (input is StandaloneInput)
            {
                return InputDevice.MouseKeyboard;
            }

            if (input is XboxInput)
            {
                return InputDevice.XboxGamepad;
            }

            if (input is Ps4Input)
            {
                return InputDevice.Ps4Gamepad;
            }

            return InputDevice.Mobile;
        }

        internal VirtualInput GetInput(InputIdentity identity)
        {
            if (!virtualInputs.TryGetValue((int) identity, out VirtualInput input))
            {
                input = new StandaloneInput();
                virtualInputs.Add((int) identity, input);
                usedInputDevices.Add((int) InputDeviceNum.MouseKeyboard);
            }

            return input;
        }

        internal void SetInput(InputIdentity identity, InputDevice device, int joystickNum)
        {
            VirtualInput input;
            switch (device)
            {
                case InputDevice.XboxGamepad:
                    input = new XboxInput(joystickNum);
                    break;
                case InputDevice.Ps4Gamepad:
                    input = new Ps4Input(joystickNum);
                    break;
                case InputDevice.Mobile:
                    input = new MobileInput();
                    break;
                default:
                    input = new StandaloneInput();
                    break;
            }

            if (virtualInputs.TryGetValue((int) identity, out VirtualInput oldInput))
            {
                usedInputDevices.Remove((int) GetInputDeviceNum(GetInputDevice(oldInput), joystickNum));
            }

            virtualInputs[(int) identity] = input;
            usedInputDevices.Add((int) GetInputDeviceNum(device, joystickNum));
            OnInputDeviceChanged?.Invoke(identity, device);
        }

        private InputDeviceNum GetInputDeviceNum(InputDevice device, int joystickNum)
        {
            switch (device)
            {
                case InputDevice.MouseKeyboard:
                    return InputDeviceNum.MouseKeyboard;
                case InputDevice.Mobile:
                    return InputDeviceNum.Mobile;
                default:
                    return (InputDeviceNum) joystickNum;
            }
        }

        // public string InputName
        // {
        //     get
        //     {
        //         switch (inputDevice)
        //         {
        //             case InputDevice.MouseKeyboard:
        //                 return ((KeyCode) inputCode).ToString();
        //             case InputDevice.XboxGamepad:
        //                 return ((XboxCode) inputCode).ToString();
        //             case InputDevice.Ps4Gamepad:
        //                 return ((Ps4Code) inputCode).ToString();
        //         }
        //
        //         return null;
        //     }
        // }

//         private void Awake()
//         {
//             InitEventSystem();
//             InitInputDevice();
//             SelectDefaultDevice();
//         }
//
//         private void OnGUI()
//         {
//             switch (inputDevice)
//             {
//                 case InputDevice.MouseKeyboard:
//                     if (IsJoystickInput())
//                     {
//                         if (IsPs4Device())
//                         {
//                             SwitchDevice(InputDevice.Ps4Gamepad);
//                         }
//                         else
//                         {
//                             SwitchDevice(InputDevice.XboxGamepad);
//                         }
//                     }
//                     else if (IsMobileInput())
//                     {
//                         SwitchDevice(InputDevice.Mobile);
//                     }
//                     else
//                     {
//                         UpdateMouseKeyboard();
//                     }
//
//                     break;
//                 case InputDevice.XboxGamepad:
//                     if (IsMouseKeyboard())
//                     {
//                         SwitchDevice(InputDevice.MouseKeyboard);
//                     }
//                     else if (IsMobileInput())
//                     {
//                         SwitchDevice(InputDevice.Mobile);
//                     }
//                     else if (IsPs4Device() && IsJoystickInput())
//                     {
//                         SwitchDevice(InputDevice.Ps4Gamepad);
//                     }
//
//                     break;
//                 case InputDevice.Ps4Gamepad:
//                     if (IsMouseKeyboard())
//                     {
//                         SwitchDevice(InputDevice.MouseKeyboard);
//                     }
//                     else if (IsMobileInput())
//                     {
//                         SwitchDevice(InputDevice.Mobile);
//                     }
//                     else if (!IsPs4Device() && IsJoystickInput())
//                     {
//                         SwitchDevice(InputDevice.XboxGamepad);
//                     }
//
//                     break;
//                 case InputDevice.Mobile:
//                     if (IsJoystickInput())
//                     {
//                         if (IsPs4Device())
//                         {
//                             SwitchDevice(InputDevice.Ps4Gamepad);
//                         }
//                         else
//                         {
//                             SwitchDevice(InputDevice.XboxGamepad);
//                         }
//                     }
//                     else if (IsMouseKeyboard())
//                     {
//                         SwitchDevice(InputDevice.MouseKeyboard);
//                     }
//
//                     break;
//             }
//         }
//
//         private void Update()
//         {
//             switch (inputDevice)
//             {
//                 case InputDevice.XboxGamepad:
//                 case InputDevice.Ps4Gamepad:
//                     UpdateJoystickInput();
//                     break;
//                 case InputDevice.Mobile:
//                     inputCode = 0;
//                     break;
//             }
//         }
//
//         private void OnApplicationFocus(bool hasFocus)
//         {
//             applicationForce = hasFocus;
//         }
//
//         public void SwitchDevice(InputDevice device)
//         {
//             if (device == inputDevice && activeInput != null)
//             {
//                 return;
//             }
//
//             inputDevice = device;
//             activeInput = virtualInputs[(int) device];
//             OnDeviceChanged?.Invoke(device);
//         }
//

//
//         private void InitEventSystem()
//         {
//             if (EventSystem.current == null)
//             {
//                 EventSystem.current = new GameObject("EventSystem").AddComponent<EventSystem>();
//                 EventSystem.current.gameObject.AddComponent<StandaloneInputModule>();
//             }
//
//             StandaloneInputModule inputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
//             // var customInput = inputModule.gameObject.AddComponent<CustomInput>();
//             // customInput.onGetButtonDown = GetButtonDown;
//             // customInput.onGetAxisRaw = GetAxisRaw;
//             // inputModule.inputOverride = customInput;
//             inputModule.horizontalAxis = "HorizontalNav";
//             inputModule.verticalAxis = "VerticalNav";
//             inputModule.submitButton = "Submit";
//             inputModule.cancelButton = "Cancel";
//         }
//
//         private void InitInputDevice()
//         {
//             inputData.Init();
//             virtualInputs.Add((int) InputDevice.MouseKeyboard, new StandaloneInput());
//             virtualInputs.Add((int) InputDevice.XboxGamepad, new XboxInput());
//             virtualInputs.Add((int) InputDevice.Ps4Gamepad, new Ps4Input());
//             virtualInputs.Add((int) InputDevice.Mobile, new MobileInput());
//         }
//
//         private void SelectDefaultDevice()
//         {
//             if (IsXboxDevice())
//             {
//                 SwitchDevice(InputDevice.XboxGamepad);
//             }
//             else if (IsPs4Device())
//             {
//                 SwitchDevice(InputDevice.Ps4Gamepad);
//             }
//             else if (IsMobileInput())
//             {
//                 SwitchDevice(InputDevice.Mobile);
//             }
//             else
//             {
//                 SwitchDevice(InputDevice.MouseKeyboard);
//             }
//         }
//
//         private bool IsMouseKeyboard()
//         {
// #if MOBILE_INPUT
//             return false;
// #endif
//             if (Event.current.isKey || Event.current.isMouse)
//             {
//                 return true;
//             }
//
//             if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f || Input.GetAxis("Mouse ScrollWheel") != 0f)
//             {
//                 return true;
//             }
//
//             return false;
//         }
//
//         private bool IsJoystickInput()
//         {
//             for (int i = 0; i < inputData.JoystickButtons.Count; i++)
//             {
//                 KeyCode button = inputData.JoystickButtons[i];
//                 if (Input.GetKey(button))
//                 {
//                     return true;
//                 }
//             }
//
//             for (int i = 0; i < inputData.JoystickAxes.Count; i++)
//             {
//                 string axis = inputData.JoystickAxes[i];
//                 if ((i == 3 || i == 4) && IsPs4Device() && applicationForce)
//                 {
//                     if (Input.GetAxis(axis) > -1f)
//                     {
//                         return true;
//                     }
//                 }
//                 else if (Input.GetAxis(axis) != 0f)
//                 {
//                     return true;
//                 }
//             }
//
//             return false;
//         }
//
//         private bool IsMobileInput()
//         {
// #if MOBILE_INPUT
//             if (EventSystem.current && EventSystem.current.IsPointerOverGameObject() && Event.current.isMouse || Input.touchCount > 0)
//             {
//                 return true;
//             }
// #endif
//
//             return false;
//         }
//
//         private bool IsXboxDevice()
//         {
//             string[] names = Input.GetJoystickNames();
//             if (names.Length > 0)
//             {
//                 return names[0].Length == 33;
//             }
//
//             return false;
//         }
//
//         private bool IsPs4Device()
//         {
//             string[] names = Input.GetJoystickNames();
//             if (names.Length > 0)
//             {
//                 return names[0].Length == 19;
//             }
//
//             return false;
//         }
//
//         private void UpdateMouseKeyboard()
//         {
//             if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
//             {
//                 inputCode = (int) Event.current.keyCode;
//                 inputAxis = 0f;
//                 return;
//             }
//
//             inputAxis = 0f;
//         }
//
//         private void UpdateJoystickInput()
//         {
//             for (int i = 0; i < inputData.JoystickButtons.Count; i++)
//             {
//                 KeyCode button = inputData.JoystickButtons[i];
//                 if (Input.GetKeyDown(button))
//                 {
//                     JoystickMapping joystick = inputData.GetJoystickMapping(button.ToString());
//                     if (joystick != null)
//                     {
//                         inputCode = IsPs4Device() ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
//                         inputAxis = 0f;
//                         return;
//                     }
//                 }
//             }
//
//             for (int i = 0; i < inputData.JoystickAxes.Count; i++)
//             {
//                 string axis = inputData.JoystickAxes[i];
//                 float axisValue = Input.GetAxis(axis);
//                 if ((i == 3 || i == 4) && IsPs4Device() && applicationForce)
//                 {
//                     if (axisValue > -1f)
//                     {
//                         JoystickMapping joystick = inputData.GetJoystickMapping(axis);
//                         if (joystick != null)
//                         {
//                             inputCode = (int) joystick.Ps4Code;
//                             inputAxis = 1f;
//                             return;
//                         }
//                     }
//                 }
//                 else if (axisValue != 0f)
//                 {
//                     JoystickMapping joystick = inputData.GetJoystickMapping(axis);
//                     if (joystick != null)
//                     {
//                         inputCode = IsPs4Device() ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
//                         inputAxis = axisValue;
//                         return;
//                     }
//                 }
//             }
//
//             inputCode = 0;
//             inputAxis = 0f;
//         }
    }

    internal enum InputDeviceNum
    {
        Joystick,
        Joystick1,
        Joystick2,
        Joystick3,
        Joystick4,
        Joystick5,
        Joystick6,
        Joystick7,
        Joystick8,
        MouseKeyboard,
        Mobile,
    }
}