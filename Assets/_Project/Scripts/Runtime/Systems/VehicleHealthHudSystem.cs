using System;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class VehicleHealthHudSystem : IStartable, IDisposable
    {
        private readonly Health _health;
        private readonly HudView _hud;

        public VehicleHealthHudSystem(VehicleEntity vehicle, GameSceneContext scene)
        {
            _health = vehicle.Health;
            _hud = scene.Hud;
        }

        public void Start()
        {
            _health.HealthChanged += Render;
            Render(_health.CurrentHealth, _health.MaxHealth);
        }

        public void Dispose()
        {
            _health.HealthChanged -= Render;
        }

        private void Render(int currentHealth, int maxHealth)
        {
            var healthNormalized = (float)currentHealth / maxHealth;
            var fillAnchors = _hud.VehicleHealthFill.rectTransform.anchorMax;
            fillAnchors.x = healthNormalized;
            _hud.VehicleHealthFill.rectTransform.anchorMax = fillAnchors;
            _hud.VehicleHealthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
