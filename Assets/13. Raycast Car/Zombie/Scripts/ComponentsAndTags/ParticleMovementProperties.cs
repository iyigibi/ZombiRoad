using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace TMG.Zombies
{
    public struct ParticleMovementProperties : IComponentData, IEnableableComponent
    {
        public float Speed;
        public bool Gravity;
    }
    public struct ParticleTimer : IComponentData
    {
        public float Value;
    }
}