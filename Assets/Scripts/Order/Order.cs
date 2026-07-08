using Alchemia.Board;

namespace Alchemia.Order
{
    public class Order
    {
        public readonly int OrderId;
        public readonly string ChainId;
        public readonly int Tier;
        public readonly int RewardEnergy;

        public Order(int orderId, string chainId, int tier, int rewardEnergy)
        {
            OrderId = orderId;
            ChainId = chainId;
            Tier = tier;
            RewardEnergy = rewardEnergy;
        }

        public bool Matches(ItemData item)
        {
            return item.IsValid && item.chainId == ChainId && item.tier == Tier;
        }
    }
}