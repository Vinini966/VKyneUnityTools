//Created By Vincent Kyne
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VkyneTools.Utility
{
    [Serializable]
    public struct WeightedItem
    {
        public int Item;
        public float Weight;

        public WeightedItem(int item, int weight) : this()
        {
            Item = item;
            Weight = weight;
        }
    }

    [Serializable]
    public class WeightedRandom
    {

        public List<WeightedItem> WeightedList = new List<WeightedItem>();
        float _totalWeight = 0;

        /// <summary>
        /// Adds an Item of Weight to the weigted list
        /// </summary>
        /// <param name="item">item of type T</param>
        /// <param name="weight">weight of the item</param>
        public void AddItem(int item, int weight)
        {
            if (weight < 0)
            {
                Debug.LogWarning(item.ToString() + " is of a negative weight. Clamping at 0.");
                weight = Mathf.Clamp(weight, 0, Int32.MaxValue);
            }


            _totalWeight += weight;

            WeightedList.Add(new WeightedItem(item, weight));
        }

        /// <summary>
        /// Changes the weight of item
        /// Note: this works better for constant values.
        /// </summary>
        /// <param name="item">Item to look for</param>
        /// <param name="newWeight">new weight to change to</param>
        public void ChangeWeight(int item, float newWeight)
        {
            int index = WeightedList.FindIndex((x) => x.Item.Equals(item));
            var weightedItem = WeightedList[index];
            weightedItem.Weight = newWeight;
            WeightedList[index] = weightedItem;
            _totalWeight = WeightedList.Sum(x => x.Weight);
        }

        public void Clear()
        {
            WeightedList.Clear();
            _totalWeight = 0;
        }

        /// <summary>
        /// Gets the next random based on the weight compared to total weight
        /// </summary>
        /// <returns>item of the random chosen</returns>
        public int GetNextRandom(bool useSeeded = true)
        {
            float rand;
            rand = UnityEngine.Random.Range(1, _totalWeight);

            foreach (var weightedItem in WeightedList.Where(item => item.Weight > 0))
            {
                rand -= weightedItem.Weight;
                if (rand <= 0)
                    return weightedItem.Item;
            }
            return WeightedList.Last().Item;
        }
    }
}