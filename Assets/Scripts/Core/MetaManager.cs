using UnityEngine;

namespace Alchemia.Systems
{
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