using System.Numerics;

namespace Engine;

/// <summary>
/// Engine-agnostic physics façade. The default implementation is provided by
/// <c>BepuPhysicsPlugin</c> in <c>Engine.Physics.Bepu</c> and inserted into the world
/// as the <see cref="IPhysicsWorld"/> resource.
/// </summary>
/// <remarks>
/// Resolve via <see cref="BehaviorContext.Physics"/> from a behaviour, or
/// <c>world.Resource&lt;IPhysicsWorld&gt;()</c> from a system.
/// </remarks>
public interface IPhysicsWorld
{
    /// <summary>Global gravity acceleration applied to every dynamic body.</summary>
    Vector3 Gravity { get; set; }

    // ---- Dynamic body creation -----------------------------------------

    /// <summary>Creates a dynamic sphere body.</summary>
    PhysicsBody CreateSphere(Vector3 position, float radius, float mass = 1f, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates a dynamic axis-aligned box body sized by <paramref name="halfExtents"/>.</summary>
    PhysicsBody CreateBox(Vector3 position, Vector3 halfExtents, float mass = 1f, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates a dynamic capsule body whose long axis is local-Y.</summary>
    PhysicsBody CreateCapsule(Vector3 position, float radius, float height, float mass = 1f, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates a dynamic cylinder body whose long axis is local-Y.</summary>
    PhysicsBody CreateCylinder(Vector3 position, float radius, float height, float mass = 1f, PhysicsMaterial? material = null, int entityId = 0);

    // ---- Static creation -----------------------------------------------

    /// <summary>Creates an immovable sphere collider.</summary>
    PhysicsBody CreateStaticSphere(Vector3 position, float radius, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates an immovable axis-aligned box collider.</summary>
    PhysicsBody CreateStaticBox(Vector3 position, Vector3 halfExtents, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates an immovable capsule collider whose long axis is local-Y.</summary>
    PhysicsBody CreateStaticCapsule(Vector3 position, float radius, float height, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates an immovable triangle-mesh collider.</summary>
    PhysicsBody CreateStaticMesh(Vector3 position, ReadOnlySpan<Vector3> vertices, ReadOnlySpan<int> indices, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates a flat horizontal ground plane (XZ plane at <paramref name="y"/>).</summary>
    PhysicsBody CreateGroundPlane(float y = 0f, float halfSize = 500f, PhysicsMaterial? material = null, int entityId = 0);

    // ---- Kinematic creation --------------------------------------------

    /// <summary>Creates a kinematic sphere (moved by code, infinite mass).</summary>
    PhysicsBody CreateKinematicSphere(Vector3 position, float radius, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates a kinematic box (moved by code, infinite mass).</summary>
    PhysicsBody CreateKinematicBox(Vector3 position, Vector3 halfExtents, PhysicsMaterial? material = null, int entityId = 0);
    /// <summary>Creates a kinematic capsule (moved by code, infinite mass).</summary>
    PhysicsBody CreateKinematicCapsule(Vector3 position, float radius, float height, PhysicsMaterial? material = null, int entityId = 0);

    // ---- Body operations (called by PhysicsBody) -----------------------

    /// <summary>Returns whether the underlying body still exists.</summary>
    bool Exists(PhysicsBody body);
    /// <summary>Removes the body from the simulation.</summary>
    void Destroy(PhysicsBody body);

    /// <summary>Reads world-space position.</summary>
    Vector3 GetPosition(PhysicsBody body);
    /// <summary>Reads world-space orientation.</summary>
    Quaternion GetRotation(PhysicsBody body);
    /// <summary>Writes world-space position. Wakes dynamic bodies.</summary>
    void SetPosition(PhysicsBody body, Vector3 position);
    /// <summary>Writes world-space orientation. Wakes dynamic bodies.</summary>
    void SetRotation(PhysicsBody body, Quaternion rotation);

    /// <summary>Reads linear velocity (returns zero for static bodies).</summary>
    Vector3 GetLinearVelocity(PhysicsBody body);
    /// <summary>Reads angular velocity (returns zero for static bodies).</summary>
    Vector3 GetAngularVelocity(PhysicsBody body);
    /// <summary>Sets linear velocity (no-op for static bodies). Wakes the body.</summary>
    void SetLinearVelocity(PhysicsBody body, Vector3 velocity);
    /// <summary>Sets angular velocity (no-op for static bodies). Wakes the body.</summary>
    void SetAngularVelocity(PhysicsBody body, Vector3 velocity);

    /// <summary>Applies an instantaneous linear impulse, optionally offset from the centre of mass.</summary>
    void ApplyImpulse(PhysicsBody body, Vector3 impulse, Vector3 offsetFromCenter);
    /// <summary>Applies an instantaneous angular impulse.</summary>
    void ApplyAngularImpulse(PhysicsBody body, Vector3 impulse);

    /// <summary>Whether the body is currently awake (being simulated).</summary>
    bool IsAwake(PhysicsBody body);
    /// <summary>Forces the body awake.</summary>
    void Wake(PhysicsBody body);
    /// <summary>Forces the body asleep.</summary>
    void Sleep(PhysicsBody body);

    // ---- Queries --------------------------------------------------------

    /// <summary>Casts a ray against every body in the world; reports the closest hit.</summary>
    bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit hit);

    // ---- Stepping (internal, called by the scheduler) ------------------

    /// <summary>Advances the simulation by <paramref name="deltaSeconds"/>. Called by the scheduler.</summary>
    void Step(float deltaSeconds);

    /// <summary>Writes simulated body poses back to the matching <see cref="Transform"/> components.</summary>
    void SyncTransforms(EcsWorld ecs);
}

