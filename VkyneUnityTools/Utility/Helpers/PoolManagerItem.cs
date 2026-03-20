using UnityEngine;

namespace VkyneTools.Utility.Helpers
{
    public class PoolManagerItem : MonoBehaviour
    {
        public PoolManager Pool;

        public enum Status { CHECKED_OUT, CHECKED_IN }
        public Status CurrentStatus;
    }
}
