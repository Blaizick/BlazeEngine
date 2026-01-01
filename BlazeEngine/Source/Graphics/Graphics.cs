using System.Runtime.CompilerServices;
using BlazeEngine.ResourcesManagement;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace BlazeEngine;

public class Graphics
{
    public Buffers buffers;
    
    public void Init()
    {
        buffers = new Buffers();
        buffers.Generate();
        
        Shaders.Init();
        IBatchItem.Init();

        Batcher batcher = new();
        ColorBatch colorBatch = new(batcher, buffers);
        colorBatch.Init();
        SpriteBatch spriteBatch = new(batcher, buffers);
        spriteBatch.Init();
        Drawf.Construct(buffers, batcher, colorBatch, spriteBatch);
    }
}

public class Buffers
{
    public int vao;
    public int vbo;
    public int ebo;

    public void Generate()
    {
        GL.GenVertexArray(out vao);
        GL.GenBuffer(out vbo);
        GL.GenBuffer(out ebo);
    }

    public void Bind()
    {
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
    }
}

public class Shader
{
    public string vertexSource;
    public string fragmentSource;

    public int program;

    private Dictionary<string, int> m_UniformLocations = new();
    
    public Shader(string vertexSource, string fragmentSource)
    {
        this.vertexSource = vertexSource;
        this.fragmentSource = fragmentSource;
        
        Init();
    }

    public void Init()
    {
        program = GL.CreateProgram();
        
        var vertexShader = InitShader(vertexSource, ShaderType.VertexShader);
        var fragmentShader = InitShader(fragmentSource, ShaderType.FragmentShader);
        
        GL.LinkProgram(program);
        GL.GetProgrami(program, ProgramProperty.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            throw new NotImplementedException();
        }

        FinalizeShader(vertexShader);
        FinalizeShader(fragmentShader);

        GL.GetProgrami(program, ProgramProperty.ActiveUniforms, out var uniformsCount);
        m_UniformLocations.Clear();
        for (uint i = 0; i < uniformsCount; i++)
        {
            GL.GetActiveUniformName(program, i, 128, out _, out var key);
            m_UniformLocations[key] = GL.GetUniformLocation(program, key);
        }
        
        int InitShader(string source, ShaderType type)
        {
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            var code = GL.GetShaderi(shader, ShaderParameterName.CompileStatus);
            if (code != (int)All.True)
            {
                GL.GetShaderInfoLog(shader, out var infoLog);
                throw new Exception(infoLog);
            }
            GL.AttachShader(program, shader);
            return shader;
        }
        void FinalizeShader(int shader)
        {
            GL.DetachShader(program, shader);
            GL.DeleteShader(shader);
        }
    }

    public void Use()
    {
        GL.UseProgram(program);
    }

    public void SetUniform(string key, ref Matrix4 mat)
    {
        GL.UniformMatrix4f(m_UniformLocations[key], 1, false, ref mat);
    }
}

public static class Drawf
{
    private static Color s_Color = Color.White;
    public static Color Color
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Color;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            s_Color = value;
            spriteBatch.Color = s_Color;
            colorBatch.Color = s_Color;
        }
    }

    private static float s_Z = 0.0f;
    public static float Z
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            s_Z = value;
        }
    }

    private static Sprite s_Sprite;
    public static Sprite sprite
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Sprite;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            s_Sprite = value;
            spriteBatch.Sprite = s_Sprite;
        }
    }
    
    private static Matrix4 s_ProjView;
    public static Matrix4 ProjView
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_ProjView;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            s_ProjView = value;
            colorBatch.ProjView = s_ProjView;
            spriteBatch.ProjView = s_ProjView;
        }
    }
    
    public static Buffers buffers;
    public static Batcher batcher;
    public static ColorBatch colorBatch;
    public static SpriteBatch spriteBatch;
    
    public static void Construct(Buffers buffers, Batcher batcher, ColorBatch colorBatch, SpriteBatch spriteBatch)
    {
        Drawf.buffers = buffers;
        Drawf.batcher = batcher;
        Drawf.colorBatch = colorBatch;
        Drawf.spriteBatch = spriteBatch;
    }

    public static void Flush()
    {
        batcher.Flush();
    }

    public static void Rect(in Vec2 position, in Vec2 size)
    {
        colorBatch.DrawRect(position.X, position.Y, size.X, size.Y);
    }
    public static void Rect(float x, float y, float w, float h)
    {
        colorBatch.DrawRect(x, y, w, h);
    }
    public static void Rect(in Vec2 position, in Vec2 size, float rotation)
    {
        colorBatch.DrawRect(position.X, position.Y, size.X, size.Y, rotation);
    }
    public static void Rect(float x, float y, float w, float h, float r)
    {
        colorBatch.DrawRect(x, y, w, h, r);
    }

    public static void Sprite(float x, float y, float w, float h)
    {
        spriteBatch.DrawSprite(x, y, w, h);
    }
    public static void Sprite(in Vec2 position, in Vec2 size)
    {
        spriteBatch.DrawSprite(position.X, position.Y, size.X, size.Y);
    }
    public static void Sprite(float x, float y, float w, float h, float r)
    {
        spriteBatch.DrawSprite(x, y, w, h, r);
    }
    public static void Sprite(in Vec2 position, in Vec2 size, float rotation)
    {
        spriteBatch.DrawSprite(position.X, position.Y, size.X, size.Y, rotation);
    }
}

