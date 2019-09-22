using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Collections;

public class Avoid : MonoBehaviour
{
    public struct Point : IComponentData
    {
        public float3 Position;
        public float RangeSqr;
    }


    EntityManager manager;
    public float range=10f;
    // Start is called before the first frame update
    void Start()
    {
        manager = World.Active.EntityManager;
        foreach(Transform child in transform)
        {
            var newEntity = manager.CreateEntity(typeof(Point));
            manager.SetComponentData(newEntity, new Point { Position = child.position, RangeSqr = math.pow(range,2f) });
        }
    }

    private void OnDrawGizmos()
    {
        foreach(Transform child in transform)
        {
            Gizmos.DrawWireSphere(child.position, range);
        }
    }
}
