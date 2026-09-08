using System;

namespace Factura.Gameplay
{
    public sealed class VehicleEntity : IDisposable
    {
        private readonly IDamageService _damageService;

        public VehicleEntity(
            GameConfig config,
            GameSceneContext scene,
            IDamageService damageService)
        {
            View = scene.Vehicle;
            Health = new Health(config.VehicleHealth);
            _damageService = damageService;
            _damageService.Register(View.HitCollider, Health);
        }

        public VehicleView View { get; }
        public Health Health { get; }

        public void Dispose()
        {
            _damageService.Unregister(View.HitCollider);
        }
    }
}
