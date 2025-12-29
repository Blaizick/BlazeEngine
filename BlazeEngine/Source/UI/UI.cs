using System.Runtime.CompilerServices;
using SDL;

namespace BlazeEngine.UI;

public class UICore
{
    public WindowInstance window;
    public MainCanvas mainCanvas;

    public UICore(WindowInstance window)
    {
        this.window = window;
    }
    
    public void Init()
    {
        mainCanvas = new(window);
        mainCanvas.Init();
    }
    
    public void Update()
    {
        mainCanvas.Update();
    }
    
    public void Draw()
    {
        Drawf.ProjView = Camera.main.UnconstrainedProjView;
        mainCanvas.Draw();
    }
}

public static class UI
{
    public static UICore uiCore;

    public static MainCanvas MainCanvas => uiCore.mainCanvas;
    
    public static void Construct(UICore uiCore)
    {
        UI.uiCore = uiCore;
    }
}

public class MainCanvas : UIElement
{
    public WindowInstance window;

    public MainCanvas(WindowInstance window)
    {
        this.window = window;
    }
    
    public override void Init()
    {
        base.Init();
        
        window.onResize += () =>
        {
            m_Size = window.Size;
            m_Position = (Vec2)window.Size * 0.5f;
        };
        
        m_Position = (Vec2)window.Size * 0.5f;
        m_Size = window.Size;
    }

    public override void Update()
    {
        base.Update();
    }
}

public class Image : UIElement
{
    public Sprite sprite = null;
    public Color color = Color.White;

    public Image(Sprite sprite)
    {
        this.sprite = sprite;
    }   
    
    public override void Draw()
    {
        Drawf.Color = color;
        if (Sprite.IsValid(sprite))
        {
            Drawf.sprite = sprite;
            Drawf.Sprite(m_Position, m_Size);
        }
        else
        {
            Drawf.Rect(m_Position, m_Size);
        }
        base.Draw();
    }
}

public class Button : UIElement
{
    public Color color = Color.White;

    protected bool m_Hold;
    
    public event Action onClick;
    public event Action onPointerStay;
    
    public Button(Action onClick)
    {
        this.onClick = onClick;
    }

    public override void Update()
    {
        Rect rect = new Rect(m_Position, m_Size);
        
        if (m_Hold && Input.IsButtonUp(SDLButton.SDL_BUTTON_LEFT))
        {
            m_Hold = false;
            onClick?.Invoke();
        }
        var mousePos = Input.MousePosition;
        if (rect.Contains(mousePos))
        {
            m_Hold = Input.IsButtonDown(SDLButton.SDL_BUTTON_LEFT);
            onPointerStay?.Invoke();
        }
        
        base.Update();
    }
    
    public override void Draw()
    {
        Drawf.Color = color;
        Drawf.Rect(m_Position, m_Size);
        base.Draw();
    }
}

public class UIElement
{
    protected Vec2 m_Position = Vec2.Zero;

    protected Vec2 m_Size = new(80.0f, 80.0f);
    
    public Vec2 Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_Position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => m_Position = value;            
    }
    
    public Vec2 Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_Size;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => m_Size = value;
    }
    
    protected List<UIElement> m_Children = new();
    protected UIElement? m_Parent = null;
    
    public virtual void Init() {}

    public virtual void Update()
    {
        foreach (var element in m_Children)
        {
            element.Update();
        }
    }

    public virtual void Draw()
    {
        foreach (var element in m_Children)
        {
            element.Draw();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddChild(UIElement element)
    {
        m_Children.Add(element);
        element.SetParentCalmly(this);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveChild(UIElement element)
    {
        var id = m_Children.IndexOf(element);
        m_Children[id].RemoveParentCalmly();
        m_Children.RemoveAt(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetParentCalmly(UIElement parent)
    {
        m_Parent = parent;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveParentCalmly()
    {
        m_Parent = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual UIElement SetPosition(float x, float y)
    {
        return SetPosition(new Vec2(x, y));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual UIElement SetPosition(in Vec2 pos)
    {
        Position = pos;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual UIElement SetSize(float w, float h)
    {
        return SetSize(new Vec2(w, h));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual UIElement SetSize(in Vec2 size)
    {
        Size = size;
        return this;
    }
    
    public Image Image(Sprite sprite)
    {
        var img = new Image(sprite);
        AddChild(img);
        return img;
    }
    public Button Button(Action onClick)
    {
        var btn = new Button(onClick);
        AddChild(btn);
        return btn;
    }
    public Table Table()
    {
        var table = new Table();
        AddChild(table);
        return table;
    }
}

public class Table : UIElement
{
    protected Vec2 m_Spacing = new Vec2(10.0f, 10.0f);
    protected Vec2Bool m_Fill = Vec2Bool.False;
    
    public Vec2 Spacing 
    { 
        get => m_Spacing; 
        set => m_Spacing = value; 
    }
    public Vec2Bool Fill
    {
        get => m_Fill;
        set => m_Fill = value;
    }

    protected float m_MarginLeft, m_MarginRight;
    protected float m_MarginTop, m_MarginBottom;
    
    public override void Update()
    {
        UpdateFill();
        Layout();
        base.Update();
    }

    public void UpdateFill()
    {
        var size = m_Size;
        var pos = m_Position;
        if (Fill.X)
        {
            size.X = m_Parent.Size.X;
            pos.X = m_Parent.Position.X; 
        }
        if (Fill.Y)
        {
            size.Y = m_Parent.Size.Y;
            pos.Y = m_Parent.Position.Y;
        }
        Position = pos;
        Size = size;
    }
    
    public void Layout()
    {
        Vec2 halfSize = m_Size * 0.5f;
        Vec2 overrideRelPos = new(-halfSize.X + m_MarginLeft, halfSize.Y - m_MarginTop);
        Vec2 curRelPos = overrideRelPos;
        Vec2 maxElemSize = Vec2.Zero;

        foreach (var element in m_Children)
        {
            Vec2 elementHalfSize = element.Size * 0.5f;
            element.Position = Position + curRelPos + new Vec2(elementHalfSize.X, -elementHalfSize.Y);

            if (element.Size.X > maxElemSize.X)
                maxElemSize.X = element.Size.X;
            if (element.Size.Y > maxElemSize.Y)
                maxElemSize.Y = element.Size.Y;

            curRelPos.X += element.Size.X + m_Spacing.X;
            
            if (curRelPos.X + halfSize.X + m_MarginRight > Size.X)
            {
                curRelPos.Y -= maxElemSize.Y + m_Spacing.Y;
                curRelPos.X = overrideRelPos.X;
            }
        }
    }

    public Table SetMarginLeft(float margin)
    {
        m_MarginLeft = margin;
        return this;
    }
    public Table SetMarginRight(float margin)
    {
        m_MarginRight = margin;
        return this;
    }
    public Table SetMarginTop(float margin)
    {
        m_MarginTop = margin;
        return this;
    }
    public Table SetMarginBottom(float margin)
    {
        m_MarginBottom = margin;
        return this;
    }

    public Table SetSpacing(in Vec2 spacing)
    {
        Spacing = spacing;
        return this;
    }
    public Table SetSpacing(float x, float y)
    {
        Spacing = new Vec2(x, y);
        return this;
    }
    

    public Table SetFill(in Vec2Bool fill)
    {
        Fill = fill;
        return this;
    }
}

//UI.MainCanvas.Button("btn", () => {
//  Console.WriteLine("Button clicked");
//});
//UI.MainCanvas.Image(Sprites.Placeholder);