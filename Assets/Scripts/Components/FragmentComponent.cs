using Unity.Entities;
using Unity.Mathematics;

namespace Vino.Fragment.Components
{
    public struct FragmentTag : IComponentData { }
    public struct FragmentData : IComponentData
    {
        public float Mass;
        public float3 Velocity;
    }
}

