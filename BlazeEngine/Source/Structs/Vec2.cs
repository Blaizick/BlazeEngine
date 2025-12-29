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

    public Vec2 Normalized
    {
        get
        {
            float magnitude = Mathf.Sqrt(m_X * m_X + m_Y * m_Y);
            if (magnitude == 0.0f)
                return Zero;
            return new Vec2(m_X / magnitude, m_Y / magnitude);
        }
    }

    public static Vec2 GetFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vec2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
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

    public static implicit operator Vec2(in Vector4 v)
    {
        return new (v.X, v.Y);
    }
    public static implicit operator Vector4(in Vec2 v)
    {
        return new (v.X, v.Y, 0.0f, 1.0f);
    }

    public static implicit operator System.Numerics.Vector2(in Vec2 v)
    {
        return new(v.X, v.Y);
    }
    public static implicit operator Vec2(in System.Numerics.Vector2 v)
    {
        return new(v.X, v.Y);
    }

    public static implicit operator Microsoft.Xna.Framework.Vector2(in Vec2 v)
    {
        return new(v.X, v.Y);
    }
    public static implicit operator Vec2(in Microsoft.Xna.Framework.Vector2 v)
    {
        return new(v.X, v.Y);
    }

    public static implicit operator OpenTK.Mathematics.Vector2(in Vec2 v)
    {
        return new OpenTK.Mathematics.Vector2(v.X, v.Y);
    }
    public static implicit operator Vec2(in OpenTK.Mathematics.Vector2 v)
    {
        return new Vec2(v.X, v.Y);
    }
    
    public static Vec2 MoveTowards(Vec2 start, Vec2 target, float maxDistanceDelta)
    {
        Vec2 dir = target - start;
        float sqrDst = dir.X * dir.X + dir.Y * dir.Y;
        if (sqrDst == 0 || (maxDistanceDelta >= 0 && sqrDst <= maxDistanceDelta * maxDistanceDelta))
            return target;
        float dst = Mathf.Sqrt(sqrDst);
        return start + (dir / dst * maxDistanceDelta);
    }

    public static Vec2 Lerp(Vec2 start, Vec2 target, float t)
    {
        return start + (target - start) * t;
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