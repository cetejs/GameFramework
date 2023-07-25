using System.Collections.Generic;

internal class JoystickMapping
{
    public string Name;
    public JoystickType Type;
    public XboxCode XboxCode;
    public Ps4Code Ps4Code;

    public static List<JoystickMapping> Mappings = new List<JoystickMapping>()
    {
        new JoystickMapping()
        {
            Name = "Button0",
            Type = JoystickType.Button,
            XboxCode = XboxCode.A,
            Ps4Code = Ps4Code.Square
        },
        new JoystickMapping()
        {
            Name = "Button1",
            Type = JoystickType.Button,
            XboxCode = XboxCode.B,
            Ps4Code = Ps4Code.Cross
        },
        new JoystickMapping()
        {
            Name = "Button2",
            Type = JoystickType.Button,
            XboxCode = XboxCode.X,
            Ps4Code = Ps4Code.Circle
        },
        new JoystickMapping()
        {
            Name = "Button3",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Y,
            Ps4Code = Ps4Code.Triangle
        },
        new JoystickMapping()
        {
            Name = "Button4",
            Type = JoystickType.Button,
            XboxCode = XboxCode.LB,
            Ps4Code = Ps4Code.L1
        },
        new JoystickMapping()
        {
            Name = "Button5",
            Type = JoystickType.Button,
            XboxCode = XboxCode.RB,
            Ps4Code = Ps4Code.R1
        },
        new JoystickMapping()
        {
            Name = "Button6",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Back,
            Ps4Code = Ps4Code.Node
        },
        new JoystickMapping()
        {
            Name = "Button7",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Start,
            Ps4Code = Ps4Code.Node
        },
        new JoystickMapping()
        {
            Name = "Button8",
            Type = JoystickType.Button,
            XboxCode = XboxCode.LeftStick,
            Ps4Code = Ps4Code.Share
        },
        new JoystickMapping()
        {
            Name = "Button9",
            Type = JoystickType.Button,
            XboxCode = XboxCode.RightStick,
            Ps4Code = Ps4Code.Options
        },
        new JoystickMapping()
        {
            Name = "Button10",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Node,
            Ps4Code = Ps4Code.LeftStick
        },
        new JoystickMapping()
        {
            Name = "Button11",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Node,
            Ps4Code = Ps4Code.RightStick
        },
        new JoystickMapping()
        {
            Name = "Button12",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Node,
            Ps4Code = Ps4Code.PlayStation
        },
        new JoystickMapping()
        {
            Name = "Button13",
            Type = JoystickType.Button,
            XboxCode = XboxCode.Node,
            Ps4Code = Ps4Code.TouchPad
        },
        new JoystickMapping()
        {
            Name = "X Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.LeftStickX,
            Ps4Code = Ps4Code.LeftStickX
        },
        new JoystickMapping()
        {
            Name = "Y Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.LeftStickY,
            Ps4Code = Ps4Code.LeftStickY
        },
        new JoystickMapping()
        {
            Name = "3rd Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.Node,
            Ps4Code = Ps4Code.RightStickX
        },
        new JoystickMapping()
        {
            Name = "4th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.RightStickX,
            Ps4Code = Ps4Code.L2
        },
        new JoystickMapping()
        {
            Name = "5th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.RightStickY,
            Ps4Code = Ps4Code.R2
        },
        new JoystickMapping()
        {
            Name = "6th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.DPadX,
            Ps4Code = Ps4Code.RightStickY
        },
        new JoystickMapping()
        {
            Name = "7th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.DPadY,
            Ps4Code = Ps4Code.DPadX
        },
        new JoystickMapping()
        {
            Name = "8th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.Node,
            Ps4Code = Ps4Code.DPadY
        },
        new JoystickMapping()
        {
            Name = "9th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.LT,
            Ps4Code = Ps4Code.Node
        },
        new JoystickMapping()
        {
            Name = "10th Axis",
            Type = JoystickType.Axis,
            XboxCode = XboxCode.RT,
            Ps4Code = Ps4Code.Node
        }
    };
}

public enum JoystickType
{
    Button,
    Axis
}

public enum XboxCode
{
    Node,
    A,
    B,
    X,
    Y,
    LB,
    RB,
    LT,
    RT,
    Start,
    Back,
    LeftStick,
    RightStick,
    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight,
    LeftStickX,
    LeftStickY,
    RightStickX,
    RightStickY,
    DPadX,
    DPadY,
}

public enum Ps4Code
{
    Node,
    Triangle,
    Circle,
    Square,
    Cross,
    L1,
    R1,
    L2,
    R2,
    Share,
    Options,
    TouchPad,
    PlayStation,
    LeftStick,
    RightStick,
    DPadLeft,
    DPadRight,
    DPadUp,
    DPadDown,
    LeftStickX,
    LeftStickY,
    RightStickX,
    RightStickY,
    DPadX,
    DPadY,
}