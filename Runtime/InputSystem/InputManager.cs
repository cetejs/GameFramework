using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        private bool alwaysListenDevice = true;
        private InputIdentity? listenDeviceIdentity = InputIdentity.Player1;
        private InputIdentity? listenRebindIdentity;
        private InputSetting defaultInputSetting;
        private InputListener listener = new InputListener();
        private HashSet<int> usedInputDevices = new HashSet<int>();
        private Dictionary<int, VirtualInput> virtualInputs = new Dictionary<int, VirtualInput>();

        private Action<InputDevice, int> onRebindInput;
        public event Action<InputIdentity, InputDevice> OnInputDeviceChanged;

        protected override void Awake()
        {
            base.Awake();
            listener.Init();
            InitEventSystem();
            InitInputSetting();
            SwitchDefaultDevice();
        }

        private void OnGUI()
        {
            OnListenDeviceInput();
            OnListenRebindInput();
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
            customInput.OnGetButtonDown = name => GetButtonDown(name);
            customInput.OnGetAxisRaw = name => GetAxisRaw(name);
            inputModule.inputOverride = customInput;
            inputModule.horizontalAxis = "Horizontal Nav";
            inputModule.verticalAxis = "Vertical Nav";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
            inputModule.transform.SetParent(transform);
        }

        private void InitInputSetting()
        {
            GameSettings gameSettings = GameSettings.Instance;
            string bundlePath = PathUtils.Combine(gameSettings.InputSettingAssetName, gameSettings.DefaultInputSettingName);
#if UNITY_EDITOR
            string assetPath = StringUtils.Concat("Assets/", AssetSetting.Instance.BundleAssetName, "/", bundlePath, ".asset");
            if (!FileUtils.Exists(assetPath))
            {
                InputSetting instance = ScriptableObject.CreateInstance<InputSetting>();
                FileUtils.CheckDirectory(assetPath);
                UnityEditor.AssetDatabase.CreateAsset(instance, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif
            defaultInputSetting = AssetManager.Instance.LoadAsset<InputSetting>(bundlePath);
        }

        private void SwitchDefaultDevice()
        {
            if (listener.IsXboxDevice(1))
            {
                SetInput(InputIdentity.Player1, InputDevice.XboxGamepad, 1);
                return;
            }

            if (listener.IsPs4Device(1))
            {
                SetInput(InputIdentity.Player1, InputDevice.Ps4Gamepad, 1);
            }
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
                        if (!alwaysListenDevice)
                        {
                            CancelDeviceListening();
                        }

                        return;
                    }
                }

                if (!usedInputDevices.Contains((int) InputDeviceNum.Mobile))
                {
                    if (listener.IsMobileInput())
                    {
                        SetInput(listenDeviceIdentity.Value, InputDevice.Mobile, 0);
                        if (!alwaysListenDevice)
                        {
                            CancelDeviceListening();
                        }

                        return;
                    }
                }

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

                            if (!alwaysListenDevice)
                            {
                                CancelDeviceListening();
                            }

                            return;
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
                        if (listener.ListenJoystickInput(((JoystickInput) input).JoystickNum, out inputCode))
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

        public float GetAxis(string name, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            InputMapping input = setting.GetBoundMapping(name, identity);
            if (input == null)
            {
                return 0f;
            }

            return GetInput(identity).GetAxis(input);
        }

        public float GetAxisRaw(string name, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            InputMapping input = setting.GetBoundMapping(name, identity);
            if (input == null)
            {
                return 0f;
            }

            return GetInput(identity).GetAxisRaw(input);
        }

        public bool GetButton(string name, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            InputMapping input = setting.GetBoundMapping(name, identity);
            if (input == null)
            {
                return false;
            }

            return GetInput(identity).GetButton(input);
        }

        public bool GetButtonDown(string name, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            InputMapping input = setting.GetBoundMapping(name, identity);
            if (input == null)
            {
                return false;
            }

            return GetInput(identity).GetButtonDown(input);
        }

        public bool GetButtonUp(string name, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            InputMapping input = setting.GetBoundMapping(name, identity);
            if (input == null)
            {
                return false;
            }

            return GetInput(identity).GetButtonUp(input);
        }

        public void SetAxisPositive(string name, InputIdentity identity = InputIdentity.Player1)
        {
            GetInput(identity).SetAxis(name, 1);
        }

        public void SetAxisNegative(string name, InputIdentity identity = InputIdentity.Player1)
        {
            GetInput(identity).SetAxis(name, -1);
        }

        public void SetAxisZero(string name, InputIdentity identity = InputIdentity.Player1)
        {
            GetInput(identity).SetAxis(name, 0);
        }

        public void SetAxis(string name, float value, InputIdentity identity = InputIdentity.Player1)
        {
            GetInput(identity).SetAxis(name, value);
        }

        public void SetButtonDown(string name, InputIdentity identity = InputIdentity.Player1)
        {
            GetInput(identity).SetButtonDown(name);
        }

        public void SetButtonUp(string name, InputIdentity identity = InputIdentity.Player1)
        {
            GetInput(identity).SetButtonUp(name);
        }

        public void RebindButton(string name, int bindCode, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            setting.RebindButton(name, bindCode, identity, GetInputDevice(identity));
        }

        public void ResetButton(string name, InputIdentity identity = InputIdentity.Player1, InputSetting setting = null)
        {
            if (setting == null)
            {
                setting = defaultInputSetting;
            }

            setting.ResetButton(name, identity, GetInputDevice(identity));
        }

        public void ListenRebindInput(Action<InputDevice, int> callback, InputIdentity identity = InputIdentity.Player1)
        {
            listenRebindIdentity = identity;
            onRebindInput += callback;
        }

        public void CancelRebindListening()
        {
            listenRebindIdentity = null;
            onRebindInput = null;
        }

        public void ListenDeviceInput(InputIdentity identity)
        {
            listenDeviceIdentity = identity;
            alwaysListenDevice = false;
        }

        public void CancelDeviceListening()
        {
            listenDeviceIdentity = null;
            alwaysListenDevice = false;
        }

        public bool TryGetInputDevice(InputIdentity identity, out InputDevice device)
        {
            device = InputDevice.MouseKeyboard;
            if (!virtualInputs.TryGetValue((int) identity, out VirtualInput input))
            {
                return false;
            }

            device = GetInputDevice(input);
            return true;
        }

        public InputDevice GetInputDevice(InputIdentity identity)
        {
            return GetInputDevice(GetInput(identity));
        }

        private InputDevice GetInputDevice(VirtualInput input)
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

        private VirtualInput GetInput(InputIdentity identity)
        {
            if (!virtualInputs.TryGetValue((int) identity, out VirtualInput input))
            {
#if MOBILE_INPUT
                input = new MobileInput();
                usedInputDevices.Add((int) InputDeviceNum.Mobile);
#else
                input = new StandaloneInput();
                usedInputDevices.Add((int) InputDeviceNum.MouseKeyboard);
#endif
                virtualInputs.Add((int) identity, input);
            }

            return input;
        }

        private void SetInput(InputIdentity identity, InputDevice device, int joystickNum)
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
                usedInputDevices.Remove((int) GetInputDeviceNum(oldInput));
            }

            virtualInputs[(int) identity] = input;
            usedInputDevices.Add((int) GetInputDeviceNum(device, joystickNum));
            OnInputDeviceChanged?.Invoke(identity, device);
        }

        private InputDeviceNum GetInputDeviceNum(VirtualInput input)
        {
            if (input is StandaloneInput)
            {
                return InputDeviceNum.MouseKeyboard;
            }

            if (input is MobileInput)
            {
                return InputDeviceNum.Mobile;
            }

            return (InputDeviceNum) ((JoystickInput) input).JoystickNum;
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

        private enum InputDeviceNum
        {
            Joystick1 = 1,
            Joystick2 = 2,
            Joystick3 = 3,
            Joystick4 = 4,
            Joystick5 = 5,
            Joystick6 = 6,
            Joystick7 = 7,
            Joystick8 = 8,
            MouseKeyboard = 9,
            Mobile = 10
        }
    }
}