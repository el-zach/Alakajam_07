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

    private void Start()
    {
        manager = World.Active.EntityManager;
        toSpawn = Game.Instance.bowMan;
        zeroPlane = new Plane(Vector3.up,Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(spawnButton))
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Entity newEntity = manager.CreateEntity(toSpawn.archetype);
        float size = UnityEngine.Random.Range(0.8f, 1.3f);
        toSpawn.InitAppearance(manager, newEntity, size);
        float3 pos = WorldFromMouse();
        manager.SetComponentData(newEntity, new Translation { Value = pos +new float3(0f,size*0.5f,0f) });
        Avoid.Instance.AddAvoidPoint(pos, size, UnityEngine.Random.Range(1f, 40f));
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
