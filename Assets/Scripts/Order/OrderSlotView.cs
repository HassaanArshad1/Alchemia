using UnityEngine;
using UnityEngine.UI;

namespace Alchemia.Order
{
    [RequireComponent(typeof(Button))]
    public class OrderSlotView : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Image icon;
        [SerializeField] private Text requestText;
        [SerializeField] private Text rewardText;

        private Button _button;
        private OrderManager _orderManager;
        private int _orderId = -1;
        private bool _isBound;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnTapped);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnTapped);
        }

        public void Bind(OrderManager orderManager, Order order)
        {
            _orderManager = orderManager;
            _orderId = order.OrderId;
            _isBound = true;

            if (requestText != null) requestText.text = $"{order.ChainId} T{order.Tier}";
            if (rewardText != null) rewardText.text = $"+{order.RewardEnergy}";

            gameObject.SetActive(true);
        }

        public void Clear()
        {
            _isBound = false;
            _orderId = -1;
            gameObject.SetActive(false);
        }

        private void OnTapped()
        {
            if (!_isBound || _orderManager == null) return;
            _orderManager.TryFulfillOrder(_orderId);
        }
    }
}