public class Batcher
{
    public List<IBatchItem> items = new();
    public List<IBatch> batches = new();

    public void Flush()
    {
        foreach (var i in items)
        {
            i.Flush();
        }
        foreach (var i in batches)
        {
            i.SetDirty();
        }
        items.Clear();
    }
    
    public void Add(IBatchItem item)
    {
        items.Add(item);
    }
}

public static class Shaders
{
    public static Shader colorBatchShader;
    public static Shader spriteBatchShader;
    
    public static void Init()
    {
        colorBatchShader = new("#version 330 core\n" +
                                       "layout(location = 0) in vec2 aPosition;\n" + 
                                       "layout(location = 1) in vec4 aColor;\n" +
                                       "out vec4 vColor;\n" +
                                       "uniform mat4 uProjView;\n" +
                                       "void main(void)\n" +
                                       "{\n" +
                                       "    vColor = aColor;\n" +
                                       "    gl_Position = uProjView * vec4(aPosition.x, aPosition.y, 0.0, 1.0);\n" +
                                       "}\n", 
            "#version 330 core\n" +
            "out vec4 oColor;\n" +
            "in vec4 vColor;\n" +
            "void main()\n" +
            "{\n" +
            "   oColor = vColor;\n" +
            "}\n");
        spriteBatchShader = new("#version 330 core\n" +
                                "layout(location = 0) in vec2 aPosition;\n" + 
                                "layout(location = 1) in vec4 aColor;\n" +
                                "layout(location = 2) in vec2 aTexCoord;\n" +
                                "out vec4 vColor;\n" +
                                "out vec2 vTexCoord;\n" +
                                "uniform mat4 uProjView;\n" +
                                "void main(void)\n" +
                                "{\n" +
                                "    vTexCoord = aTexCoord;\n" +
                                "    vColor = aColor;\n" +
                                "    gl_Position = uProjView * vec4(aPosition.x, aPosition.y, 0.0, 1.0);\n" +
                                "}\n", 
            "#version 330 core\n" +
            "out vec4 oColor;\n" +
            "in vec4 vColor;\n" +
            "in vec2 vTexCoord;\n" +
            "uniform sampler2D uTexture;\n" +
            "void main()\n" +
            "{\n" +
            "   oColor = vColor * texture(uTexture, vTexCoord);\n" +
            "}\n");
    }
}

public interface IBatch
{
    public void SetDirty();
}

public class BaseBatch<T> : IBatch where T : class, IBatchItem
{
    protected T m_LastItem;
    public bool isDirty = true;
    public Batcher batcher;
    public Buffers buffers;

