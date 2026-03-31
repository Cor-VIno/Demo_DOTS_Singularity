using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Vino.BlackHole.Components;
using Vino.Fragment.Components;
using UnityEngine.SocialPlatforms;
using Vino.Global.Gravity.Component;
using Unity.Transforms;
using Vino.Global.Gravity.Jobs;
using Unity.Jobs;

namespace Vino.Global.Gravity.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SingularityGravitySystem : ISystem
    {
        //private JobHandle myHandle;
        

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlackHoleData>();
            state.RequireForUpdate<GravityConfigData>();
            
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var blackHoleMass = SystemAPI.GetSingleton<BlackHoleData>().Mass;
            var blackHoleRadius = SystemAPI.GetSingleton<BlackHoleData>().Radius;
            var blackHoleRadiusSq = blackHoleRadius * blackHoleRadius;
            var blackHoleEntity = SystemAPI.GetSingletonEntity<BlackHoleData>();
            var blackHolePos = SystemAPI.GetComponent<LocalTransform>(blackHoleEntity).Position;

            var G = SystemAPI.GetSingleton<GravityConfigData>().G;
            var particleDrag = SystemAPI.GetSingleton<GravityConfigData>().ParticleDrag;
            var epsilonSq = SystemAPI.GetSingleton<GravityConfigData>().epsilon * SystemAPI.GetSingleton<GravityConfigData>().epsilon;
            var GM = G * blackHoleMass;
            float deltaTime = SystemAPI.Time.DeltaTime;

            float eventHorizonSq = SystemAPI.GetSingleton<BlackHoleData>().eventHorizon * SystemAPI.GetSingleton<BlackHoleData>().eventHorizon;//消失的距离的平方
            float spawnRadius = SystemAPI.GetSingleton<GravityConfigData>().SpawnRadius;//刷新的半径
            float initialSpeed = SystemAPI.GetSingleton<GravityConfigData>().InitialSpeed;//初始速度

            uint randomSeed = math.hash((double2)SystemAPI.Time.ElapsedTime) + 1;

            var calculateSingularityGravityJob = new CalculateSingularityGravityJob
            {
                blackHoleRadiusSq = blackHoleRadiusSq,
                blackHolePos = blackHolePos,
                GM = GM,
                particleDrag = particleDrag,
                epsilonSq = epsilonSq,
                deltaTme = deltaTime,
                eventHorizonSq = eventHorizonSq,
                spawnRadius = spawnRadius,
                randomSeed = randomSeed,
                initialSpeed = initialSpeed
            };

            calculateSingularityGravityJob.ScheduleParallel();

        }
    }
}