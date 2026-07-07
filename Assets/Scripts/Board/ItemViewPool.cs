using System.Collections.Generic;
using UnityEngine;
using Alchemia.Data;

namespace Alchemia.Board
{

    public class ItemViewPool : MonoBehaviour
    {
        [SerializeField] private ItemView itemViewPrefab;
        [SerializeField] private int initialPoolSize = 20;

        private readonly Queue<ItemView> pool = new Queue<ItemView>();

        private void Awake()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                pool.Enqueue(CreateNew());
            }
        }

        private ItemView CreateNew()
        {
            ItemView view = Instantiate(itemViewPrefab, transform);
            view.gameObject.SetActive(false);
            return view;
        }
        
        public ItemView Spawn(MergeItem item, Vector3 worldPosition)
        {
            ItemView view = pool.Count > 0 ? pool.Dequeue() : CreateNew();
            view.Setup(item, worldPosition);
            return view;
        }

        public void Despawn(ItemView view)
        {
            view.ReturnToPool();
            pool.Enqueue(view);
        }
    }
}
