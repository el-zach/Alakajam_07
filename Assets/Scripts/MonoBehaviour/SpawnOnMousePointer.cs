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
        toSpawn.InitAppearance(manager, newEntity, UnityEngine.Random.Range(0.8f, 1.3f));
        manager.SetComponentData(newEntity, new Translation { Value = WorldFromMouse() });
    }

    Plane zeroPlane;

    float3 WorldFromMouse()
    {
        //float3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        zeroPlane.Raycast(ray, out distance);
        float3 position = ray.GetPoint(distance)+Vector3.up*0.5f;
        return position;
    }

}
