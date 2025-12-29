using System.Reflection;
using BlazeEngine;
using SDL;
using VelcroPhysics;
using Random = BlazeEngine.Random;

public class GameCore : IGameCore, IFixedUpdateGameCore
{
    public Player player = new ();
    public AsteroidsRegistry asteroidsRegistry;
    public AsteroidsFactory asteroidsFactory;
    
    public void Init()
    {
        Sprites.Init();
        player.Init();
        Camera.main.Size = 3.0f;
        AsteroidTypes.Init();
        asteroidsRegistry = new();
        asteroidsFactory = new(asteroidsRegistry);
    }

    public int frames = 0;
    public float timeSinceLastTitleUpdate = 0.0f;
    public void Update()
    {
        frames++;
        timeSinceLastTitleUpdate += Time.Delta;
        if (timeSinceLastTitleUpdate > 1.0f)
        {
            Core.window.Title = $"FPS: {frames}";
            frames = 0;
            timeSinceLastTitleUpdate = 0.0f;
        }
        
        player.Update();
        asteroidsFactory.Update();
        asteroidsRegistry.Update();
    }

    public void Draw()
    {
        player.Draw();
        asteroidsRegistry.Draw();
    }

    public void Quit()
    {
        
    }

    public void PreFixedUpdate()
    {
        player.PreFixedUpdate();
        asteroidsRegistry.PreFixedUpdate();
    }

    public void FixedUpdate()
    {
    }

    public void PostFixedUpdate()
    {
        player.PostFixedUpdate();
        asteroidsRegistry.PostFixedUpdate();
    }
}

public class Player
{
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
        BlazeEngine.Drawf.Color = Color.White;
        BlazeEngine.Drawf.sprite = Sprites.player0;
        BlazeEngine.Drawf.Sprite(position.X, position.Y, 1, 1, rotation);
    }
}

public class AsteroidsRegistry
{
    public Seq<Asteroid> asteroids = new();

    public void Update()
    {
        foreach (var i in asteroids)
            i.Update();
    }

    public void Draw()
    {
        foreach (var i in asteroids)
            i.Draw();
    }
    
    public void PreFixedUpdate()
    {
        foreach (var i in asteroids)
            i.PreFixedUpdate();
    }

    public void PostFixedUpdate()
    {
        foreach (var i in asteroids)
            i.PostFixedUpdate();
    }
}

public class AsteroidsFactory
{
    public float lastSpawnTime = 0.0f;
    
    public AsteroidsRegistry registry;

    public AsteroidsFactory(AsteroidsRegistry registry)
    {
        this.registry = registry;
    }
    
    public void Update()
    {
        if (Time.Since(lastSpawnTime) >= 3.0f)
        {
            var asteroid = AsteroidTypes.all[Random.NextInt(0, AsteroidTypes.all.Length)].AsAsteroid();
            asteroid.Init();
            asteroid.rigidbody.LinearVelocity = new Vec2(Random.NextFloat(-1, 1), Random.NextFloat(-1, 1)).Normalized * 4.0f;
            registry.asteroids.Add(asteroid);
            lastSpawnTime = Time.time;
        }
    }
}

public class AsteroidType
{
    public Sprite[] sprites;
    public float size;
    public float colliderSize;
    
    public Sprite GetRandomSprite()
    {
        return sprites[Random.NextInt(0, sprites.Length)];
    }

    public Asteroid AsAsteroid()
    {
        return new Asteroid(GetRandomSprite(), size, colliderSize);
    }
}

public static class AsteroidTypes
{
    public static AsteroidType smallAsteroid;
    public static AsteroidType mediumAsteroid;
    public static AsteroidType bigAsteroid;

    public static AsteroidType[] all;
    
    public static void Init()
    {
        smallAsteroid = new AsteroidType
        {
            sprites = new[]
            {
                Sprites.smallAsteroid0,
                Sprites.smallAsteroid1,
                Sprites.smallAsteroid2
            },
            size = 0.75f,
            colliderSize = 0.75f * 0.5f
            
        };
        mediumAsteroid = new AsteroidType
        {
            sprites = new[]
            {
                Sprites.mediumAsteroid0,
                Sprites.mediumAsteroid1,
                Sprites.mediumAsteroid2
            },
            size = 1.0f,
            colliderSize = 1.0f * 0.5f
        };
        bigAsteroid = new AsteroidType
        {
            sprites = new[]
            {
                Sprites.bigAsteroid0,
                Sprites.bigAsteroid1,
                Sprites.bigAsteroid2
            },
            size = 1.5f,
            colliderSize = 1.5f * 0.5f
        };

        all = new[]
        {
            smallAsteroid,
            mediumAsteroid,
            bigAsteroid
        };
    }
}

public class Asteroid
{
    public Sprite sprite;
    public float size;
    public float colliderSize;

    public Vec2 position;
    public float rotation;

    public Rigidbody rigidbody;
    public CircleCollider collider;
    
    public Asteroid(Sprite sprite, float size, float colliderSize)
    {
        this.size = size;
        this.colliderSize = colliderSize;
        collider = new(colliderSize);
        rigidbody = new(collider);
        this.sprite = sprite;
    }
    
    public void Init()
    {
        
    }

    public void Update()
    {
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
        BlazeEngine.Drawf.Color = Color.White;
        BlazeEngine.Drawf.sprite = sprite;
        BlazeEngine.Drawf.Sprite(position.X, position.Y, size, size, rotation);
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