    protected Matrix4 m_ProjView = Matrix4.Identity;
    public Matrix4 ProjView
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_ProjView;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_ProjView = value;
            SetDirty();
        }
    }

    protected Color m_Color = Color.White;
    public Color Color
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Color;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Color = value;
        }
    }

    public BaseBatch(Batcher batcher, Buffers buffers)
    {
        this.batcher = batcher;
        this.buffers = buffers;
    }
    
    public void Init()
    {
        batcher.batches.Add(this);
    }
    
    public void SetDirty()
    {
        m_LastItem = null!;
        isDirty = true;
    }

    public Vec2[] GetRectPositions(float x, float y, float w, float h)
    {
        float hw = w * 0.5f;
        float hh = h * 0.5f;
        return new[]
        {
            new Vec2(x - hw, y - hh),
            new Vec2(x + hw, y - hh),
            new Vec2(x + hw, y + hh),
            new Vec2(x - hw, y + hh),
        };
    }
    public Vec2[] GetRectPositions(float x, float y, float w, float h, float r)
    {
        float rad = r * MathF.PI / 180f;
        float cos = MathF.Cos(rad);
        float sin = MathF.Sin(rad);

        float hw = w * 0.5f;
        float hh = h * 0.5f;

        Vec2 Rotate(Vec2 p) =>
            new Vec2(
                p.X * cos - p.Y * sin,
                p.X * sin + p.Y * cos
            ) + new Vec2(x, y);

        return new[]
        {
            Rotate(new Vec2(-hw, -hh)),
            Rotate(new Vec2( hw, -hh)),
            Rotate(new Vec2( hw,  hh)),
            Rotate(new Vec2(-hw,  hh))
        };
    }
}

public class ColorBatch : BaseBatch<ColorBatchItem>
{
    public ColorBatch(Batcher batcher, Buffers buffers) : base(batcher, buffers)
    {
        
    }

    public void DrawRects(float[] vertices, int figuresCount)
    {
        if (isDirty || !m_LastItem.CanAdd(1))
        {
            m_LastItem = new ColorBatchItem(buffers, Shaders.colorBatchShader, m_ProjView);
            batcher.Add(m_LastItem);
            isDirty = false;
        }
        m_LastItem.Add(vertices, figuresCount);
    }
    public void DrawRects(Vec2[] positions, int figuresCount)
    {
        float[] vertices = new float[figuresCount * ColorBatchItem.VerticesPerFigure];
        for (int i = 0; i < figuresCount * IBatchItem.VertexesPerFigure; i++)
        {
            int verticeOffset = i * ColorBatchItem.VerticesPerVertex;

            vertices[verticeOffset + 0] = positions[i].X;
            vertices[verticeOffset + 1] = positions[i].Y;

            vertices[verticeOffset + 2] = m_Color.Rf;
            vertices[verticeOffset + 3] = m_Color.Gf;
            vertices[verticeOffset + 4] = m_Color.Bf;
            vertices[verticeOffset + 5] = m_Color.Af;
        }
        DrawRects(vertices, figuresCount);
    }
    public void DrawRect(float x, float y, float w, float h, float r)
    {
        DrawRects(GetRectPositions(x, y, w, h, r), 1);
    }
    public void DrawRect(float x, float y, float w, float h)
    {
        DrawRects(GetRectPositions(x, y, w, h), 1);
    }
}

public class SpriteBatch : BaseBatch<SpriteBatchItem>
{
    private Sprite m_Sprite = null!;
    public Sprite Sprite
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Sprite;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (m_Sprite == null || m_Sprite.Texture != value.Texture)
            {
                SetDirty();                
            }
            m_Sprite = value;
        }
    }
    
    public SpriteBatch(Batcher batcher, Buffers buffers) : base(batcher, buffers)
    {
        
    }

    public void DrawSprites(float[] vertices, int figuresCount)
    {
        if (isDirty || !m_LastItem.CanAdd(1))
        {
            m_LastItem = new SpriteBatchItem(buffers, Shaders.spriteBatchShader, m_ProjView, m_Sprite.Texture);
            batcher.Add(m_LastItem);
            isDirty = false;
        }
        m_LastItem.Add(vertices, figuresCount);
    }
    public void DrawSprites(Vec2[] positions, Vec2[] texCoords, int figuresCount)
    {
        float[] vertices = new float[figuresCount * SpriteBatchItem.VerticesPerFigure];
        for (int i = 0; i < figuresCount * IBatchItem.VertexesPerFigure; i++)
        {
            int verticeOffset = i * SpriteBatchItem.VerticesPerVertex;

            vertices[verticeOffset + 0] = positions[i].X;
            vertices[verticeOffset + 1] = positions[i].Y;

            vertices[verticeOffset + 2] = m_Color.Rf;
            vertices[verticeOffset + 3] = m_Color.Gf;
            vertices[verticeOffset + 4] = m_Color.Bf;
            vertices[verticeOffset + 5] = m_Color.Af;
            
            vertices[verticeOffset + 6] = texCoords[i].X;
            vertices[verticeOffset + 7] = texCoords[i].Y;
        }
        DrawSprites(vertices, figuresCount);
    }
    public void DrawSprite(float x, float y, float w, float h, float r)
    {
        DrawSprites(GetRectPositions(x, y, w, h, r), GetTexCoords(Sprite.UV), 1);
    }
    public void DrawSprite(float x, float y, float w, float h)
    {
        DrawSprites(GetRectPositions(x, y, w, h), GetTexCoords(Sprite.UV), 1);
    }

    public Vec2[] GetTexCoords(in Rect uv)
    {
        return new[]
        {
            new Vec2(uv.MinX, uv.MinY),
            new Vec2(uv.MaxX, uv.MinY),
            new Vec2(uv.MaxX, uv.MaxY),
            new Vec2(uv.MinX, uv.MaxY),
        };
    }
}

