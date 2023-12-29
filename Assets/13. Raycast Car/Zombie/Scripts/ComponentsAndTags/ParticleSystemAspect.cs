using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.Zombies
{
    public readonly partial struct ParticleSystemAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;

        private readonly RefRW<ParticleSystemSpawnTimer> _SpawnTimer;

        private readonly RefRO<ParticleSystemProperties> _particleSystemProperties;
        private readonly RefRW<ParticleRandom> _particleRandom;

        public bool TimeToSpawnParticle => SpawnTimer <= 0f;

        public float ParticleSpawnRate => _particleSystemProperties.ValueRO.ParticleSpawnRate;
        public float SystemLifeTime => _particleSystemProperties.ValueRO.SystemLifeTime;

        public Entity ParticlePrefab => _particleSystemProperties.ValueRO.ParticlePrefab;


        public float SpawnTimer
        {
            get => _SpawnTimer.ValueRO.Value;
            set => _SpawnTimer.ValueRW.Value = value;
        }
        public float Timer
        {
            get => _SpawnTimer.ValueRO.Timer;
            set => _SpawnTimer.ValueRW.Timer = value;
        }
        public float3 Position
        {
            get => _transform.ValueRO.Position;
            set => _transform.ValueRW.Position = value;
        }

        public quaternion GetRandomRotation()
        {

            return quaternion.Euler(_particleRandom.ValueRW.Value.NextFloat3(-0.30f,0.30f));

        }
        public bool ParticleSpawnPointInitialized()
        {
            return true;
        }
        public bool CheckForLifeTime()
        {
            return Timer! > SystemLifeTime;
        }
    }
}