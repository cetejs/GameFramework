using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class InputListener
    {
        private Dictionary<string, JoystickMapping> joystickMappings = new Dictionary<string, JoystickMapping>();

        private readonly List<KeyCode> JoystickButtons = new List<KeyCode>()
        {
            KeyCode.JoystickButton0,
            KeyCode.Joystick1Button0,
            KeyCode.Joystick2Button0,
            KeyCode.Joystick3Button0,
            KeyCode.Joystick4Button0,
            KeyCode.Joystick5Button0,
            KeyCode.Joystick6Button0,
            KeyCode.Joystick7Button0,
            KeyCode.Joystick8Button0,
        };

        private readonly List<string> JoystickAxes = new List<string>
        {
            "X Axis",
            "Y Axis",
            "3rd Axis",
            "4th Axis",
            "5th Axis",
            "6th Axis",
            "7th Axis",
            "8th Axis",
            "9th Axis",
            "10th Axis"
        };

        public InputListener()
        {
            CollectJoystickMappings();
        }

        public bool IsMouseKeyboardInput()
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

        public bool IsJoystickInput(int joystickNum)
        {
            return ListenJoystickInput(joystickNum, out int inputCode);
        }

        public bool IsMobileInput()
        {
#if MOBILE_INPUT
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject() && Event.current.isMouse || Input.touchCount > 0)
            {
                return true;
            }
#endif

            return false;
        }

        public bool ListenMouseKeyboardInput(out int inputCode)
        {
            inputCode = 0;
            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                inputCode = (int) Event.current.keyCode;
                return true;
            }

            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButton(i))
                {
                    inputCode = (int) KeyCode.Mouse0 + i;
                }
            }

            return false;
        }

        public bool ListenJoystickInput(int joystickNum, out int inputCode)
        {
            inputCode = 0;
            if (!HasJoystick(joystickNum))
            {
                return false;
            }

            KeyCode startKeyCode = JoystickButtons[joystickNum];
            for (KeyCode keyCode = startKeyCode; keyCode < startKeyCode + 20; keyCode++)
            {
                if (Input.GetKey(keyCode))
                {
                    string joystickName = keyCode.ToString();
                    joystickName = joystickName.Substring(joystickName.Length - 7);
                    if (joystickMappings.TryGetValue(joystickName, out JoystickMapping joystick))
                    {
                        inputCode = IsPs4Device(joystickNum) ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
                        return true;
                    }

                    return false;
                }
            }

            for (int i = 0; i < JoystickAxes.Count; i++)
            {
                string axisName;
                if (joystickNum == 0)
                {
                    axisName = StringUtils.Concat("Joystick ", JoystickAxes[i]);
                }
                else
                {
                    axisName = StringUtils.Concat("Joystick", joystickNum, " ", JoystickAxes[i]);
                }

                if ((i == 3 || i == 4) && IsPs4Device(joystickNum) && Application.isFocused)
                {
                    if (Input.GetAxis(axisName) > -1f)
                    {
                        if (joystickMappings.TryGetValue(axisName, out JoystickMapping joystick))
                        {
                            inputCode = IsPs4Device(joystickNum) ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
                            return true;
                        }

                        return false;
                    }
                }
                else if (Input.GetAxis(axisName) != 0f)
                {
                    if (joystickMappings.TryGetValue(axisName, out JoystickMapping joystick))
                    {
                        inputCode = IsPs4Device(joystickNum) ? (int) joystick.Ps4Code : (int) joystick.XboxCode;
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        public bool HasJoystick(int joystickNum)
        {
            joystickNum = Mathf.Max(0, joystickNum - 1);
            string[] names = Input.GetJoystickNames();
            return names.Length > joystickNum;
        }

        public bool IsXboxDevice(int joystickNum)
        {
            joystickNum = Mathf.Max(0, joystickNum - 1);
            string[] names = Input.GetJoystickNames();
            if (names.Length > joystickNum)
            {
                return names[joystickNum].Length == 33;
            }

            return false;
        }

        public bool IsPs4Device(int joystickNum)
        {
            joystickNum = Mathf.Max(0, joystickNum - 1);
            string[] names = Input.GetJoystickNames();
            if (names.Length > joystickNum)
            {
                return names[joystickNum].Length == 19;
            }

            return false;
        }

        private void CollectJoystickMappings()
        {
            foreach (JoystickMapping joystick in InputSetting.Instance.JoystickMappings)
            {
                joystickMappings.Add(joystick.Name, joystick);
            }
        }
    }
}