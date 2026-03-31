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
            float deltaTime = SystemAPI.Time.DeltaTime;
            uint randomSeed = math.hash((double2)SystemAPI.Time.ElapsedTime) + 1;

            var blackHoleData = SystemAPI.GetSingleton<BlackHoleData>();
            var gravityConfig = SystemAPI.GetSingleton<GravityConfigData>();

            var blackHoleEntity = SystemAPI.GetSingletonEntity<BlackHoleData>();
            var blackHolePos = SystemAPI.GetComponent<LocalTransform>(blackHoleEntity).Position;

            float GM = gravityConfig.G * blackHoleData.Mass;
            float epsilonSq = gravityConfig.epsilon * gravityConfig.epsilon;
            float blackHoleRadiusSq = blackHoleData.Radius * blackHoleData.Radius;
            float eventHorizonSq = blackHoleData.eventHorizon * blackHoleData.eventHorizon; // 消失判定距离平方

            var calculateSingularityGravityJob = new CalculateSingularityGravityJob
            {
                GM = GM,
                blackHoleRadiusSq = blackHoleRadiusSq,
                blackHolePos = blackHolePos,
                epsilonSq = epsilonSq,
                deltaTme = deltaTime,
                particleDrag = gravityConfig.ParticleDrag,
                initialSpeed = gravityConfig.InitialSpeed,
                spawnRadius = gravityConfig.SpawnRadius,
                eventHorizonSq = eventHorizonSq,
                randomSeed = randomSeed,
                ringInnerRatio = blackHoleData.RingInnerRatio,
                ringOuterRatio = blackHoleData.RingOuterRatio,
                ringPercentage = blackHoleData.RingPercentage
            };

            calculateSingularityGravityJob.ScheduleParallel();
        }
    }
}