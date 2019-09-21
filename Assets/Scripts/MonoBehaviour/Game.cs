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
    public class UnitSettings
    {
        public Mesh mesh;
        public Material material;
        public EntityArchetype archetype;

        public void InitAppearance(EntityManager _manager, Entity _entity, float _size=1f)
        {
            _manager.SetSharedComponentData(_entity,new RenderMesh{ mesh = mesh,material = material});
            _manager.SetComponentData(_entity, new Scale { Value = 1f });
        }
    }

    EntityManager manager;
    public UnitSettings enemy, spawner, bowMan, knight, projectile;

    //----------EntityTags---------//
    public struct Enemy : IComponentData { }
    public struct SpawnPoint : IComponentData { }

    //----------PhysicComponents-------//
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
    public struct Acceleration : IComponentData
    {
        public float3 Value;
    }

    //---------GameComponents---------//
    public struct Health : IComponentData
    {
        public float Value;
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
    }

    [ContextMenu("Test Spawn")]
    public void TestSpawnEnemy()
    {
        var newUnit = manager.CreateEntity(enemy.archetype);
        manager.SetComponentData(newUnit,
            new Translation
            {
                Value = new float3(
                    UnityEngine.Random.Range(-5f, 5f),
                    0f,
                    UnityEngine.Random.Range(-5f, 5f)
                    )
            });
        enemy.InitAppearance(manager, newUnit);
    }

}
