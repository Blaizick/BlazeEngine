using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace BlazeEngine;

public class Camera
{
    private Matrix4 m_ProjView;
    private Matrix4 m_InvProjView;
    private Matrix4 m_UnconstrainedProjView;
    private Matrix4 m_InvUnconstrainedProjView;
    
    public Matrix4 ProjView => m_ProjView;
    public Matrix4 InvProjView => m_InvProjView;
    public Matrix4 UnconstrainedProjView => m_UnconstrainedProjView;
    public Matrix4 InvUnconstrainedProjView => m_InvUnconstrainedProjView;
    
    public const float PixelsPerInit = 150.0f;

    public static Camera main;
    public static WindowInstance window;
    
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

    public static void Construct(WindowInstance windowInstance)
    {
        Camera.window = windowInstance;
    }
    public static void Init()
    {
        main = new Camera(0.0f, 0.0f, 5.0f, 0.0f);
    }

    public void Recalculate()
    {
        var view = Matrix4.CreateTranslation(-m_Position.X, -m_Position.Y, 0.0f) *
                   Matrix4.CreateRotationZ(-m_Rotation) *
                   Matrix4.CreateScale(1.0f / m_Size, 1.0f / m_Size, 1.0f);
        float hw = window.Size.X / PixelsPerInit * 0.5f;
        float hh = window.Size.Y / PixelsPerInit * 0.5f;
        var proj = Matrix4.CreateOrthographicOffCenter(-hw, hw, -hh, hh, -1.0f, 1.0f);
        m_ProjView = view * proj;

        m_InvProjView = Matrix4.Invert(m_ProjView);
        
        m_UnconstrainedProjView = Matrix4.CreateOrthographicOffCenter(0, window.Size.X, 0, window.Size.Y, -1.0f, 1.0f);
        m_InvUnconstrainedProjView = Matrix4.Invert(m_UnconstrainedProjView);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec2 ViewportPointToNdc(in Vec2 point)
    {
        return new Vec2(point.X / window.Size.X * 2 - 1, 
            (1 - (point.Y / window.Size.Y)) * 2 - 1);
    }
    
    public Vec2 ViewportToWorldPoint(in Vec2 point)
    {
        return Vector4.TransformRow(ViewportPointToNdc(point), m_InvProjView).Xy;
    }
    public Vec2 ViewportToScreenPoint(in Vec2 point)
    {
        return Vector4.TransformRow(ViewportPointToNdc(point), m_InvUnconstrainedProjView).Xy;
    }
}