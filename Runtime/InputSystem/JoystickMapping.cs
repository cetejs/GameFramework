public class JoystickMapping
{
    public string Name;
    public JoystickType Type;
    public XboxCode XboxCode;
    public Ps4Code Ps4Code;
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
    LeftStickX,
    LeftStickY,
    RightStickX,
    RightStickY,
    DPadX,
    DPadY,
}