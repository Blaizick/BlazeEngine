using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using SDL;

namespace BlazeEngine;

public unsafe class Window
{
    public SDL_Window* sdlWindow;

    private string m_Title = "SDL Window!";
    private Vec2Int m_Size = new(800, 600);

    public string Title
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Title;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Title = value;
            SDL3.SDL_SetWindowTitle(sdlWindow, m_Title);
        }
    }
    public Vec2Int Size
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
            SDL3.SDL_SetWindowSize(sdlWindow, m_Size.X, m_Size.Y);
            GL.Viewport(0, 0, m_Size.X, m_Size.Y);
            onResize?.Invoke();    
        }
    }
    
    public event Action onResize;

    public void Create()
    {
        sdlWindow = SDL3.SDL_CreateWindow(m_Title, m_Size.X, m_Size.Y, SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        if (sdlWindow == null)
        {
            Debug.LogError("SDL_CreateWindow failed!");
        }
    }

    public void SyncSize()
    {
        int x = m_Size.X, y = m_Size.Y; 
        SDL3.SDL_GetWindowSize(sdlWindow, &x, &y);
        m_Size = new Vec2Int(x, y);
        GL.Viewport(0, 0, x, y);
        onResize?.Invoke();
    }
}
