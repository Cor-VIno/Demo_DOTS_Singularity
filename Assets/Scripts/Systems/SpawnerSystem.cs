using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Vino.Fragment.Components;
using Vino.Spawner.Component;

namespace Vino.Fragment.Systems
{
    [BurstCompile]
    public partial struct SpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spawner = SystemAPI.GetSingleton<SpawnerData>();
            var instances = state.EntityManager.Instantiate(spawner.prefab, spawner.count, Allocator.Temp);

            instances.Dispose();
            state.Enabled = false;
        }
    }
}