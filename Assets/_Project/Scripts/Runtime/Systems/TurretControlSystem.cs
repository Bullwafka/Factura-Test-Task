using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class TurretControlSystem : ITickable
    {
        private readonly IPlayerInputService _input;
        private readonly GameConfig _config;
        private readonly Transform _turretPivot;

        private float _dragStartYaw;

        public TurretControlSystem(
            IPlayerInputService input,
            GameConfig config,
            GameSceneContext scene)
        {
            _input = input;
            _config = config;
            _turretPivot = scene.Vehicle.TurretYawPivot;
        }

        public void Tick()
        {
            if (!_input.IsInputFired)
            {
                _dragStartYaw = NormalizeAngle(_turretPivot.localEulerAngles.y);
                return;
            }

            var targetYaw = Mathf.Clamp(
                _dragStartYaw + _input.HorizontalDeltaNormalized.x * _config.TurretDragAngle,
                -_config.TurretMaxYawAngle,
                _config.TurretMaxYawAngle);
            var targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
            _turretPivot.localRotation = Quaternion.RotateTowards(
                _turretPivot.localRotation,
                targetRotation,
                _config.TurretTurnSpeed * Time.deltaTime);
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
