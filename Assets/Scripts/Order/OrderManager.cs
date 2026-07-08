using System.Collections.Generic;
using UnityEngine;
using Alchemia.Board;
using Alchemia.Data;
using Alchemia.Systems;

namespace Alchemia.Order
{
    public class OrderManager : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private MetaManager metaManager;
        [SerializeField] private OrderGenerationConfig config;
        [SerializeField] private int maxActiveOrders = 3;

        private readonly List<Order> _activeOrders = new List<Order>();
        private int _nextOrderId;

        public IReadOnlyList<Order> ActiveOrders => _activeOrders;

        public event System.Action<Order> OnOrderCreated;
        public event System.Action<Order> OnOrderFulfilled;
        public event System.Action<Order> OnFulfillFailed;

        private void Start()
        {
            FillQueue();
        }

        private void FillQueue()
        {
            while (_activeOrders.Count < maxActiveOrders)
            {
                Order order = GenerateOrder();
                _activeOrders.Add(order);
                OnOrderCreated?.Invoke(order);
            }
        }

        private Order GenerateOrder()
        {
            string chainId = config.possibleChainIds[Random.Range(0, config.possibleChainIds.Count)];
            int tier = Random.Range(config.minTier, config.maxTier + 1);
            int reward = tier * config.rewardPerTier;
            return new Order(_nextOrderId++, chainId, tier, reward);
        }

        public bool TryFulfillOrder(int orderId)
        {
            Order order = _activeOrders.Find(o => o.OrderId == orderId);
            if (order == null) return false;

            if (!boardManager.TryFindItemOfType(order.ChainId, order.Tier, out int x, out int y))
            {
                OnFulfillFailed?.Invoke(order);
                return false;
            }

            boardManager.RemoveItem(x, y);
            metaManager.AddEnergy(order.RewardEnergy);

            _activeOrders.Remove(order);
            OnOrderFulfilled?.Invoke(order);

            Order replacement = GenerateOrder();
            _activeOrders.Add(replacement);
            OnOrderCreated?.Invoke(replacement);

            return true;
        }
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            OnOrderCreated += order =>
                Debug.Log($"[Order] created #{order.OrderId}: {order.ChainId} T{order.Tier} -> +{order.RewardEnergy} energy");
            OnOrderFulfilled += order =>
                Debug.Log($"[Order] fulfilled #{order.OrderId} | energy now: {metaManager.Energy}");
            OnFulfillFailed += order =>
                Debug.Log($"[Order] fulfill FAILED #{order.OrderId}: no {order.ChainId} T{order.Tier} on board");
        }

        [ContextMenu("DEBUG: Log Active Orders")]
        private void DebugLogActiveOrders()
        {
            if (_activeOrders.Count == 0) { Debug.Log("[Order] no active orders"); return; }
            foreach (var o in _activeOrders)
                Debug.Log($"  #{o.OrderId}: {o.ChainId} T{o.Tier} -> +{o.RewardEnergy}");
        }

        [ContextMenu("DEBUG: Try Fulfill First Order")]
        private void DebugFulfillFirstOrder()
        {
            if (_activeOrders.Count == 0) { Debug.Log("[Order] no active orders"); return; }
            TryFulfillOrder(_activeOrders[0].OrderId);
        }

        [ContextMenu("DEBUG: Spawn Item Matching First Order")]
        private void DebugSpawnItemForFirstOrder()
        {
            if (_activeOrders.Count == 0) { Debug.Log("[Order] no active orders"); return; }
            Order order = _activeOrders[0];

            if (!boardManager.TryGetFirstEmptyCell(out int x, out int y))
            {
                Debug.Log("[Order] board full, can't spawn test item");
                return;
            }

            boardManager.PlaceItem(x, y, new ItemData(order.ChainId, order.Tier));
            Debug.Log($"[Order] spawned {order.ChainId} T{order.Tier} at ({x},{y}) for order #{order.OrderId}");
        }
#endif
        
    }
}