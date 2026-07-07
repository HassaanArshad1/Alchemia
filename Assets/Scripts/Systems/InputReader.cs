using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Alchemia.Systems
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> OnPointerDown;
        public event Action<Vector2> OnPointerUp;
        public event Action<Vector2> OnPointerMoved;

        private bool wasPressed;

        private void Update()
        {
            Pointer pointer = Pointer.current;
            if (pointer == null) return;

            Vector2 position = pointer.position.ReadValue();
            bool isPressed = pointer.press.isPressed;

            if (isPressed && !wasPressed)
                OnPointerDown?.Invoke(position);
            else if (!isPressed && wasPressed)
                OnPointerUp?.Invoke(position);
            else if (isPressed)
                OnPointerMoved?.Invoke(position);

            wasPressed = isPressed;
        }
    }
}
