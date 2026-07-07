using System.Collections.Generic;
using UnityEngine;
using Alchemia.Board;
using Alchemia.Data;
using Alchemia.Systems;

namespace Alchemia.Generator
{
    public enum GenerateFailReason
    {
        
        OnCooldown,
        NoEnergy,
        BoardFull
    }

    public class GeneratorManager : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private MetaManager metaManager;
        
        private readonly Dictionary<string, float> _readyAt = new Dictionary<string, float>();

        public event System.Action<int, int, ItemData> OnItemGenerated;
        public event System.Action<GenerateFailReason> OnGenerateFailed;
        public event System.Action<int> OnGenerationComplete;

        public bool IsReady(string generatorId)
        {
            if (!_readyAt.TryGetValue(generatorId, out float readyAt))
                return true;
            return Time.time >= readyAt;
        }

        public float RemainingCooldown(string generatorId)
        {
            if (!_readyAt.TryGetValue(generatorId, out float readyAt))
                return 0f;
            return Mathf.Max(0f, readyAt - Time.time);
        }

        public float CooldownFraction(string generatorId, float cooldownSeconds)
        {
            if (cooldownSeconds <= 0f) return 0f;
            return Mathf.Clamp01(RemainingCooldown(generatorId) / cooldownSeconds);
        }

        public bool TryGenerate(GeneratorData gen)
        {
            // 1. Cooldown
            if (!IsReady(gen.generatorId))
            {
                OnGenerateFailed?.Invoke(GenerateFailReason.OnCooldown);
                return false;
            }

            // 2. Energy availability
            if (!metaManager.HasEnergy(gen.energyCost))
            {
                OnGenerateFailed?.Invoke(GenerateFailReason.NoEnergy);
                return false;
            }
            
            // 3. Check for Space
            int count = Random.Range(gen.minSpawnCount, gen.maxSpawnCount + 1);
            if (!boardManager.TryGetEmptyCells(count, out List<(int x, int y)> cellsToFill))
            {
                OnGenerateFailed?.Invoke(GenerateFailReason.BoardFull);
                return false;
            }

            // Generate
            metaManager.TrySpendEnergy(gen.energyCost);
            foreach (var (x, y) in cellsToFill)
            {
                string chainId = gen.possibleChainIds[Random.Range(0, gen.possibleChainIds.Count)];
                int tier = Random.value < gen.secondTierChance ? 2 : 1;

                ItemData item = new ItemData(chainId, tier);
                boardManager.PlaceItem(x, y, item);
                OnItemGenerated?.Invoke(x, y, item);
            }

            _readyAt[gen.generatorId] = Time.time + gen.cooldownSeconds;
            OnGenerationComplete?.Invoke(cellsToFill.Count);
            return true;
        }
        
        
#if UNITY_EDITOR
        [SerializeField] private GeneratorData debugGenerator;

        [ContextMenu("DEBUG: Try Generate")]
        private void DebugTryGenerate()
        {
            bool ok = TryGenerate(debugGenerator);
            Debug.Log($"[Generator] TryGenerate -> {ok} | energy left: {metaManager.Energy}");
        }
#endif
    }
}