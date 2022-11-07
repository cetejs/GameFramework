using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.InputService
{
    public class InputConfig : ScriptableObject
    {
        public List<InputMapping> inputMappings = new List<InputMapping>();
        public List<JoystickMapping> joystickMappings = new List<JoystickMapping>();
        
        private static InputConfig instance;

        public static InputConfig Get()
        {
            if (instance)
            {
                return instance;
            }

            InputConfig config = Resources.Load<InputConfig>("InputConfig");
            if (!config)
            {
                Debug.LogError("Please press GameFramework/ImportConfig");
                return null;
            }

            instance = config;
            return instance;
        }
    }

    [Serializable]
    public class InputMapping
    {
        public string buttonName;
        public string description;
        public Keyboard keyboard;
        public JoystickXbox xbox;
        public JoystickPs4 ps4;
        public string mobile;
    }

    [Serializable]
    public class JoystickMapping
    {
        public string joystick;
        public JoystickXbox xbox;
        public JoystickPs4 ps4;
        public bool isAxis;
        public bool isInvert;

        public JoystickMapping Clone()
        {
            return new JoystickMapping()
            {
                joystick = joystick,
                xbox = xbox,
                ps4 = ps4,
                isAxis = isAxis,
                isInvert = isInvert
            };
        }
    }

    public enum Keyboard
    {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Clear = 12,
        Return = 13,
        Pause = 19,
        Escape = 27,
        Space = 32,
        Exclaim = 33,
        DoubleQuote = 34,
        Hash = 35, 
        Dollar = 36, 
        Percent = 37, 
        Ampersand = 38, 
        Quote = 39, 
        LeftParen = 40,
        RightParen = 41,
        Asterisk = 42,
        Plus = 43,
        Comma = 44,
        Minus = 45, 
        Period = 46, 
        Slash = 47,
        Alpha0 = 48, 
        Alpha1 = 49, 
        Alpha2 = 50, 
        Alpha3 = 51, 
        Alpha4 = 52, 
        Alpha5 = 53,
        Alpha6 = 54, 
        Alpha7 = 55,
        Alpha8 = 56, 
        Alpha9 = 57, 
        Colon = 58, 
        Semicolon = 59,
        Less = 60, 
        Equals = 61, 
        Greater = 62, 
        Question = 63,
        At = 64, 
        LeftBracket = 91,
        Backslash = 92,
        RightBracket = 93, 
        Caret = 94, 
        Underscore = 95, 
        BackQuote = 96, 
        A = 97, 
        B = 98,
        C = 99, 
        D = 100,
        E = 101, 
        F = 102, 
        G = 103, 
        H = 104, 
        I = 105, 
        J = 106,
        K = 107, 
        L = 108,
        M = 109, 
        N = 110,
        O = 111, 
        P = 112, 
        Q = 113,
        R = 114,
        S = 115,
        T = 116, 
        U = 117,
        V = 118, 
        W = 119, 
        X = 120,
        Y = 121, 
        Z = 122, 
        LeftCurlyBracket = 123,
        Pipe = 124,
        RightCurlyBracket = 125, 
        Tilde = 126, 
        Delete = 127,
        Keypad0 = 256,
        Keypad1 = 257, 
        Keypad2 = 258, 
        Keypad3 = 259, 
        Keypad4 = 260, 
        Keypad5 = 261, 
        Keypad6 = 262,
        Keypad7 = 263, 
        Keypad8 = 264, 
        Keypad9 = 265, 
        KeypadPeriod = 266, 
        KeypadDivide = 267, 
        KeypadMultiply = 268, 
        KeypadMinus = 269, 
        KeypadPlus = 270, 
        KeypadEnter = 271, 
        KeypadEquals = 272,
        UpArrow = 273, 
        DownArrow = 274,
        RightArrow = 275, 
        LeftArrow = 276, 
        Insert = 277, 
        Home = 278, 
        End = 279, 
        PageUp = 280, 
        PageDown = 281, 
        F1 = 282, 
        F2 = 283,
        F3 = 284, 
        F4 = 285, 
        F5 = 286, 
        F6 = 287,
        F7 = 288, 
        F8 = 289, 
        F9 = 290, 
        F10 = 291, 
        F11 = 292, 
        F12 = 293, 
        F13 = 294, 
        F14 = 295, 
        F15 = 296, 
        Numlock = 300, 
        CapsLock = 301, 
        ScrollLock = 302, 
        RightShift = 303, 
        LeftShift = 304, 
        RightControl = 305,
        LeftControl = 306,
        RightAlt = 307, 
        LeftAlt = 308, 
        RightApple = 309,
        RightCommand = 309, 
        RightMeta = 309, 
        LeftApple = 310, 
        LeftCommand = 310, 
        LeftMeta = 310, 
        LeftWindows = 311, 
        RightWindows = 312, 
        AltGr = 313,
        Help = 315, 
        Print = 316,
        SysReq = 317, 
        Break = 318,
        Menu = 319,
        Horizontal = 600,
        Vertical = 601,
        HorizontalArrow = 602,
        VerticalArrow = 603,
        MouseX = 604,
        MouseY = 605,
        MouseScrollWheel = 606,
        Mouse0 = 607, 
        Mouse1 = 608, 
        Mouse2 = 609,
    }

    public enum JoystickXbox
    {
        None = 0,
        A = 1,
        B = 2,
        X = 3,
        Y = 4,
        LB = 5,
        RB = 6,
        Back = 7,
        Start = 8,
        LeftStickClick = 9,
        RightStickClick = 10,
        LeftStickHorizontal = 11,
        LeftStickVertical = 12,
        Triggers = 13,
        RightStickHorizontal = 14,
        RightStickVertical = 15,
        DPadHorizontal = 16,
        DPadVertical = 17,
        LT = 18,
        RT = 19,
    }

    public enum JoystickPs4
    {
        None = 0,
        Square = 1,
        Cross = 2,
        Circle = 3,
        Triangle = 4,
        L1 = 5,
        R1 = 6,
        L2 = 7,
        R2 = 8,
        Share = 9,
        Options = 10,
        LeftStickClick = 11,
        RightStickClick = 12,
        PlayStation = 13,
        TouchPad = 14,
        LeftStickHorizontal = 15,
        LeftStickVertical = 16,
        RightStickHorizontal = 17,
        RightStickVertical = 18,
        DPadHorizontal = 19,
        DPadVertical = 20,
    }
}