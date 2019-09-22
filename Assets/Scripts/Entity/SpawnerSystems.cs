using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

public class SpawnerSystems 
{
    static EntityManager manager;
    public class Spawner_MainThread : ComponentSystem
    {

        protected override void OnUpdate()
        {
            if (manager == null) manager = World.Active.EntityManager;
            List<float3> spawnAt = new List<float3>();
            //List<Entity> toDestroy = new List<Entity>();
            Entities.ForEach((Entity entity, ref Game.SpawnPoint spawner, ref Translation position) => 
            {
                spawner.Timer += Time.deltaTime;
                if(spawner.Timer >= spawner.Frequency)
                {
                    spawnAt.Add(position.Value);
                    spawner.Timer = 0f;
                    spawner.Count--;
                    if(spawner.Count == 0)
                    {
                        //toDestroy.Add(entity);
                        PostUpdateCommands.DestroyEntity(entity);
                        //queue destruction spawner
                    }
                }
            });

            foreach (float3 pos in spawnAt)
            {
                //Entity entity = manager.CreateEntity(Game.Instance.enemy.archetype);
                //Game.Instance.enemy.InitAppearance(manager, entity);
                //Game.Instance.enemy.InitStats(manager, entity);
                //manager.SetComponentData(entity, new Translation
                //{
                //    Value = pos
                //});
                Game.Instance.SpawnEnemy(pos,true);
            }
        }

    }

}
