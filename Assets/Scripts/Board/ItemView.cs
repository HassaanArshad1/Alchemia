using UnityEngine;
using Alchemia.Data;

namespace Alchemia.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemView : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        public MergeItem CurrentItem { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Setup(MergeItem item, Vector3 worldPosition)
        {
            CurrentItem = item;
            spriteRenderer.sprite = item != null ? item.Sprite : null;
            transform.position = worldPosition;
            gameObject.SetActive(true);
        }

        public void MoveTo(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void ReturnToPool()
        {
            CurrentItem = null;
            gameObject.SetActive(false);
        }
    }
}
