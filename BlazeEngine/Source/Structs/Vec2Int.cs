namespace BlazeEngine;

public unsafe struct Vec2Int
{
    private int m_X;
    private int m_Y;

    public int X
    {
        get { return m_X; }
        set { m_X = value; }
    }

    public int Y
    {
        get { return m_Y; }
        set { m_Y = value; }
    }

    public Vec2Int(int x, int y)
    {
        m_X = x;
        m_Y = y;
    }

    public static implicit operator Vec2(Vec2Int v)
    {
        return new Vec2(v.X, v.Y);
    }
    
    public static readonly Vec2Int Zero = new Vec2Int(0, 0);
}