using System.Collections.Generic;
using UnityEngine;

namespace Alchemia.Order
{
    public class OrderDockController : MonoBehaviour
    {
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private List<OrderSlotView> slots; // sized to maxActiveOrders

        private readonly Dictionary<int, OrderSlotView> _boundSlots = new Dictionary<int, OrderSlotView>();

        private void OnEnable()
        {
            orderManager.OnOrderCreated += HandleOrderCreated;
            orderManager.OnOrderFulfilled += HandleOrderRemoved;
        }

        private void OnDisable()
        {
            orderManager.OnOrderCreated -= HandleOrderCreated;
            orderManager.OnOrderFulfilled -= HandleOrderRemoved;
        }

        private void HandleOrderCreated(Order order)
        {
            OrderSlotView freeSlot = FindFreeSlot();
            if (freeSlot == null)
            {
                Debug.LogWarning("OrderDockController: no free slot for new order.");
                return;
            }

            freeSlot.Bind(orderManager, order);
            _boundSlots[order.OrderId] = freeSlot;
        }

        private void HandleOrderRemoved(Order order)
        {
            if (_boundSlots.TryGetValue(order.OrderId, out OrderSlotView slot))
            {
                slot.Clear();
                _boundSlots.Remove(order.OrderId);
            }
        }

        private OrderSlotView FindFreeSlot()
        {
            foreach (var slot in slots)
            {
                bool taken = false;
                foreach (var bound in _boundSlots.Values)
                {
                    if (bound == slot) { taken = true; break; }
                }
                if (!taken) return slot;
            }
            return null;
        }
    }
}