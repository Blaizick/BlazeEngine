using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;

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
        Draw.Construct(buffers, batcher, colorBatch);
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

public static class Draw
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
            colorBatch.color = s_Color;
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

    public static Buffers buffers;
    public static Batcher batcher;
    public static ColorBatch colorBatch;

    public static Matrix4 ProjView
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return colorBatch.ProjView;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            colorBatch.ProjView = value;
        }
    }
    
    public static void Construct(Buffers buffers, Batcher batcher, ColorBatch colorBatch)
    {
        Draw.buffers = buffers;
        Draw.batcher = batcher;
        Draw.colorBatch = colorBatch;
    }

    public static void Flush()
    {
        batcher.Flush();
    }
    
    public static void Rect(float x, float y, float w, float h)
    {
        colorBatch.DrawRect(x, y, w, h);
    }

    public static void Rect(float x, float y, float w, float h, float r)
    {
        colorBatch.DrawRect(x, y, w, h, r);
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
    
    public static void Init()
    {
        colorBatchShader = new Shader("#version 330 core\n" +
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
    }
}

public interface IBatch
{
    public void SetDirty();
}

public class ColorBatch : IBatch
{
    private ColorBatchItem m_LastItem;
    public bool isDirty = true;
    public Batcher batcher;
    public Buffers buffers;
    
    private Matrix4 m_ProjView = Matrix4.Identity;
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
    
    public Color color = Color.White;
    
    public ColorBatch(Batcher batcher, Buffers buffers)
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

    public void DrawRect(float[] vertices, int figuresCount)
    {
        if (isDirty || !m_LastItem.CanAdd(1))
        {
            m_LastItem = new ColorBatchItem(buffers, Shaders.colorBatchShader, m_ProjView);
            batcher.Add(m_LastItem);
            isDirty = false;
        }
        m_LastItem.Add(vertices, figuresCount);
    }
    public void DrawRect(Vec2[] positions, int figuresCount)
    {
        float[] vertices = new float[figuresCount * ColorBatchItem.VerticesPerFigure];
        for (int i = 0; i < figuresCount * IBatchItem.VertexesPerFigure; i++)
        {
            int verticeOffset = i * ColorBatchItem.VerticesPerVertex;

            vertices[verticeOffset + 0] = positions[i].X;
            vertices[verticeOffset + 1] = positions[i].Y;

            vertices[verticeOffset + 2] = color.Rf;
            vertices[verticeOffset + 3] = color.Gf;
            vertices[verticeOffset + 4] = color.Bf;
            vertices[verticeOffset + 5] = color.Af;
        }
        DrawRect(vertices, figuresCount);
    }
    public void DrawRect(float x, float y, float w, float h, float r)
    {
        Matrix4 mat = Matrix4.CreateTranslation(new Vector3(x, y, 0)) * 
                      Matrix4.CreateRotationZ(r * Mathf.Deg2Rad) * 
                      Matrix4.CreateScale(new Vector3(w, h, 1));
        float hw = 0.5f;
        float hh = 0.5f;
        DrawRect(new[]
        {
            (Vec2)(mat * new Vector4(-hw, -hh, 0, 1)),
            (Vec2)(mat * new Vector4(hw, -hh, 0, 1)),
            (Vec2)(mat * new Vector4(hw, hh, 0, 1)),
            (Vec2)(mat * new Vector4(-hw, hh, 0, 1))
        }, 1);
    }
    public void DrawRect(float x, float y, float w, float h)
    {
        float hw = w * 0.5f;
        float hh = h * 0.5f;
        DrawRect(new[]
        {
            new Vec2(x - hw, y - hh),
            new Vec2(x + hw, y - hh),
            new Vec2(x + hw, y + hh),
            new Vec2(x - hw, y + hh),
        }, 1);
    }
}

public class ColorBatchItem : IBatchItem
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

    public Shader shader;
    public Buffers buffers;
    public Matrix4 projView;

    private int m_FiguresCount = 0;
    
    private float[] m_Vertices = new float[VerticesPerBatch];
    
    public ColorBatchItem(Buffers buffers, Shader shader, Matrix4 projView)
    {
        this.buffers = buffers;
        this.shader = shader;
        this.projView = projView;
    }

    public void Add(float[] vertices, int figuresCount)
    {
        int offset = m_FiguresCount * VerticesPerFigure;
        for (int i = 0; i < figuresCount * VerticesPerFigure; i++)
        {
            m_Vertices[offset + i] = vertices[i];
        }
        m_FiguresCount += figuresCount;
    }

    public bool CanAdd(int figuresCount)
    {
        return m_FiguresCount + figuresCount <= IBatchItem.FiguresPerBatch;
    }
    
    public void Flush()
    {
        int verticesCount = m_FiguresCount * VerticesPerFigure;
        int indicesCount = m_FiguresCount * IBatchItem.IndicesPerFigure;
        
        buffers.Bind();        
        shader.Use();

        shader.SetUniform("uProjView", ref projView);
        
        GL.VertexAttribPointer(0, PositionVertices, VertexAttribPointerType.Float, false, VertexStride, PositionOffset);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, ColorVertices, VertexAttribPointerType.Float, false, VertexStride, ColorOffset);
        GL.EnableVertexAttribArray(1);
        
        GL.BufferData(BufferTarget.ArrayBuffer, verticesCount * sizeof(float), m_Vertices, BufferUsage.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indicesCount * sizeof(uint), IBatchItem.indices, BufferUsage.StaticDraw);
        
        GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
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

    public void Recalculate()
    {
        float maxX = m_Texture.image.size.X;
        float maxY = m_Texture.image.size.Y;
        
        m_UV = Rect.FromBounds(m_Region.MinX / maxX, m_Region.MinY / maxY, 
            m_Region.MaxX / maxX, m_Region.MaxY / maxY);
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
}

public class Image
{
    public Vec2Int size;
    public byte[] data;

    public Image(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        using (var stream = File.Open(path, FileMode.Open))
        {
            var stbImage = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            size = new Vec2Int(stbImage.Width, stbImage.Height);
            data = stbImage.Data;
        }
    }
}