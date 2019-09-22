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
    public static Avoid Instance;
    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public struct Point : IComponentData
    {
        public float3 Position;
        public float RangeSqr;
        public float Power;
    }


    EntityManager manager;
    public float range=10f;
    public float power = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        manager = World.Active.EntityManager;
        foreach(Transform child in transform)
        {
            AddPointEntity(child);
        }
    }

    public void AddAvoidPoint(Vector3 _worldPos, float _range = 1f, float _power = 1f)
    {
        GameObject managedObject = new GameObject();
        managedObject.transform.SetParent(this.transform);
        managedObject.transform.position = _worldPos;
        managedObject.transform.localScale = Vector3.one * _range;
        managedObject.transform.localRotation = Quaternion.Euler(0f, _power, 0f);
        AddPointEntity(managedObject.transform);
    }

    void AddPointEntity(Transform child)
    {
        var newEntity = manager.CreateEntity(typeof(Point));
        manager.SetComponentData(newEntity,
            new Point
            {
                Position = child.position,
                RangeSqr = math.pow(range * child.localScale.x, 2f),
                Power = child.localRotation.eulerAngles.y * power
            });
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
