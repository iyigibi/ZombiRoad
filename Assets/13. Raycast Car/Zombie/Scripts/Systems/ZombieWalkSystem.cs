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
            var brainRadius = brainScale * 1f + 0.5f;
            var brainPosition= SystemAPI.GetComponent<LocalTransform>(brainEntity).Position;
            var brainForward= SystemAPI.GetComponent<LocalTransform>(brainEntity).Forward();
            var brainRigth= SystemAPI.GetComponent<LocalTransform>(brainEntity).Right();
            new ZombieWalkJob
            {
                DeltaTime = deltaTime,
                BrainRadiusSq = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                brainPosition= brainPosition,
                brainForward= brainForward,
                brainRigth= brainRigth
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
        public float3 brainForward;
        public float3 brainRigth;


        [BurstCompile]
        private void Execute(ZombieWalkAspect zombie, [EntityIndexInQuery] int sortKey)
        {
            zombie.Walk(DeltaTime, brainPosition);

            if (zombie.IsInStoppingRange(brainPosition, BrainRadiusSq, brainForward, brainRigth))
            {
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
            }
            
        }
    }

}
