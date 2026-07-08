using System.Collections.Generic;
using UnityEngine;

namespace Alchemia.Data
{
    [CreateAssetMenu(fileName = "OrderGenerationConfig", menuName = "Alchemia/Order Generation Config")]
    public class OrderGenerationConfig : ScriptableObject
    {
        [Header("Eligible Output")]
        [Tooltip("Chains an order can request. One is picked with equal probability.")]
        public List<string> possibleChainIds;

        [Header("Tier Range")]
        public int minTier = 1;
        public int maxTier = 3;

        [Header("Reward")]
        [Tooltip("Energy reward = requested tier * this value.")]
        public int rewardPerTier = 5;
    }
}