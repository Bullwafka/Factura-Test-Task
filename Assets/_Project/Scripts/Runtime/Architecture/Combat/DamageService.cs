using System;
using System.Collections.Generic;
using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class DamageService : IDamageService
    {
        private readonly Dictionary<Collider, IDamageable> _damageables = new();

        public void Register(Collider collider, IDamageable damageable)
        {
            if (collider == null)
                throw new ArgumentNullException(nameof(collider));
            if (damageable == null)
                throw new ArgumentNullException(nameof(damageable));

            _damageables[collider] = damageable;
        }

        public void Unregister(Collider collider)
        {
            if (collider != null)
                _damageables.Remove(collider);
        }

        public bool TryApplyDamage(Collider collider, int damage)
        {
            if (collider == null || !_damageables.TryGetValue(collider, out var damageable))
                return false;

            damageable.TakeDamage(damage);
            return true;
        }
    }
}
