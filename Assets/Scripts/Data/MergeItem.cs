using UnityEngine;

namespace Alchemia.Data
{
    [CreateAssetMenu(fileName = "MergeItem", menuName = "Alchemia/Merge Item")]
    public class MergeItem : ScriptableObject
    {
        [Tooltip("Which merge chain this belongs to, e.g. \"potion\". Must match Cell's ItemData.chainId exactly.")]
        [SerializeField] private string chainId;

        [Tooltip("Position within the chain, starting at 1.")]
        [SerializeField] private int tier = 1;

        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;

        public string ChainId => chainId;
        public int Tier => tier;
        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        
        public string Key => ItemRegistry.MakeKey(chainId, tier);
    }
}
