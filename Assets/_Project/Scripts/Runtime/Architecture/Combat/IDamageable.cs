using System;

namespace Factura.Gameplay
{
    public interface IDamageable
    {
        event Action<int, int> HealthChanged;
        event Action Died;

        int CurrentHealth { get; }
        int MaxHealth { get; }
        bool IsDead { get; }

        void TakeDamage(int damage);
    }
}
