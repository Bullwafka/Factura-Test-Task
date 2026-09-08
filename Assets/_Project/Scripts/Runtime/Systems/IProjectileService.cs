using UnityEngine;

namespace Factura.Gameplay
{
    public interface IProjectileService
    {
        void Fire(Vector3 position, Vector3 direction);
    }
}
