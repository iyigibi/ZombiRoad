using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace TMG.Zombies
{
    public readonly partial struct ZombieWalkAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<ZombieTimer> _walkTimer;
        private readonly RefRO<ZombieWalkProperties> _walkProperties;
        private readonly RefRW<ZombieHeading> _heading;
        private readonly RefRW<PhysicsVelocity> _velocity;

        private float WalkSpeed => _walkProperties.ValueRO.WalkSpeed;
        private float WalkAmplitude => _walkProperties.ValueRO.WalkAmplitude;
        private float WalkFrequency => _walkProperties.ValueRO.WalkFrequency;
        private float Heading => _heading.ValueRO.Value;

        private float WalkTimer
        {
            get => _walkTimer.ValueRO.Value;
            set => _walkTimer.ValueRW.Value = value;
        }

        public void Walk(float deltaTime,float3 brainPosition)
        {

            var zombieHeading = MathHelpers.GetHeading(_transform.ValueRO.Position, brainPosition);
            _heading.ValueRW.Value = zombieHeading;


            WalkTimer += deltaTime;
            //_transform.ValueRW.Position.y = 0;
            
            _velocity.ValueRW.Angular = float3.zero;
            _transform.ValueRW.Rotation = quaternion.Euler(0, Heading, 0);
            if(math.distance(float3.zero, _velocity.ValueRW.Linear) < 5)
            {
                _velocity.ValueRW.Linear += _transform.ValueRO.Forward() * deltaTime*5;
            }
            

            //var swayAngle = WalkAmplitude * math.sin(WalkFrequency * WalkTimer);

        }
        
        public bool IsInStoppingRange(float3 brainPosition, float brainRadiusSq,float3 bF,float3 bR)
        {
            /*
            var __transform = _transform.ValueRO;
            float zx = __transform.Position.x;
            float zz = __transform.Position.z;
            float bx = brainPosition.x;
            float bz = brainPosition.z;
            float w = 1;
            float h = 3;
            if (zx<bx+h*bF.x && zx>bx-h*bF.x
                &&
                zz < bz + h * bF.z && zz > bz - h * bF.z
                )
            {
                return true;
            }
            return false;
            */
            return math.distancesq(brainPosition, _transform.ValueRO.Position) <= brainRadiusSq;
        }
    }
}
