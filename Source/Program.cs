using SDL;

namespace BlazeEngine;

public unsafe class Program
{
    public static void Main(string[] args)
    {
        Core.Init();
    }
}

public static class Core
{
    public static Window window;
    public static ScriptCore scriptCore;
    public static Application application;
    public static Graphics graphics;
    public static InputCore inputCore;
    public static PhysicsCore physicsCore;
    
    public static void Init()
    {
        Debug.Construct(new ConsoleLogger());

        inputCore = new();
        window = new();
        scriptCore = new();
        physicsCore = new();
        application = new(window, scriptCore, inputCore, physicsCore);
        graphics = new();
        
        Input.Construct(inputCore);
        Time.Init();
        scriptCore.Init();
        physicsCore.Init();
        application.Init();
        graphics.Init();
        Camera.Construct(window);
        Camera.Init();
        scriptCore.CallInit();
        
        application.Loop();
        
        scriptCore.CallQuit();
    }
}