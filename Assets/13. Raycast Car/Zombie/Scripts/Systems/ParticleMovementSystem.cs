using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace TMG.Zombies
{
    [BurstCompile]
    public partial struct ParticleMovementSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            new ParticleMovementJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.Run();
        }

    }
    [BurstCompile]
    public partial struct ParticleMovementJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;


        [BurstCompile]
        private void Execute(ParticleMovementAspect particle, [EntityIndexInQuery] int sortKey)
        {
            particle.Move(DeltaTime);
            if (particle.IsInStoppingRange())
            {
                ECB.DestroyEntity(sortKey, particle.Entity);
            }

        }
    }
}