using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Collections;

public class Test : MonoBehaviour
{

    public int count = 1;
    public Mesh mesh;
    public Material material;

    EntityManager entityManager;
    EntityArchetype BasicUnit;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        Execute();
    }
    
    void Init()
    {
        entityManager = World.Active.EntityManager;
        BasicUnit = entityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale) //could also be typeof(NonUniformScale) 
            );
    }

    [ContextMenu("Execute")]
    void Execute() // exists in Monobehaviour space
    {
        NativeArray<Entity> entityArray = new NativeArray<Entity>(count,Allocator.Temp); //we dont keep track of the array so having it disposable is good (?)

        entityManager.CreateEntity(BasicUnit,entityArray);

        foreach (Entity entity in entityArray)
        {
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });
            entityManager.SetComponentData(entity, new Translation
            {
                Value = new float3(0f, UnityEngine.Random.value, 0f)
            });
            entityManager.SetComponentData(entity, new Scale
            {
                Value = 1f
            });
        }
    }

}

public class MovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(
            (ref Translation translation) => //anonymous action
        {
            if(translation.Value.y<=10f)
                translation.Value += new float3(0f, 1f, 0f);
        });
    }
}

public class RotatorSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation) =>
        {
            rotation.Value = quaternion.Euler(0f, 0f, math.PI * Time.realtimeSinceStartup);
        });
    }
}