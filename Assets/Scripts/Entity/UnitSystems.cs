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

public class UnitSystems 
{
    //static EntityManager manager;
    public class Unit_MainThread : ComponentSystem
    {
        protected override void OnUpdate()
        {
            //if (UnitSystems.manager == null) manager = World.Active.EntityManager;
            Entities.ForEach((Entity entity, ref Game.HasWalkingTarget target, ref Translation position, ref Game.Acceleration acceleration) => // DO IT ON THE MAINTHREAD BECAUSE WHY NOT RIGHT?!
            {
                //PostUpdateCommands.RemoveComponent(entity, typeof(RemoveTarget));
                float3 distance = target.Position - position.Value;
                distance.y = 0f;
                if (math.lengthsq(distance) <= 0.1f)
                {
                    PostUpdateCommands.RemoveComponent(entity, typeof(Game.HasWalkingTarget));
                    acceleration.Directed = new float3(0f, 0f, 0f);
                }
            });
        }
    }

    //---------------OperationTags----------------
    public struct RemoveTarget : IComponentData { }


    //---------------MovementSystems---------------
    public class AccelerateToTarget : JobComponentSystem
    {
        //private readonly EndFrameBarrier m_EndFrameBarrier;
        

        [BurstCompile]
        struct TargetingJob : IJobForEachWithEntity<Game.Acceleration, Game.HasWalkingTarget, Translation>
        {
            //public EntityCommandBuffer.Concurrent commands;

            public void Execute(Entity entity, int index, ref Game.Acceleration acceleration, [ReadOnly] ref Game.HasWalkingTarget target, [ReadOnly] ref Translation position)
            {
                float3 directed = target.Position - position.Value;
                //if (math.lengthsq(directed) >= 0.1f)
                //{
                    //commands.RemoveComponent(entity, typeof(Game.HasTarget));
                    //commands.RemoveComponent(index, entity, typeof(Game.HasTarget));
                //}
                directed = math.normalize(directed) * acceleration.Value;
                acceleration.Directed = new float3(directed.x, 0f, directed.z);
            }
        }

        //EndSimulationEntityCommandBufferSystem bufferSystem;

        //protected override void OnCreate()
        //{
        //    bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //    base.OnCreate();
        //}

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new TargetingJob();// { commands = bufferSystem.CreateCommandBuffer().ToConcurrent() };
            return job.Schedule(this, inputDeps);
        }
    }

    
    public class AccelerateVelocity : JobComponentSystem
    {
        [ExcludeComponent(typeof(Game.MaxSpeed))]
        [BurstCompile]
        struct AccelerationJob : IJobForEach<Game.Velocity, Game.Acceleration>
        {
            public float deltaTime;

            public void Execute(ref Game.Velocity velocity,[ReadOnly] ref Game.Acceleration acceleration)
            {
                velocity.Value += acceleration.Directed * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new AccelerationJob() { deltaTime = Time.deltaTime };
            return job.Schedule(this, inputDeps);
        }
    }

    public class LimitedAccelerateVelocity : JobComponentSystem
    {
        [BurstCompile]
        struct LimitedAccelerationJob : IJobForEach<Game.Velocity, Game.Acceleration, Game.MaxSpeed>
        {
            public float deltaTime;
            public void Execute(ref Game.Velocity velocity, [ReadOnly] ref Game.Acceleration acceleration, [ReadOnly] ref Game.MaxSpeed maxSpeed)
            {
                float3 newVelocity = velocity.Value + acceleration.Directed * deltaTime;
                if(math.lengthsq( newVelocity) <= math.pow(maxSpeed.Value*deltaTime,2f))
                    velocity.Value += acceleration.Directed * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new LimitedAccelerationJob() { deltaTime = Time.deltaTime };
            return job.Schedule(this, inputDeps);
        }
    }

    public class VelocityTranslation : JobComponentSystem
    {
        [BurstCompile]
        struct MovementJob : IJobForEach<Translation, Game.Velocity>
        {
            public void Execute(ref Translation position, [ReadOnly] ref Game.Velocity velocity)
            {
                position.Value += velocity.Value;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MovementJob();
            return job.Schedule(this, inputDeps);
        }
    }

    [UpdateAfter(typeof(VelocityTranslation))]
    public class DampenVelocity : JobComponentSystem
    {
        [BurstCompile]
        struct DampenJob : IJobForEach<Game.Velocity, Game.DampenVelocity>
        {
            //public float timeDelta;

            public void Execute(ref Game.Velocity velocity, [ReadOnly] ref Game.DampenVelocity dampen)
            {
                velocity.Value *= dampen.Value;// *timeDelta;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new DampenJob();// { timeDelta = Time.deltaTime};
            return job.Schedule(this, inputDeps);
        }
    }
    //------------RenderingSystems-------------//
}
