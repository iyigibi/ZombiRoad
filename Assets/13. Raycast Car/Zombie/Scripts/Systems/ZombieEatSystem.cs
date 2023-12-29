using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.Zombies
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieWalkSystem))]
    public partial struct ZombieEatSystem : ISystem
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
            var brainPosition= SystemAPI.GetComponent<LocalTransform>(brainEntity).Position;
            var brainRadius = brainScale * 5f + 1f;
            
            new ZombieEatJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BrainEntity = brainEntity,
                BrainRadiusSq = brainRadius * brainRadius+10,
                brainPosition= brainPosition
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieEatJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity BrainEntity;
        public float BrainRadiusSq;
        public float3 brainPosition;
        
        [BurstCompile]
        private void Execute(ZombieEatAspect zombie, [EntityIndexInChunk]int sortKey)
        {
            if (zombie.IsInEatingRange(brainPosition, BrainRadiusSq))
            {
                zombie.Eat(DeltaTime, ECB, sortKey, BrainEntity);
                ECB.DestroyEntity(sortKey,zombie.Entity);
            }
            else
            {
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, false);
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
            }
        }
    }

}
