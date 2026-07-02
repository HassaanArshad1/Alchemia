using System;

namespace Alchemia.Board
{
    [Serializable]
    public struct ItemData
    {
        public string chainId;
        public int tier;

        public ItemData(string chainId, int tier)
        {
            this.chainId = chainId;
            this.tier = tier;
        }

        public bool IsValid => !string.IsNullOrEmpty(chainId);

        public static ItemData Empty => new ItemData(null, 0);
        
        public bool CanMergeWith(ItemData other)
        {
            return IsValid
                && other.IsValid
                && chainId == other.chainId
                && tier == other.tier;
        }
    }
    
    [Serializable]
    public class Cell
    {
        public readonly int x;
        public readonly int y;

        public ItemData Item { get; private set; }

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            Item = ItemData.Empty;
        }

        public bool IsEmpty => !Item.IsValid;

        public void SetItem(ItemData item)
        {
            Item = item;
        }

        public void Clear()
        {
            Item = ItemData.Empty;
        }
    }
}
