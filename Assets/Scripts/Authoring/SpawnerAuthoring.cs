using Unity.Entities;
using UnityEngine;
using Vino.Spawner.Component;
namespace Vino.Spawner.Authoring
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public int count;
        public class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                Entity fragmentEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
                AddComponent<SpawnerTag>(entity);
                AddComponent(entity, new SpawnerData
                {
                    prefab = fragmentEntity,
                    count = authoring.count
                });
            }
        }
    }
}