using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class PlayerInputService : IPlayerInputService, ITickable
    {
        private Vector2 _pressPosition;

        public bool IsInputFired { get; private set; }
        public Vector2 PointerScreenPosition { get; private set; }
        public Vector2 HorizontalDeltaNormalized { get; private set; }

        public void Tick()
        {
            if (TryReadTouch(out var position, out var isHeld))
            {
                UpdatePointer(position, isHeld);
                return;
            }

            if (Mouse.current != null)
            {
                UpdatePointer(
                    Mouse.current.position.ReadValue(),
                    Mouse.current.leftButton.isPressed);
                return;
            }

            ReleasePointer();
        }

        private static bool TryReadTouch(out Vector2 position, out bool isHeld)
        {
            var touchscreen = Touchscreen.current;
            if (touchscreen != null)
            {
                var touch = touchscreen.primaryTouch;
                if (touch.press.isPressed || touch.press.wasPressedThisFrame || touch.press.wasReleasedThisFrame)
                {
                    position = touch.position.ReadValue();
                    isHeld = touch.press.isPressed;
                    return true;
                }
            }

            position = default;
            isHeld = false;
            return false;
        }

        private void UpdatePointer(Vector2 position, bool isHeld)
        {
            PointerScreenPosition = position;

            if (isHeld && !IsInputFired)
            {
                _pressPosition = position;
            }

            IsInputFired = isHeld;
            if (!isHeld)
            {
                HorizontalDeltaNormalized = Vector2.zero;
                return;
            }

            HorizontalDeltaNormalized = new Vector2(
                (position.x - _pressPosition.x) / Mathf.Max(1f, Screen.width),
                (position.y - _pressPosition.y) / Mathf.Max(1f, Screen.height));
        }

        private void ReleasePointer()
        {
            IsInputFired = false;
            HorizontalDeltaNormalized = Vector2.zero;
        }
    }
}
