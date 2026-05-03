using System;
using System.Collections.Generic;
using UnityEngine;
using VkyneTools.Abstract.Utility.Helpers;

namespace VkyneTools.Abstract.Utility
{
    public abstract class PoolManager<T> : MonoBehaviour where T : PoolManagerItem
    {
        public int MaxItems = 6;

        public GameObject Prefab;

        public Queue<T> Pool;

        public bool AllowOverflowDynamic;

        public bool DirtyPool; //You put a P in my ool!

        public Vector3 StorageLocation;

        int _totalItems = 0;

        // Start is called before the first frame update
        void Start()
        {
            Pool = new Queue<T>();

            //creates initial item
            CreateNewItem();
        }

        /// <summary>
        /// Checks out a object from the pool or creates one as needed given its under MaxItems
        /// </summary>
        /// <returns>Checked Out Object</returns>
        public GameObject CheckOutNextInPool()
        {

            if (_totalItems >= MaxItems && Pool.Count <= 0 && !AllowOverflowDynamic)
            {
                Debug.LogWarning($"<color=Red>Cannot create new item as limit has been reached.\nHave you checked back in unused objects?</color>");
                return null;
            }

            //if there are no items and the total items is still under the max allowed items
            if (Pool.Count <= 0)
            {
                CreateNewItem();
            }

            PoolManagerItem PoolManagerItem = Pool.Dequeue();
            PoolManagerItem.CurrentStatus = PoolManagerItem.Status.CHECKED_OUT;

            return PoolManagerItem.gameObject;
        }

        /// <summary>
        /// Returns Item back to the pool/queue
        /// </summary>
        /// <param name="item"></param>
        public void ReturnToPool(GameObject item, Action<GameObject> checkInAction = null)
        {
            if (!DirtyPool)
            {
                PoolManagerItem PoolManagerItem = item.GetComponent<PoolManagerItem>();

                if (PoolManagerItem != null && PoolManagerItem.Pool != (object)this)
                {
                    Debug.LogWarning($"Item {item.name} does not belong to this pool.");
                    return;
                }

                if (PoolManagerItem.CurrentStatus == PoolManagerItem.Status.CHECKED_IN)
                {
                    Debug.LogWarning($"Item {item.name} is already checked in.");
                    return;
                }

                checkInAction?.Invoke(item);

                if (_totalItems > MaxItems)
                {
                    Destroy(PoolManagerItem.gameObject);
                    _totalItems--;
                    return;
                }

                CheckInItem(PoolManagerItem);
            }
            else
            {
                Destroy(item);
                _totalItems--;
                CreateNewItem();
            }

        }

        /// <summary>
        /// Creates new item of prefab
        /// </summary>
        void CreateNewItem()
        {
            GameObject newObject = Instantiate(Prefab, transform);
            newObject.name = Prefab.name + _totalItems.ToString();

            PoolManagerItem item = newObject.AddComponent<T>();
            item.Pool = this;

            CheckInItem(item);

            _totalItems++;
        }


        void CheckInItem(PoolManagerItem PoolManagerItem)
        {

            PoolManagerItem.gameObject.transform.position = StorageLocation;
            Pool.Enqueue((T)PoolManagerItem);
            PoolManagerItem.CurrentStatus = PoolManagerItem.Status.CHECKED_IN;
        }
    }
}


