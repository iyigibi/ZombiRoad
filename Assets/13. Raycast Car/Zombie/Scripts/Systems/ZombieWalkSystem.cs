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
            var brainPosition= SystemAPI.GetComponent<LocalTransform>(brainEntity).Position;
            var carRotation = SystemAPI.GetComponent<LocalTransform>(brainEntity).Rotation;
            new ZombieWalkJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                brainPosition= brainPosition,
                carRotation = carRotation
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieWalkJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public float3 brainPosition;
        public quaternion carRotation;


        [BurstCompile]
        private void Execute(ZombieWalkAspect zombie, [EntityIndexInQuery] int sortKey)
        {
            zombie.Walk(DeltaTime, brainPosition);

            if (zombie.IsInStoppingRange(brainPosition, carRotation))
            {
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
            }
            
        }
    }

}
