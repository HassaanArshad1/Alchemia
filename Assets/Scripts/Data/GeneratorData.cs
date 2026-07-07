using System.Collections.Generic;
using UnityEngine;

namespace Alchemia.Data
{
    [CreateAssetMenu(fileName = "GeneratorData", menuName = "Alchemia/Generator Data")]
    public class GeneratorData : ScriptableObject
    {
        [Tooltip("Unique key for this generator. Used for cooldown tracking.")]
        public string generatorId;

        [Header("Output")]
        [Tooltip("Chains this generator can spawn from. One is picked with equal probability per tile.")]
        public List<string> possibleChainIds;

        [Tooltip("Minimum tiles spawned per activation (inclusive).")]
        public int minSpawnCount = 1;

        [Tooltip("Maximum tiles spawned per activation (inclusive).")]
        public int maxSpawnCount = 3;

        [Range(0f, 1f)]
        [Tooltip("Per-tile chance of spawning at tier 2 instead of tier 1.")]
        public float secondTierChance = 0.15f;

        [Header("Cost & Timing")]
        [Tooltip("Energy spent per activation \u2014 flat, regardless of how many tiles roll.")]
        public int energyCost = 1;

        [Tooltip("Seconds before this generator can fire again.")]
        public float cooldownSeconds = 2f;
    }
}