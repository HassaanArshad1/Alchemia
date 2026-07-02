using System.Collections.Generic;
using UnityEngine;

namespace Alchemia.Data
{
    [CreateAssetMenu(fileName = "ItemRegistry", menuName = "Alchemia/Item Registry")]
    public class ItemRegistry : ScriptableObject
    {
        [SerializeField] private List<MergeItem> items = new List<MergeItem>();

        private Dictionary<string, MergeItem> lookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            lookup = new Dictionary<string, MergeItem>();
            foreach (MergeItem item in items)
            {
                if (item == null) continue;

                string key = item.Key;
                if (lookup.ContainsKey(key))
                {
                    Debug.LogWarning($"ItemRegistry: duplicate key '{key}' on '{item.name}' — check for a chainId/tier collision.");
                    continue;
                }
                lookup[key] = item;
            }
        }
        
        public static string MakeKey(string chainId, int tier) => $"{chainId}_{tier}";
        
        public MergeItem GetItem(string chainId, int tier)
        {
            if (lookup == null) BuildLookup();

            lookup.TryGetValue(MakeKey(chainId, tier), out MergeItem result);
            if (result == null)
                Debug.LogWarning($"ItemRegistry: no MergeItem found for '{MakeKey(chainId, tier)}'.");

            return result;
        }

        public MergeItem GetNextTier(MergeItem current)
        {
            return GetItem(current.ChainId, current.Tier + 1);
        }
    }
}
