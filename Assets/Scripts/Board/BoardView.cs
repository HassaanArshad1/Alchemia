using System.Collections.Generic;
using UnityEngine;
using Alchemia.Data;

namespace Alchemia.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ItemViewPool itemViewPool;
        [SerializeField] private ItemRegistry itemRegistry;

        private readonly Dictionary<(int x, int y), ItemView> activeViews = new Dictionary<(int x, int y), ItemView>();

        private void OnEnable()
        {
            boardManager.OnItemPlaced += HandleItemPlaced;
            boardManager.OnItemRemoved += HandleItemRemoved;
            boardManager.OnItemMoved += HandleItemMoved;
        }

        private void OnDisable()
        {
            boardManager.OnItemPlaced -= HandleItemPlaced;
            boardManager.OnItemRemoved -= HandleItemRemoved;
            boardManager.OnItemMoved -= HandleItemMoved;
        }

        private void HandleItemPlaced(int x, int y, ItemData data)
        {
            MergeItem definition = itemRegistry.GetItem(data.chainId, data.tier);
            if (definition == null) return;

            ItemView view = itemViewPool.Spawn(definition, boardManager.GridToWorld(x, y));
            activeViews[(x, y)] = view;
        }

        private void HandleItemRemoved(int x, int y)
        {
            if (!activeViews.TryGetValue((x, y), out ItemView view)) return;

            itemViewPool.Despawn(view);
            activeViews.Remove((x, y));
        }

        private void HandleItemMoved(int fromX, int fromY, int toX, int toY)
        {
            bool hadFrom = activeViews.TryGetValue((fromX, fromY), out ItemView fromView);
            bool hadTo = activeViews.TryGetValue((toX, toY), out ItemView toView);

            if (hadFrom) activeViews.Remove((fromX, fromY));
            if (hadTo) activeViews.Remove((toX, toY));

            if (hadFrom)
            {
                fromView.MoveTo(boardManager.GridToWorld(toX, toY));
                activeViews[(toX, toY)] = fromView;
            }

            if (hadTo)
            {
                toView.MoveTo(boardManager.GridToWorld(fromX, fromY));
                activeViews[(fromX, fromY)] = toView;
            }
        }

        public ItemView GetViewAt(int x, int y)
        {
            activeViews.TryGetValue((x, y), out ItemView view);
            return view;
        }
        
        public void ResyncView(int x, int y)
        {
            if (activeViews.TryGetValue((x, y), out ItemView view))
            {
                view.MoveTo(boardManager.GridToWorld(x, y));
            }
        }
    }
}
