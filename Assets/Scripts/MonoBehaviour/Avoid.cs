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
        public float Power;
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
            manager.SetComponentData(newEntity, 
                new Point {
                    Position = child.position,
                    RangeSqr = math.pow(range*child.localScale.x,2f),
                    Power = child.localRotation.eulerAngles.y+1f
                });
        }
    }

    private void OnDrawGizmos()
    {
        foreach(Transform child in transform)
        {
            float value = 1f - Mathf.Abs(child.localRotation.eulerAngles.y) / 180f;
            Gizmos.color = new Color(1f,value, value);
            Gizmos.DrawWireSphere(child.position, range*child.localScale.x);
        }
    }
}
