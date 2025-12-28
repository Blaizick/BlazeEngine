using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SDL;

namespace BlazeEngine;

public unsafe class Application
{
    public WindowInstance window;
    public ScriptCore scriptCore;
    public InputCore inputCore;
    public PhysicsCore physicsCore;
    
    public Application(WindowInstance window, ScriptCore scriptCore, InputCore inputCore, PhysicsCore physicsCore)
    {
        this.window = window;
        this.scriptCore = scriptCore;
        this.inputCore = inputCore;
        this.physicsCore = physicsCore;
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
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
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

            if (Time.TimeSinceLastFixedUpdate > Physics.FixedUpdateDelay)
            {
                scriptCore.CallPreFixedUpdate();
                physicsCore.FixedUpdate();
                scriptCore.CallFixedUpdate();
                scriptCore.CallPostFixedUpdate();
                Time.FixedUpdate();
            }
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Draw.ProjView = Camera.main.ProjView;
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