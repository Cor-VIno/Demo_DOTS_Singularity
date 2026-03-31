using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Vino.Fragment.Components;

namespace Vino.Fragment.Authorings
{
    public class FragmentAuthoring : MonoBehaviour
    {
        public float Mass = 1.0f;
        public float3 Velocity;
        public class Backer : Baker<FragmentAuthoring>
        {
            public override void Bake(FragmentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<FragmentTag>(entity);
                AddComponent(entity, new FragmentData { 
                    Mass = authoring.Mass,
                    Velocity = authoring.Velocity
                });
            }
        }
    }
}