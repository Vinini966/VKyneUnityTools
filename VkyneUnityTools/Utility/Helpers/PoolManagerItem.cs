using System;
using UnityEngine;

namespace VkyneTools.Abstract.Utility.Helpers
{
    public abstract class PoolManagerItem : MonoBehaviour
    {
        public object Pool;

        public enum Status { CHECKED_OUT, CHECKED_IN }
        public Status CurrentStatus;
    }
}
