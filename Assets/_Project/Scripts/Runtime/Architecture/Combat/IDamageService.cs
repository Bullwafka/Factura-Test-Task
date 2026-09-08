using UnityEngine;

namespace Factura.Gameplay
{
    public interface IDamageService
    {
        void Register(Collider collider, IDamageable damageable);
        void Unregister(Collider collider);
        bool TryApplyDamage(Collider collider, int damage);
    }
}
