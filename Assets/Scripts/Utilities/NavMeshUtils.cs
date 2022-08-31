using UnityEngine;
using UnityEngine.AI;

namespace Utilities
{
    public static class NavMeshUtils
    {
        public static bool TryGetCoast(Vector3 at, out float coast)
        {
            coast = -1f;
            
            NavMeshHit hit;
            NavMesh.SamplePosition(at, out hit, 0.1f, NavMesh.AllAreas);

            int index = MathUtils.IndexFromMask(hit.mask);

            if (index == -1)
                return false;
            
            coast = NavMesh.GetAreaCost(index);
            return true;
        }
    }
}