public class ColorBatchItem : BaseBatchItem
{
    public const int PositionVertices = 2;
    public const int PositionStride = PositionVertices * sizeof(float);
    public const int PositionOffset = 0;
    
    public const int ColorVertices = 4;
    public const int ColorStride = ColorVertices * sizeof(float);
    public const int ColorOffset = PositionOffset + PositionStride;

    public const int VerticesPerVertex = PositionVertices + ColorVertices;
    public const int VerticesPerFigure = VerticesPerVertex * IBatchItem.VertexesPerFigure;
    public const int VerticesPerBatch = VerticesPerFigure * IBatchItem.FiguresPerBatch;
    public const int VertexStride = VerticesPerVertex * sizeof(float);

    public ColorBatchItem(Buffers buffers, Shader shader, Matrix4 projView) : base(buffers, shader, projView)
    {
        m_Vertices = new float[VerticesPerBatch];
    }

    public override void Add(float[] vertices, int figuresCount)
    {
        Add(vertices, figuresCount, m_FiguresCount * VerticesPerFigure, figuresCount * VerticesPerFigure);
    }

    public override void Flush()
    {
        BindAndUseAll();
        
        GL.VertexAttribPointer(0, PositionVertices, VertexAttribPointerType.Float, false, VertexStride, PositionOffset);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, ColorVertices, VertexAttribPointerType.Float, false, VertexStride, ColorOffset);
        GL.EnableVertexAttribArray(1);
        
        LoadAndDraw(m_FiguresCount *  VerticesPerFigure);
    }
}

public class SpriteBatchItem : BaseBatchItem
{
    public const int PositionVertices = 2;
    public const int PositionStride = PositionVertices * sizeof(float);
    public const int PositionOffset = 0;
    
    public const int ColorVertices = 4;
    public const int ColorStride = ColorVertices * sizeof(float);
    public const int ColorOffset = PositionOffset + PositionStride;

    public const int TexCoordVertices = 2;
    public const int TexCoordStride = TexCoordVertices * sizeof(float);
    public const int TexCoordOffset = ColorOffset + ColorStride;

    public const int VerticesPerVertex = PositionVertices + ColorVertices + TexCoordVertices;
    public const int VerticesPerFigure = VerticesPerVertex * IBatchItem.VertexesPerFigure;
    public const int VerticesPerBatch = VerticesPerFigure * IBatchItem.FiguresPerBatch;
    public const int VertexStride = VerticesPerVertex * sizeof(float);

    public Texture texture;
    
    public SpriteBatchItem(Buffers buffers, Shader shader, Matrix4 projView, Texture texture) : base(buffers, shader, projView)
    {
        m_Vertices = new float[VerticesPerBatch];
        this.texture = texture;
    }

    public override void Add(float[] vertices, int figuresCount)
    {
        Add(vertices, figuresCount, m_FiguresCount * VerticesPerFigure, figuresCount * VerticesPerFigure);
    }

    public override void Flush()
    {
        BindAndUseAll();
        
        texture.Bind(0);
        
        GL.VertexAttribPointer(0, PositionVertices, VertexAttribPointerType.Float, false, VertexStride, PositionOffset);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, ColorVertices, VertexAttribPointerType.Float, false, VertexStride, ColorOffset);
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, TexCoordVertices, VertexAttribPointerType.Float, false, VertexStride, TexCoordOffset);
        GL.EnableVertexAttribArray(2);
        
        LoadAndDraw(m_FiguresCount *  VerticesPerFigure);
    }
}

