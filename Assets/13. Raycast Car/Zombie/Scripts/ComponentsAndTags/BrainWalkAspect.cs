using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.Zombies
{
    public readonly partial struct BrainWalkAspect: IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<BrainTag> brainTag;



        public void Walk(float deltaTime)
        {
            _transform.ValueRW.Position += _transform.ValueRO.Forward() * deltaTime;
            
        }
        
    }
}