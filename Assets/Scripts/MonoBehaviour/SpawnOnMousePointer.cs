using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Collections;

public class SpawnOnMousePointer : MonoBehaviour
{
    EntityManager manager;

    Game.UnitSettings toSpawn;

    public KeyCode spawnButton;

    public int cost = 35;

    private void Start()
    {
        manager = World.Active.EntityManager;
        toSpawn = Game.Instance.knight;
        zeroPlane = new Plane(Vector3.up,Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(spawnButton))
        {
            if (PlayerManager.Instance.gold >= cost)
            {
                Spawn();
                PlayerManager.Instance.Pay(cost);
            }
        }
    }

    void Spawn()
    {
        Entity newEntity = manager.CreateEntity(toSpawn.archetype);
        float size = toSpawn.sizeCurve.Evaluate(UnityEngine.Random.value);
        toSpawn.InitAppearance(manager, newEntity, Vector3.one*size);
        float3 pos = WorldFromMouse();
        manager.SetComponentData(newEntity, new Translation { Value = pos +new float3(0f,size*0.5f,0f) });
        float _health = toSpawn.health.Evaluate(UnityEngine.Random.value);
        manager.SetComponentData(newEntity, new Game.Health { Max = _health, Current = _health });
        Avoid.Instance.AddAvoidPoint(pos, newEntity, 0.5f+size*0.2f, UnityEngine.Random.Range(1f, 20f));
    }

    Plane zeroPlane;

    float3 WorldFromMouse()
    {
        //float3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        zeroPlane.Raycast(ray, out distance);
        float3 position = ray.GetPoint(distance);
        return position;
    }

}
