using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class SwipeTurretAimProvider : ITurretAimProvider
    {
        private readonly IPlayerInputService _input;
        private readonly GameConfig _config;
        private readonly Transform _turretPivot;

        private float _dragStartYaw;

        public SwipeTurretAimProvider(
            IPlayerInputService input,
            GameConfig config,
            GameSceneContext scene)
        {
            _input = input;
            _config = config;
            _turretPivot = scene.Vehicle.TurretYawPivot;
        }

        public bool TryGetTargetYaw(out float targetYaw)
        {
            if (!_input.IsInputFired)
            {
                _dragStartYaw = NormalizeAngle(_turretPivot.localEulerAngles.y);
                targetYaw = default;
                return false;
            }

            targetYaw = Mathf.Clamp(
                _dragStartYaw + _input.HorizontalDeltaNormalized.x * _config.TurretDragAngle,
                -_config.TurretMaxYawAngle,
                _config.TurretMaxYawAngle);
            return true;
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
