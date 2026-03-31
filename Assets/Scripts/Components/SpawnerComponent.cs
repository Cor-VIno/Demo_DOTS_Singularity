using Unity.Entities;

namespace Vino.Spawner.Component
{
    public struct SpawnerData : IComponentData
    {
        public Entity prefab;
        public int count;
    }
    public struct SpawnerTag : IComponentData { }
}