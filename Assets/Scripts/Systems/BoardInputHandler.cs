using UnityEngine;
using Alchemia.Board;

namespace Alchemia.Systems
{
    public class BoardInputHandler : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private BoardView boardView;
        [SerializeField] private Camera boardCamera;

        private readonly Plane boardPlane = new Plane(Vector3.forward, Vector3.zero);

        private bool isDragging;
        private int dragFromX, dragFromY;
        private ItemView draggedView;

        private void OnEnable()
        {
            inputReader.OnPointerDown += HandlePointerDown;
            inputReader.OnPointerMoved += HandlePointerMoved;
            inputReader.OnPointerUp += HandlePointerUp;
        }

        private void OnDisable()
        {
            inputReader.OnPointerDown -= HandlePointerDown;
            inputReader.OnPointerMoved -= HandlePointerMoved;
            inputReader.OnPointerUp -= HandlePointerUp;
        }

        private bool TryScreenToGrid(Vector2 screenPos, out int x, out int y)
        {
            Ray ray = boardCamera.ScreenPointToRay(screenPos);
            if (boardPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                return boardManager.WorldToGrid(worldPoint, out x, out y);
            }

            x = -1;
            y = -1;
            return false;
        }

        private void HandlePointerDown(Vector2 screenPos)
        {
            if (!TryScreenToGrid(screenPos, out int x, out int y)) return;
            if (boardManager.IsEmpty(x, y)) return;

            draggedView = boardView.GetViewAt(x, y);
            if (draggedView == null) return;

            isDragging = true;
            dragFromX = x;
            dragFromY = y;
        }

        private void HandlePointerMoved(Vector2 screenPos)
        {
            if (!isDragging || draggedView == null) return;

            Ray ray = boardCamera.ScreenPointToRay(screenPos);
            if (boardPlane.Raycast(ray, out float distance))
            {
                draggedView.MoveTo(ray.GetPoint(distance));
            }
        }

        private void HandlePointerUp(Vector2 screenPos)
        {
            if (!isDragging) return;
            isDragging = false;

            if (TryScreenToGrid(screenPos, out int toX, out int toY))
                OnDrop(toX, toY);
            else
                boardView.ResyncView(dragFromX, dragFromY);

            draggedView = null;
        }
        
        private void OnDrop(int dropX, int dropY)
        {

            if (!boardManager.InBounds(dropX, dropY) || (dropX == dragFromX && dropY == dragFromY))
            {
                SnapBack(dragFromX, dragFromY);
                return;
            }

            if (boardManager.IsEmpty(dropX, dropY))
            {
                boardManager.MoveItem(dragFromX, dragFromY, dropX, dropY);
            }
            else
            {
                bool merged = boardManager.TryMerge(dragFromX, dragFromY, dropX, dropY);
                if (!merged) SnapBack(dragFromX, dragFromY);
            }
        }

        private void SnapBack(int x, int y)
        {
            boardView.ResyncView(x, y);
            OnMergeRejected();
        }

        private void OnMergeRejected()
        {
            // Reserved for negative haptics.
        }
    }
}
