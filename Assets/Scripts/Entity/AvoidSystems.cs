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

public class AvoidSystems
{

    static EntityManager manager;

    public class Avoid_MainThread : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (manager == null) manager = World.Active.EntityManager;

            //Entities.ForEach((Entity entity, ref Avoid.Point point) => 
            //{
                
            //});
        }
    }

    [UpdateBefore(typeof(UnitSystems.VelocityTranslation))]
    [UpdateAfter(typeof(UnitSystems.LimitedAccelerateVelocity))]
    public class AddAvoidance : JobComponentSystem
    {
        EntityQuery query;

        [RequireComponentTag(typeof(Game.Enemy))]
        public struct AddAvoidanceJob : IJobForEach<Game.Velocity, Translation>
        {
            [NativeDisableParallelForRestriction]
            [DeallocateOnJobCompletion]
            public NativeArray<ArchetypeChunk> chunks;
            public ArchetypeChunkComponentType<Avoid.Point> pointType;

            public float deltaTime;

            public void Execute(ref Game.Velocity velocity, ref Translation position)
            {
                for (int i=0; i < chunks.Length; i++)
                {
                    var points = chunks[i].GetNativeArray(pointType);
                    for(int j = 0; j < points.Length; j++)
                    {
                        float3 distanceVector = position.Value - points[j].Position;
                        distanceVector.y = 0f;
                        float distanceSqr = math.lengthsq(distanceVector);
                        if (distanceSqr <= points[j].RangeSqr)
                        {
                            velocity.Value +=
                                math.normalize(distanceVector)
                                * (1f - (distanceSqr / points[j].RangeSqr))
                                * points[j].Power //pushForce
                                * deltaTime;
                        }
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps = new AddAvoidanceJob
            {
                chunks = query.CreateArchetypeChunkArray(Allocator.TempJob),
                pointType = GetArchetypeChunkComponentType<Avoid.Point>(),
                deltaTime = Time.deltaTime
            }.Schedule(this, inputDeps);

            return inputDeps;
        }

        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(Avoid.Point));
        }

    }

}
