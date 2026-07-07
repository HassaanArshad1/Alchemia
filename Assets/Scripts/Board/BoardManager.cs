using System;
using Alchemia.Data;
using UnityEngine;

namespace Alchemia.Board
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Grid Dimensions")]
        [SerializeField] private int width = 7;
        [SerializeField] private int height = 9;

        [Header("World Layout")]
        [Tooltip("World-space position of the bottom-left cell's center.")]
        [SerializeField] private Vector3 origin = Vector3.zero;

        [Tooltip("Distance in world units between adjacent cell centers.")]
        [SerializeField] private float cellSize = 1f;

        [Header("Data")]
        [SerializeField] private ItemRegistry itemRegistry;
        
        public int Width => width;
        public int Height => height;

        private Cell[,] cells;

        public event Action<int, int, ItemData> OnItemPlaced;

        public event Action<int, int> OnItemRemoved;

        public event Action<int, int, int, int> OnItemMoved;

        public event Action<int, int, ItemData> OnItemsMerged;

        private void Awake()
        {
            BuildGrid();
        }

        private void BuildGrid()
        {
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
        }
        
        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public Vector3 GridToWorld(int x, int y)
        {
            return origin + new Vector3(x * cellSize, y * cellSize, 0f);
        }

        public bool WorldToGrid(Vector3 worldPos, out int x, out int y)
        {
            Vector3 local = worldPos - origin;
            x = Mathf.RoundToInt(local.x / cellSize);
            y = Mathf.RoundToInt(local.y / cellSize);
            return InBounds(x, y);
        }


        public Cell GetCell(int x, int y)
        {
            return InBounds(x, y) ? cells[x, y] : null;
        }

        public bool IsEmpty(int x, int y)
        {
            Cell c = GetCell(x, y);
            return c != null && c.IsEmpty;
        }

        public bool PlaceItem(int x, int y, ItemData item)
        {
            Cell c = GetCell(x, y);
            if (c == null) return false;

            c.SetItem(item);
            OnItemPlaced?.Invoke(x, y, item);
            return true;
        }

        public bool RemoveItem(int x, int y)
        {
            Cell c = GetCell(x, y);
            if (c == null) return false;

            c.Clear();
            OnItemRemoved?.Invoke(x, y);
            return true;
        }

        public bool MoveItem(int fromX, int fromY, int toX, int toY)
        {
            Cell from = GetCell(fromX, fromY);
            Cell to = GetCell(toX, toY);
            if (from == null || to == null || from.IsEmpty) return false;

            ItemData moving = from.Item;
            ItemData displaced = to.Item;

            to.SetItem(moving);
            if (displaced.IsValid)
                from.SetItem(displaced);
            else
                from.Clear(); 

            OnItemMoved?.Invoke(fromX, fromY, toX, toY);
            return true;
        }
        
        public bool TryMerge(int fromX, int fromY, int toX, int toY)
        {
            if (fromX == toX && fromY == toY) return false;
            if (!InBounds(fromX, fromY) || !InBounds(toX, toY)) return false;

            Cell fromCell = cells[fromX, fromY];
            Cell toCell   = cells[toX, toY];

            if (fromCell.IsEmpty || toCell.IsEmpty) return false;

            ItemData a = fromCell.Item;
            ItemData b = toCell.Item;

            if (a.chainId != b.chainId || a.tier != b.tier) return false;
            
            MergeItem current = itemRegistry.GetItem(a.chainId, a.tier);
            MergeItem next = itemRegistry.GetNextTier(current);
            if (next == null)
            {
                OnMaxTierReached(a);
                return false;
            }

            ItemData result = new ItemData(next.ChainId, next.Tier); 

            RemoveItem(fromX, fromY);
            RemoveItem(toX, toY);
            PlaceItem(toX, toY, result);

            OnItemsMerged?.Invoke(toX, toY, result);
            return true;
        }
        private void OnMaxTierReached(ItemData item)
        {
            // Intentionally empty. Reserved for order-sink logic.
        }

        public bool TryGetFirstEmptyCell(out int x, out int y)
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (cells[i, j].IsEmpty)
                    {
                        x = i;
                        y = j;
                        return true;
                    }
                }
            }
            x = -1;
            y = -1;
            return false;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 center = origin + new Vector3(x * cellSize, y * cellSize, 0f);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, 0.01f) * 0.95f);
                }
            }

            if (cells == null) return;
            Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.5f);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y] != null && !cells[x, y].IsEmpty)
                    {
                        Gizmos.DrawCube(GridToWorld(x, y),
                            new Vector3(cellSize, cellSize, 0.01f) * 0.8f);
                    }
                }
            }
        }

#if UNITY_EDITOR
        
        [ContextMenu("DEBUG: Spawn Two Mergeable Items")]
        private void DebugSpawnPair()
        {
            ItemData brew = new ItemData("potion", 1);   // match your chainId + tier-0
            ItemData simplePotion = new ItemData("potion", 2);
            ItemData enhancedPotion = new ItemData("potion", 3);
            ItemData elixor = new ItemData("potion", 5);
            PlaceItem(0, 0, brew);
            PlaceItem(1, 0, brew);
            PlaceItem(0, 1, simplePotion);
            PlaceItem(1, 1, enhancedPotion);
            PlaceItem(0, 2, elixor);
            PlaceItem(1, 2, elixor);
        }
#endif
    }
}
