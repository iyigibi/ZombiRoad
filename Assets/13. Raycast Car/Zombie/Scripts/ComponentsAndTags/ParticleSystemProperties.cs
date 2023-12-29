using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public struct ParticleSystemProperties : IComponentData
{
    public Entity ParticlePrefab;
    public float ParticleSpawnRate;
    public float SystemLifeTime;
}
public struct ParticleSystemSpawnTimer : IComponentData
{
    public float Value;
    public float Timer;
}
public struct ParticleRandom : IComponentData
{
    public Random Value;

}