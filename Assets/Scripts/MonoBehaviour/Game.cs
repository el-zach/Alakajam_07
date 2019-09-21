using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Collections;

public class Game : MonoBehaviour
{
    [System.Serializable]
    public class ObjectSettings
    {
        public Mesh mesh;
        public Material material;
        public EntityArchetype archetype;

        public void InitAppearance(EntityManager _manager, Entity _entity, float _size=1f)
        {
            _manager.SetSharedComponentData(_entity,new RenderMesh{ mesh = mesh,material = material});
            _manager.SetComponentData(_entity, new Scale { Value = _size });
        }
    }
    [System.Serializable]
    public class UnitSettings : ObjectSettings
    {
        public float health;
        public float maxSpeed;
        public float acceleration;
        public float velocityDampen;

        public void InitStats(EntityManager _manager, Entity _entity)
        {
            _manager.SetComponentData(_entity, new Health { Value = health });
            _manager.SetComponentData(_entity, new Acceleration { Value = acceleration });
            _manager.SetComponentData(_entity, new DampenVelocity { Value = velocityDampen });
            _manager.SetComponentData(_entity, new MaxSpeed { Value = maxSpeed });
        }

    }

    EntityManager manager;
    public UnitSettings enemy, bowMan, knight, projectile;
    public ObjectSettings spawner;

    [Header("Test Settings")]
    public float spawnDistance = 10f;
    [Range(0.1f,2f)]
    public float spawnFrequency = 1f;
    public int spawnCount = 1;

    //----------EntityTags---------//
    public struct Enemy : IComponentData { }
    public struct SpawnPoint : IComponentData { }
    //public struct Billboard : IComponentData { }

    //----------PhysicComponents-------//
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
    public struct Acceleration : IComponentData
    {
        public float Value;
        public float3 Directed;
    }
    public struct MaxSpeed : IComponentData
    {
        public float Value;
    }
    public struct DampenVelocity : IComponentData
    {
        public float Value;
    }

    //---------GameComponents---------//
    public struct Health : IComponentData
    {
        public float Value;
    }
    public struct Target : IComponentData
    {
        public float3 Position;
    }

    public void Init()
    {
        manager = World.Active.EntityManager;
        //------setup Archetypes------//
        enemy.archetype = manager.CreateArchetype(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Scale), //could also be typeof(NonUniformScale) 
            typeof(Enemy),
            typeof(Velocity),
            typeof(Acceleration),
            typeof(DampenVelocity),
            typeof(MaxSpeed),
            typeof(Target),
            typeof(Health)
        );

        spawner.archetype = manager.CreateArchetype(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Scale),
            typeof(SpawnPoint)
        );
    }

    private void Start()
    {
        Init();
        TestSpawnEnemy();
        StartCoroutine(SpawnEveryFewSeconds(UnityEngine.Random.Range(0.2f, 3f)));
    }

    [ContextMenu("Test Spawn")]
    public void TestSpawnEnemy()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var newUnit = manager.CreateEntity(enemy.archetype);
            Vector3 spawnPoint = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward * spawnDistance + Vector3.up*0.5f;
            manager.SetComponentData(newUnit,
                new Translation
                {
                    Value = spawnPoint
                });
            enemy.InitAppearance(manager, newUnit);
            enemy.InitStats(manager, newUnit);
        }
    }

    IEnumerator SpawnEveryFewSeconds(float _time)
    {
        yield return new WaitForSeconds(_time);
        TestSpawnEnemy();
        StartCoroutine(SpawnEveryFewSeconds(UnityEngine.Random.Range(0.2f, 3f)*spawnFrequency));
    }

}
