using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

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
                typeof(Translation)
            );
    }

    [ContextMenu("Execute")]
    void Execute()
    {
        for (int i = 0; i < count; i++)
        {
            Entity entity = entityManager.CreateEntity(BasicUnit);
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });
        }
    }

}
