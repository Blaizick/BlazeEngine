using OpenTK.Mathematics;

namespace BlazeEngine;

public struct Vec2 : IFormattable
{
    private float m_X;
    private float m_Y;

    public float X
    {
        get { return m_X; }
        set { m_X = value; }
    }
    public float Y
    {
        get{ return m_Y; }
        set { m_Y = value; }
    }
    
    public Vec2(float x, float y)
    {
        m_X = x;
        m_Y = y;
    }
    
    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        return new Vec2(a.X + b.X, a.Y + b.Y);
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        return new Vec2(a.X - b.X, a.Y - b.Y);
    }

    public static Vec2 operator /(Vec2 a, Vec2 b)
    {
        return new Vec2(a.X / b.X, a.Y / b.Y);
    }
    public static Vec2 operator /(Vec2 a, float b)
    {
        return new Vec2(a.X / b, a.Y / b);
    }

    public static Vec2 operator *(Vec2 a, Vec2 b)
    {
        return new Vec2(a.X * b.X, a.Y * b.Y);
    }
    public static Vec2 operator *(Vec2 a, float b)
    {
        return new Vec2(a.X * b, a.Y * b);
    }

    public static implicit operator Vec2(Vector4 v)
    {
        return new Vec2(v.X, v.Y);
    }
    public static implicit operator Vector4(Vec2 v)
    {
        return new Vector4(v.X, v.Y, 0.0f, 1.0f);
    }
    
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable =
            $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return ToString(null, null);
    }
    
    public static Vec2 Zero => new Vec2(0.0f, 0.0f);
}