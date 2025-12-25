using System.Runtime.CompilerServices;
using SDL;

namespace BlazeEngine;

public class InputCore
{
    private Vec2 m_MousePosition;
    private float m_MouseWheelDelta;
    
    private Dictionary<SDL_Keycode, bool> m_KeysDownDic = new();
    private Dictionary<SDL_Keycode, bool> m_KeysUpDic = new();

    public Vec2 MousePosition => m_MousePosition;
    public float MouseWheelDelta => m_MouseWheelDelta;
    
    public bool TryProcessEvent(in SDL_Event e)
    {
        switch (e.type)
        {
            case (uint)SDL_EventType.SDL_EVENT_KEY_DOWN:
                m_KeysDownDic[e.key.key] = true;
                return true;
            case (uint)SDL_EventType.SDL_EVENT_KEY_UP:
                m_KeysDownDic[e.key.key] = false;
                m_KeysUpDic[e.key.key] = true;
                return true;
            case (uint)SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                m_MousePosition = new Vec2(e.motion.x, e.motion.y);
                return true;
            case (uint)SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
                m_MouseWheelDelta = e.wheel.y;
                return true;
            default:
                return false;
        }
    }

    public void LateUpdate()
    {
        foreach (var (key, value) in m_KeysUpDic)
        {
            m_KeysUpDic[key] = false;
        }    
        m_MouseWheelDelta = 0.0f;
    }

    public bool IsKeyDown(SDL_Keycode keycode)
    {
        return m_KeysDownDic.TryGetValue(keycode, out var isDown) && isDown;
    }
    public bool IsKeyUp(SDL_Keycode keycode)
    {
        return m_KeysUpDic.TryGetValue(keycode, out var isUp) && isUp;
    }
}

public static class Input
{
    public static InputCore inputCore;

    public static Vec2 MousePosition => inputCore.MousePosition;
    public static float MouseWheelDelta => inputCore.MouseWheelDelta;
    
    public static void Construct(InputCore inputCore)
    {
        Input.inputCore = inputCore;
    }

    public static bool IsKeyDown(SDL_Keycode keycode)
    {
        return inputCore.IsKeyDown(keycode);
    }
    public static bool IsKeyUp(SDL_Keycode keycode)
    {
        return inputCore.IsKeyUp(keycode);
    }
}