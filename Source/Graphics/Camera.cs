using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace BlazeEngine;

public class Camera
{
    public Matrix4 view;
    public Matrix4 proj;
    public Matrix4 projView;

    public const float PixelsPerInit = 100.0f;

    public static Camera main;
    public static Window window;
    
    private Vec2 m_Position = Vec2.Zero;
    private float m_Size = 5.0f;
    private float m_Rotation = 0.0f;

    public Vec2 Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Position;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Position = value;
            Recalculate();
        }
    }
    public float Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Size;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Size = value;
            Recalculate();
        }
    }
    public float Rotation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Rotation;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Rotation = value;
            Recalculate();
        }
    }
    
    public Camera(float x, float y, float size, float r)
    {
        SetTransform(x, y, size, r);
        window.onResize += Recalculate;
    }

    public void SetTransform(float x, float y, float size, float r)
    {
        m_Position = new Vec2(x, y);
        m_Size = size;
        m_Rotation = r;
        
        Recalculate();
    }

    public static void Construct(Window window)
    {
        Camera.window = window;
    }
    public static void Init()
    {
        main = new Camera(0.0f, 0.0f, 5.0f, 0.0f);
    }

    public void Recalculate()
    {
        view = Matrix4.CreateTranslation(-m_Position.X, -m_Position.Y, 0.0f) *
               Matrix4.CreateRotationZ(-m_Rotation) *
               Matrix4.CreateScale(1.0f / m_Size, 1.0f / m_Size, 1.0f);
        {
            float hw = window.Size.X / PixelsPerInit * 0.5f;
            float hh = window.Size.Y / PixelsPerInit * 0.5f;
            proj = Matrix4.CreateOrthographicOffCenter(-hw, hw, -hh, hh, 1.0f, -1.0f);
        }
        projView = view * proj;
    }
}