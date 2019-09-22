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

public class ObjectSystems
{
    //--------Tags------------//
    public struct Billboard : IComponentData {
        //public float TurnSpeed;
    }

    //--------Rendering-------//
    public class BillBoardTurn : JobComponentSystem
    {
        [RequireComponentTag(typeof(Billboard))]
        [BurstCompile]
        struct BillBoardJob : IJobForEach<Rotation,Translation>//,Billboard>
        {
            //public float deltaTime;
            public float3 cameraPosition;
            public void Execute(ref Rotation rotation, ref Translation position)//, ref Billboard billboard)
            {
                
                float3 targetDirection = cameraPosition - position.Value;
                targetDirection.y = 0f;
                rotation.Value = quaternion.LookRotation(-targetDirection, new float3(0f, 1f, 0f));
                //rotation.Value = quaternion.lerp(
                //    rotation.Value, 
                //    quaternion.LookRotation(targetDirection, new float3(0f, 1f, 0f)),
                //    deltaTime * billboard.TurnSpeed);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //var job = new BillBoardJob() { deltaTime = Time.deltaTime, cameraPosition = Camera.main.transform.position };
            var job = new BillBoardJob() { cameraPosition = Camera.main.transform.position };
            return job.Schedule(this, inputDeps);
        }
    }

}

