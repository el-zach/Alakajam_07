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

    public class MainThread : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (manager == null) manager = World.Active.EntityManager;
            //Entities.
            //Entities.ForEach((Entity entity, ref Avoid.Point point) => // DO IT ON THE MAINTHREAD BECAUSE WHY NOT RIGHT?!
            //{
                
            //});
        }
    }

}
