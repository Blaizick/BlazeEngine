using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using VelcroPhysics;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;

namespace BlazeEngine;

public class PhysicsCore
{
    public World world;

    public void Init()
    {
        world = new(new(0, 0));
        Physics.Construct(this);
    }

    public void FixedUpdate()
    {
        world.Step(Time.TimeSinceLastFixedUpdate);
    }
}

public class Rigidbody
{
    public Collider collider;

    public Rigidbody(Collider collider)
    {
        this.collider = collider;
    }

    public Vec2 LinearVelocity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => collider.body.LinearVelocity;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => collider.body.LinearVelocity = value;
    }
    public float AngularVelocity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => collider.body.AngularVelocity;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => collider.body.AngularVelocity = value;
    }
}


public class Collider
{
    public Body body;
    
    private TransformData m_LastSyncData;
    private TransformData m_SyncData;

    public void SyncToWorld(in Vec2 position, float rotation)
    {
        body.Position = position;
        body.Rotation = rotation;
    }
    public void SyncToWorld(in TransformData transformData)
    {
        body.Position = transformData.position;
        body.Rotation = transformData.rotation;
    }
    public TransformData SyncFromWorld()
    {
        m_LastSyncData = m_SyncData;
        m_SyncData = new TransformData(body.Position, body.Rotation);
        return m_SyncData;
    }
    public void SyncFromWorld(ref Vec2 position, ref float rotation)
    {
        m_LastSyncData = m_SyncData;
        m_SyncData = new TransformData(body.Position, body.Rotation);
        position = body.Position;
        rotation = body.Rotation;
    }
    
    
    public TransformData GetInterpolationData()
    {
        return new TransformData(Vec2.Lerp(m_LastSyncData.position, m_SyncData.position, Time.PercentsToNextFixedUpdate), 
            Mathf.Lerp(m_LastSyncData.rotation, m_SyncData.rotation, Time.PercentsToNextFixedUpdate));
    }

    public void Interpolate(ref Vec2 position, ref float rotation)
    {
        position = Vec2.Lerp(m_LastSyncData.position, m_SyncData.position, Time.PercentsToNextFixedUpdate);
        rotation = Mathf.Lerp(m_LastSyncData.rotation, m_SyncData.rotation, Time.PercentsToNextFixedUpdate);
    }
        
    public struct TransformData
    {
        public Vec2 position;
        public float rotation;

        public TransformData(Vec2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}

public class BoxCollider : Collider
{
    public BoxCollider(float w, float h, float density = 1.0f, BodyType bodyType = BodyType.Dynamic)
    {
        body = BodyFactory.CreateRectangle(Physics.physicsCore.world, w, h, density);
        body.BodyType = bodyType;
    }
}

public class CircleCollider : Collider
{
    public CircleCollider(float radius, float density = 1.0f, BodyType bodyType = BodyType.Dynamic) 
    {
        body = BodyFactory.CreateCircle(Physics.physicsCore.world, radius, density);
        body.BodyType = bodyType;
    }
}


public static class Physics
{
    public const float FixedUpdatesPerSecond = 50.0f;
    public const float FixedUpdateDelay = 1.0f / FixedUpdatesPerSecond;

    public static void Construct(PhysicsCore physicsCore)
    {
        Physics.physicsCore = physicsCore;
    }
    
    public static PhysicsCore physicsCore;
}