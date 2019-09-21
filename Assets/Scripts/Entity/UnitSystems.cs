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

    //---------------MovementSystems---------------
    public class AccelerateToTarget : JobComponentSystem
    {
        [BurstCompile]
        struct TargetingJob : IJobForEach<Game.Acceleration, Game.Target, Translation>
        {
            public void Execute(ref Game.Acceleration acceleration, [ReadOnly] ref Game.Target target, [ReadOnly] ref Translation position)
            {
                acceleration.Directed = math.normalize(target.Position - position.Value) * acceleration.Value;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new TargetingJob();
            return job.Schedule(this, inputDeps);
        }
    }

    public class AccelerateVelocity : JobComponentSystem
    {
        [BurstCompile]
        struct AccelerationJob : IJobForEach<Game.Velocity, Game.Acceleration>
        {
            public float deltaTime;

            public void Execute(ref Game.Velocity velocity,[ReadOnly] ref Game.Acceleration acceleration)
            {
                velocity.Value += + acceleration.Directed * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new AccelerationJob() { deltaTime = Time.deltaTime };
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

}
