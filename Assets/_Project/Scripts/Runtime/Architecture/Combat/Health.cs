using System;
using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class Health : IDamageable
    {
        public event Action<int, int> HealthChanged;
        public event Action Died;

        public Health(int maxHealth)
        {
            MaxHealth = Mathf.Max(1, maxHealth);
            CurrentHealth = MaxHealth;
        }

        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; }
        public bool IsDead => CurrentHealth <= 0;

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || IsDead)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (IsDead)
                Died?.Invoke();
        }
    }
}
