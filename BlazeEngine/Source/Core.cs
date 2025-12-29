using BlazeEngine.UI;
using SDL;

namespace BlazeEngine;

public static class Core
{
    public static WindowInstance window;
    public static ScriptCore scriptCore;
    public static Application application;
    public static Graphics graphics;
    public static InputCore inputCore;
    public static PhysicsCore physicsCore;
    public static UICore uiCore;
    
    public static void Init()
    {
        Debug.Construct(new ConsoleLogger());
        YAML.Init();
        
        inputCore = new();
        window = new();
        scriptCore = new();
        physicsCore = new();
        uiCore = new(window);
        application = new(window, scriptCore, inputCore, physicsCore, uiCore);
        graphics = new();
        
        Random.Init();
        Input.Construct(inputCore);
        Time.Init();
        scriptCore.Init();
        physicsCore.Init();
        application.Init();
        Window.Construct(window);
        graphics.Init();
        Camera.Construct(window);
        Camera.Init();
        uiCore.Init();
        UI.UI.Construct(uiCore);
        scriptCore.CallInit();
        
        application.Loop();
        
        scriptCore.CallQuit();
    }
}