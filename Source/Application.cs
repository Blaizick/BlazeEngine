using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SDL;

namespace BlazeEngine;

public unsafe class Application
{
    public Window window;
    public ScriptCore scriptCore;
    public InputCore inputCore;
    
    public Application(Window window, ScriptCore scriptCore, InputCore inputCore)
    {
        this.window = window;
        this.scriptCore = scriptCore;
        this.inputCore = inputCore;
    }
    
    public void Init()
    {
        if (!SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
        {
            Debug.LogError("SDL_Init failed!");
        }
        
        SDL3.SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_MINOR_VERSION, 0);
        SDL3.SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL3.SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_FLAGS, 0);
        SDL3.SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_PROFILE_MASK, SDL3.SDL_GL_CONTEXT_PROFILE_CORE);

        window.Create();
        
        var glContext = SDL3.SDL_GL_CreateContext(window.sdlWindow);
        if (glContext == null)
        {
            Debug.LogError("SDL_GL_CreateContext failed!");
        }
        
        SDL3.SDL_GL_SetSwapInterval(0);

        GLLoader.LoadBindings(new SDL3GLBindingsContext());
        
        window.SyncSize();
    }

    public void Loop()
    {
        GL.ClearColor(new Color4<Rgba>(0.1f, 0.1f, 0.1f, 1.0f));

        bool running = true;
        while (running)
        {
            SDL_Event e;
            while (SDL3.SDL_PollEvent(&e))
            {
                switch (e.type)
                {
                    case (uint)SDL_EventType.SDL_EVENT_QUIT:
                        running = false;
                        break;
                    case (uint)SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                        window.SyncSize();
                        break;
                }
                inputCore.TryProcessEvent(e);
            }
            
            scriptCore.CallUpdate();
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Draw.ProjView = Camera.main.projView;
            scriptCore.CallDraw();
            Draw.Flush();
            SDL3.SDL_GL_SwapWindow(window.sdlWindow);

            inputCore.LateUpdate();
            Time.Update();
        }
    }
}

public class SDL3GLBindingsContext : IBindingsContext
{
    public IntPtr GetProcAddress(string procName)
    {
        return SDL3.SDL_GL_GetProcAddress(procName);
    }
}