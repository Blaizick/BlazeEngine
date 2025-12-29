namespace BlazeEngine;

public struct Vec2Bool
{
    private bool m_X = false;
    private bool m_Y = false;

    public bool X
    {
        get => m_X;
        set => m_X = value;
    }
    public bool Y
    {
        get => m_Y;
        set => m_Y = value;
    }

    public Vec2Bool(bool x, bool y)
    {
        m_X = x;
        m_Y = y;
    }

    public static readonly Vec2Bool False = new Vec2Bool(false, false);
    public static readonly Vec2Bool True = new Vec2Bool(true, true);
}