public abstract class BaseBatchItem : IBatchItem
{
    protected int m_FiguresCount = 0;
    protected float[] m_Vertices;
    
    public Shader shader;
    public Buffers buffers;
    public Matrix4 projView;

    public BaseBatchItem(Buffers buffers, Shader shader, Matrix4 projView)
    {
        this.buffers = buffers;
        this.shader = shader;
        this.projView = projView;
    }
    
    public bool CanAdd(int figuresCount)
    {
        return m_FiguresCount + figuresCount <= IBatchItem.FiguresPerBatch;
    }

    public void BindAndUseAll()
    {
        buffers.Bind();        
        shader.Use();
        shader.SetUniform("uProjView", ref projView);
    }

    public void LoadAndDraw(int verticesCount)
    {
        int indicesCount = m_FiguresCount * IBatchItem.IndicesPerFigure;
        
        GL.BufferData(BufferTarget.ArrayBuffer, verticesCount * sizeof(float), m_Vertices, BufferUsage.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indicesCount * sizeof(uint), IBatchItem.indices, BufferUsage.StaticDraw);
        
        GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
    }

    public abstract void Flush();

    public abstract void Add(float[] vertices, int figuresCount);

    public void Add(float[] vertices, int figuresCount, int offset, int verticesCount)
    {
        for (int i = 0; i < verticesCount; i++)
        {
            m_Vertices[offset + i] = vertices[i];
        }
        m_FiguresCount += figuresCount;
    }
}

public interface IBatchItem
{
    public const int FiguresPerBatch = 512;
    public const int VertexesPerFigure = 4;
    public const int IndicesPerFigure = 6;
    
    public static uint[] indices;

    public static void Init()
    {
        indices = new uint[FiguresPerBatch * IndicesPerFigure];
        
        for (int figure = 0; figure < FiguresPerBatch; figure++)
        {
            int indiceOffset = figure * IndicesPerFigure;
            uint vertexOffset = (uint)(figure * VertexesPerFigure);

            indices[indiceOffset + 0] = vertexOffset + 0;
            indices[indiceOffset + 1] = vertexOffset + 1;
            indices[indiceOffset + 2] = vertexOffset + 2;
            
            indices[indiceOffset + 3] = vertexOffset + 2;
            indices[indiceOffset + 4] = vertexOffset + 3;
            indices[indiceOffset + 5] = vertexOffset + 0;
        }
    }
    
    public void Flush();
    public bool CanAdd(int figuresCount);
    public void Add(float[] vertices, int figuresCount);
}

public class Sprite
{
    private Rect m_UV;
    private RectInt m_Region;
    
    private Texture m_Texture;
    
    public Rect UV
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_UV;
        }
    }
    public RectInt Region
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Region;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Region = value;
            Recalculate();
        }
    }

    public Texture Texture
    {
        get
        {
            return m_Texture;
        }
    } 
    
    public Sprite(Texture texture)
    {
        m_Texture = texture;
        Region = RectInt.FromBounds(0, 0, texture.image.size.X, texture.image.size.Y);
    }

    public Sprite(string path) : this(new Texture(Image.LoadFromFile(path)))
    {
        
    }

    public void Recalculate()
    {
        float maxX = m_Texture.image.size.X;
        float maxY = m_Texture.image.size.Y;
        
        m_UV = Rect.FromBounds(m_Region.MinX / maxX, m_Region.MinY / maxY, 
            m_Region.MaxX / maxX, m_Region.MaxY / maxY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return m_Texture != null && m_Texture.IsValid();
    }

    public static bool IsValid(Sprite sprite)
    {
        return sprite != null && sprite.IsValid();
    }
}

public class Texture
{
    public Image image;
    public int handle;

    public Texture(Image image)
    {
        this.image = image;
        
        handle = GL.GenTexture();
        
        Bind(0);

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.size.X, image.size.Y,
            0, PixelFormat.Rgba, PixelType.UnsignedByte, image.data);

        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Bind(int init)
    {
        GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + init));
        GL.BindTexture(TextureTarget.Texture2d, handle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return image != null;
    }
}