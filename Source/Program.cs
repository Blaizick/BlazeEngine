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
    
    public static void Init()
    {
        Debug.Construct(new ConsoleLogger());

        inputCore = new();
        window = new();
        scriptCore = new();
        application = new(window, scriptCore, inputCore);
        graphics = new();
        
        Input.Construct(inputCore);
        Time.Init();
        scriptCore.Init();
        application.Init();
        graphics.Init();
        Camera.Construct(window);
        Camera.Init();
        scriptCore.CallInit();
        application.Loop();
        
        scriptCore.CallQuit();
    }
}

public class GameCore : IGameCore
{
    public void Init()
    {
        
    }

    public int frames = 0;
    public float sdt = 0.0f;
    public void Update()
    {
        if (Input.IsKeyDown(SDL_Keycode.SDLK_D))
            Camera.main.Position += new Vec2(2f * Time.Delta, 0.0f);
        if (Input.IsKeyDown(SDL_Keycode.SDLK_A))
            Camera.main.Position -= new Vec2(2f * Time.Delta, 0.0f);
        if (Input.IsKeyDown(SDL_Keycode.SDLK_W))
            Camera.main.Position += new Vec2(0.0f, 2f * Time.Delta);
        if (Input.IsKeyDown(SDL_Keycode.SDLK_S))
            Camera.main.Position -= new Vec2(0.0f, 2f * Time.Delta);
        Camera.main.Size = Mathf.Clamp(Camera.main.Size - Input.MouseWheelDelta * 0.5f, 0.1f, 10.0f);

        frames++;
        sdt += Time.Delta;
        if (sdt > 1.0f)
        {
            Core.window.Title = $"FPS: {frames}";
            frames = 0;
            sdt = 0.0f;
        }
    }

    public void Draw()
    {
        BlazeEngine.Draw.Color = Color.White;
        BlazeEngine.Draw.Rect(0, 0, 1, 1);
    }

    public void Quit()
    {
    }
}