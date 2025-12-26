using BlazeEngine;
using SDL;
using VelcroPhysics;

public class GameCore : IGameCore, IFixedUpdateGameCore
{
    Player player = new ();
    public void Init()
    {
        Sprites.Init();
        player.Init();
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
        
        player.Update();
    }

    public void Draw()
    {
        player.Draw();
    }

    public void Quit()
    {
        
    }

    public void PreFixedUpdate()
    {
        player.PreFixedUpdate();
    }

    public void FixedUpdate()
    {
        
    }

    public void PostFixedUpdate()
    {
        player.PostFixedUpdate();
    }
}

public class Player
{
    public Sprite sprite;

    public Vec2 position;
    public float rotation;

    public Rigidbody rb;
    public BoxCollider collider;
    
    public void Init()
    {
        collider = new(1.0f, 1.0f);
        rb = new(collider);
    }
    
    public void Update()
    {
        rb.AngularVelocity = Mathf.MoveTowards(rb.AngularVelocity, 0.0f, 40.0f * Time.Delta);
        if (Input.IsKeyDown(SDL_Keycode.SDLK_RIGHT))
        {
            rb.AngularVelocity = -480.0f;
        }
        if (Input.IsKeyDown(SDL_Keycode.SDLK_LEFT))
        {
            rb.AngularVelocity = 480.0f;
        }
        
        rb.LinearVelocity = Vec2.MoveTowards(rb.LinearVelocity, Vec2.Zero, 2f * Time.Delta);
        if (Input.IsKeyDown(SDL_Keycode.SDLK_UP))
        {
            rb.LinearVelocity = Vec2.GetFromAngle(rotation) * 4;
        }
        if (Input.IsKeyDown(SDL_Keycode.SDLK_DOWN))
        {
            rb.LinearVelocity = Vec2.GetFromAngle(rotation) * -4;
        }

        collider.Interpolate(ref position, ref rotation);
    }

    public void PreFixedUpdate()
    {
        collider.SyncToWorld(position, rotation);
    }
    public void PostFixedUpdate()
    {
        collider.SyncFromWorld(ref position, ref rotation);
    }
    
    public void Draw()
    {
        BlazeEngine.Draw.Color = Color.White;
        BlazeEngine.Draw.sprite = Sprites.player0;
        BlazeEngine.Draw.Sprite(position.X, position.Y, 1, 1, rotation);
    }
}

public static class Sprites
{
    public static Sprite bigAsteroid0;
    public static Sprite bigAsteroid1;
    public static Sprite bigAsteroid2;
    
    public static Sprite mediumAsteroid0;
    public static Sprite mediumAsteroid1;
    public static Sprite mediumAsteroid2;
    
    public static Sprite smallAsteroid0;
    public static Sprite smallAsteroid1;
    public static Sprite smallAsteroid2;

    public static Sprite player0;
    public static Sprite player1;

    public static Sprite ufo;

    public static void Init()
    {
        bigAsteroid0 = new Sprite(@"./Sprites/BigAsteroid0.png");
        bigAsteroid1 = new Sprite(@"./Sprites/BigAsteroid1.png");
        bigAsteroid2 = new Sprite(@"./Sprites/BigAsteroid2.png");

        mediumAsteroid0 = new Sprite(@"./Sprites/MediumAsteroid0.png");
        mediumAsteroid1 = new Sprite(@"./Sprites/MediumAsteroid1.png");
        mediumAsteroid2 = new Sprite(@"./Sprites/MediumAsteroid2.png");
        
        smallAsteroid0 = new Sprite(@"./Sprites/SmallAsteroid0.png");
        smallAsteroid1 = new Sprite(@"./Sprites/SmallAsteroid1.png");
        smallAsteroid2 = new Sprite(@"./Sprites/SmallAsteroid2.png");
        
        player0 = new Sprite(@"./Sprites/Player0.png");
        player1 = new Sprite(@"./Sprites/Player1.png");
        
        ufo = new Sprite(@"./Sprites/UFO.png");
    }
}