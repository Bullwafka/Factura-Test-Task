using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class TurretControlSystem : ITickable
    {
        private readonly ITurretAimProvider _aimProvider;
        private readonly GameConfig _config;
        private readonly VehicleEntity _vehicle;
        private readonly Transform _turretPivot;

        public TurretControlSystem(
            ITurretAimProvider aimProvider,
            GameConfig config,
            VehicleEntity vehicle)
        {
            _aimProvider = aimProvider;
            _config = config;
            _vehicle = vehicle;
            _turretPivot = vehicle.View.TurretYawPivot;
        }

        public void Tick()
        {
            if (_vehicle.Health.IsDead)
                return;

            if (!_aimProvider.TryGetTargetYaw(out var targetYaw))
            {
                return;
            }

            var targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
            _turretPivot.localRotation = Quaternion.RotateTowards(
                _turretPivot.localRotation,
                targetRotation,
                _config.TurretTurnSpeed * Time.deltaTime);
        }
    }
}
