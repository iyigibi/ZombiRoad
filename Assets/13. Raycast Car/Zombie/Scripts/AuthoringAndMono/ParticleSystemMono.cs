using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace TMG.Zombies
{
    public class ParticleSystemMono : MonoBehaviour
    {
        public float SystemLifeTime;
        public GameObject particlePrefab;
        public float particleSpawnRate;
    }

    public class ParticleSystemBaker : Baker<ParticleSystemMono>
    {
        public override void Bake(ParticleSystemMono authoring)
        {
            var particleSystemEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(particleSystemEntity, new ParticleSystemProperties
            {
                     ParticlePrefab = GetEntity(authoring.particlePrefab, TransformUsageFlags.Dynamic),
                    ParticleSpawnRate = authoring.particleSpawnRate,
                    SystemLifeTime = authoring.SystemLifeTime
                    
            }
            );
            AddComponent<ParticleSystemSpawnTimer>(particleSystemEntity);
            AddComponent(particleSystemEntity, new ParticleRandom
            {
                Value = Unity.Mathematics.Random.CreateFromIndex(1)//Random seed
            });
        }
    }
}
