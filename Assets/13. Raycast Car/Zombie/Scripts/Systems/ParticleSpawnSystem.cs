using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.Zombies
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ParticleSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraveyardProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();



            new SpawnParticleJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Run();

     
        }
    }
    
    [BurstCompile]
    public partial struct SpawnParticleJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer ECB;

        [BurstCompile]
        private void Execute(ParticleSystemAspect particleSystem)
        {
            particleSystem.SpawnTimer -= DeltaTime;
            particleSystem.Timer += DeltaTime;
            if (!particleSystem.TimeToSpawnParticle) return;
            if (!particleSystem.ParticleSpawnPointInitialized()) return;

            particleSystem.SpawnTimer = particleSystem.ParticleSpawnRate;
            var newParticle = ECB.Instantiate(particleSystem.ParticlePrefab);

            var trans = new Unity.Transforms.LocalTransform();
            trans.Position = particleSystem.Position;
            trans.Rotation = particleSystem.GetRandomRotation();
            trans.Scale = 1;
            ECB.SetComponent(newParticle, trans);

        }
    }
}
