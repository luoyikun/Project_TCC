using UnityEngine;

namespace UTJ
{
    // 为弹簧骨骼提供作用力的基类。
    public class ForceProvider : MonoBehaviour
    {
        public virtual Vector3 GetForceOnBone(SpringBone springBone)
        {
            return Vector3.zero;
        }
    }
}
