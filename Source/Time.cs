using System.Runtime.CompilerServices;
using SDL;

namespace BlazeEngine;

public static class Time
{
    private static float s_LastTime;
    private static float s_Delta;

    public static float time
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return (float)(SDL3.SDL_GetPerformanceCounter() / (decimal)SDL3.SDL_GetPerformanceFrequency());
        }
    }
    public static float Delta
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Delta;
        }
    }

    public static void Init()
    {
        s_LastTime = time;
    }

    public static void Update()
    {
        var tmp = time;
        s_Delta = tmp - s_LastTime;
        s_LastTime = tmp;            
    }
}