using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace TMG.Zombies
{
    public readonly partial struct ZombieEatAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
       
        private readonly RefRW<ZombieTimer> _zombieTimer;
        private readonly RefRO<ZombieEatProperties> _eatProperties;
        private readonly RefRO<ZombieHeading> _heading;

        private readonly RefRW<PhysicsVelocity> _velocity;

        private float EatDamagePerSecond => _eatProperties.ValueRO.EatDamagePerSecond;
        private float EatAmplitude => _eatProperties.ValueRO.EatAmplitude;
        private float EatFrequency => _eatProperties.ValueRO.EatFrequency;
        private float Heading => _heading.ValueRO.Value;
        
        private float ZombieTimer
        {
            get => _zombieTimer.ValueRO.Value;
            set => _zombieTimer.ValueRW.Value = value;
        }
        
        public void Eat(float deltaTime, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity brainEntity)
        {
            ZombieTimer += deltaTime;
            //var eatAngle = EatAmplitude * math.sin(EatFrequency * ZombieTimer);
            //_transform.ValueRW.Rotation = quaternion.Euler(eatAngle, Heading, 0);

           // _velocity.ValueRW.Linear = 0;
           // _velocity.ValueRW.Angular = float3.zero;
          //  _transform.ValueRW.Rotation = quaternion.Euler(0, Heading, 0);

            var eatDamage = EatDamagePerSecond * deltaTime;
            var curBrainDamage = new BrainDamageBufferElement { Value = eatDamage };
            ecb.AppendToBuffer(sortKey, brainEntity, curBrainDamage);

            
        }
        
        public bool IsInEatingRange(float3 brainPosition, float brainRadiusSq)
        {
            return math.distancesq(brainPosition, _transform.ValueRO.Position) <= brainRadiusSq - 1;
        }
    }
}
