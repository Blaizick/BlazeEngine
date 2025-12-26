using System.Runtime.CompilerServices;
using OpenTK.Graphics.Vulkan;
using SDL;

namespace BlazeEngine;

public static class Time
{
    private static float s_LastUpdateTime;
    private static float s_Delta;

    private static float s_LastFixedUpdateTime;
    private static float s_FixedDelta;

    private static float s_PercentsToNextFixedUpdate;
    
    public static float time => (float)(SDL3.SDL_GetPerformanceCounter() / (decimal)SDL3.SDL_GetPerformanceFrequency());
    public static float Delta => s_Delta;
    public static float FixedDelta = s_FixedDelta;
    public static float TimeSinceLastFixedUpdate => time - s_LastFixedUpdateTime;

    public static float PercentsToNextFixedUpdate => s_PercentsToNextFixedUpdate;

    public static void Init()
    {
        s_LastUpdateTime = time;
        s_LastFixedUpdateTime = time;
    }

    public static void Update()
    {
        var tmp = time;
        s_Delta = tmp - s_LastUpdateTime;
        s_LastUpdateTime = tmp;      
        s_PercentsToNextFixedUpdate = TimeSinceLastFixedUpdate / Physics.FixedUpdateDelay;
    }
    public static void FixedUpdate()
    {
        var tmp = time;
        s_FixedDelta = tmp - s_LastFixedUpdateTime;
        s_LastFixedUpdateTime = tmp;
        s_PercentsToNextFixedUpdate = 0;
    } 
}