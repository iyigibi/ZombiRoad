using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.Zombies
{
    public readonly partial struct ParticleMovementAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<ParticleTimer> _movementTimer;
        private readonly RefRO<ParticleMovementProperties> _movementProperties;

        private float ParticleSpeed => _movementProperties.ValueRO.Speed;
        private bool Gravity => _movementProperties.ValueRO.Gravity;

        private float ParticleTimer
        {
            get => _movementTimer.ValueRO.Value;
            set => _movementTimer.ValueRW.Value = value;
        }

        public void Move(float deltaTime)
        {
            ParticleTimer += deltaTime;
            if (Gravity)
            {
                // _transform.ValueRW.Position.y += (1 - math.pow(ParticleTimer * 2 - 1, 2)) * deltaTime * ParticleSpeed;
                _transform.ValueRW.Position.y += (15 - 75*ParticleTimer) * deltaTime;
                _transform.ValueRW.Position.x += (_transform.ValueRO.Up() * deltaTime * ParticleSpeed).x;
                _transform.ValueRW.Position.z += (_transform.ValueRO.Up() * deltaTime * ParticleSpeed).z;
            }
            else
                _transform.ValueRW.Position += _transform.ValueRO.Up() * ParticleSpeed * deltaTime;
            _transform.ValueRW.Scale = (0.5f+ParticleTimer) *.06f;
        }


        public bool IsInStoppingRange()
        {
            return ParticleTimer>1.5;
        }
    }
}
