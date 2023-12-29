using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace TMG.Zombies
{
    public class ParticleMono : MonoBehaviour
    {
        public float Speed;
        public bool Gravity;

    }

    public class ParticleBaker : Baker<ParticleMono>
    {
        public override void Bake(ParticleMono authoring)
        {
            var particleEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<ParticleTimer>(particleEntity);
            AddComponent(particleEntity, new ParticleMovementProperties
            {
                Speed = authoring.Speed,
                Gravity = authoring.Gravity
            });
        }
    }
}