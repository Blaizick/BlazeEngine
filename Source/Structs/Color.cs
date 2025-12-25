using System.Runtime.CompilerServices;

namespace BlazeEngine;

public struct Color
{
    public byte r;
    public byte g;
    public byte b;
    public byte a;

    public float Rf
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return r / 255f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            r = (byte)(value * 255f);
        }
    }
    public float Gf
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return g / 255f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            g = (byte)(value * 255f);
        }
    }
    public float Bf
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return b / 255f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            b = (byte)(value * 255f);
        }
    }
    public float Af
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return a / 255f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            a = (byte)(value * 255f);
        }
    }

    public Color(float r, float g, float b, float a)
    {
        Rf = r;
        Gf = g;
        Bf = b;
        Af = a;
    }
    public Color(byte r, byte g, byte b, byte a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static Color White => new(1.0f, 1.0f, 1.0f, 1.0f);
    public static Color Black => new(0.0f, 0.0f, 0.0f, 1.0f);
    public static Color LightDark => new(0.1f, 0.1f, 0.1f, 1.0f);
}