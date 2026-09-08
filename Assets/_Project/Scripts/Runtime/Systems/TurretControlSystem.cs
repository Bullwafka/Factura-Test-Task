using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class TurretControlSystem : ITickable
    {
        private readonly ITurretAimProvider _aimProvider;
        private readonly GameConfig _config;
        private readonly Transform _turretPivot;

        public TurretControlSystem(
            ITurretAimProvider aimProvider,
            GameConfig config,
            GameSceneContext scene)
        {
            _aimProvider = aimProvider;
            _config = config;
            _turretPivot = scene.Vehicle.TurretYawPivot;
        }

        public void Tick()
        {
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
