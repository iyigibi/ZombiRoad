using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.Zombies
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieRiseSystem))]
    public partial struct ZombieWalkSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrainTag>();
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
            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            var brainRadius = brainScale * 5f + 0.5f;
            var brainPosition= SystemAPI.GetComponent<LocalTransform>(brainEntity).Position;

            new ZombieWalkJob
            {
                DeltaTime = deltaTime,
                BrainRadiusSq = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                brainPosition= brainPosition,
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float BrainRadiusSq;
        public EntityCommandBuffer.ParallelWriter ECB;
        public float3 brainPosition;


        [BurstCompile]
        private void Execute(ZombieWalkAspect zombie, [EntityIndexInQuery] int sortKey)
        {
            zombie.Walk(DeltaTime, brainPosition);

            if (zombie.IsInStoppingRange(brainPosition, BrainRadiusSq))
            {
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
            }
            
        }
    }

}