using UnityEngine;

namespace Alchemia.Systems
{
    // Stub — Step 8 needs only the energy gate for generation.
    // Later step fills in coins, energy regen, and persistence.
    public class MetaManager : MonoBehaviour
    {
        [SerializeField] private int energy = 50;

        public int Energy => energy;

        public bool HasEnergy(int amount) => energy >= amount;

        public bool TrySpendEnergy(int amount)
        {
            if (energy < amount) return false;
            energy -= amount;
            return true;
        }

        public void AddEnergy(int amount) => energy += amount;
    }
}