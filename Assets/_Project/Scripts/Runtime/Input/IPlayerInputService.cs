using UnityEngine;

namespace Factura.Gameplay
{
    public interface IPlayerInputService
    {
        bool IsInputFired { get; }
        Vector2 PointerScreenPosition { get; }
        Vector2 HorizontalDeltaNormalized { get; }
    }
}
