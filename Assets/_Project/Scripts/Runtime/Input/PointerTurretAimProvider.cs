using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class PointerTurretAimProvider : ITurretAimProvider
    {
        private const float MinimumForwardDistance = 0.01f;

        private readonly IPlayerInputService _input;
        private readonly GameConfig _config;
        private readonly Camera _camera;
        private readonly Transform _vehicle;

        public PointerTurretAimProvider(
            IPlayerInputService input,
            GameConfig config,
            GameSceneContext scene)
        {
            _input = input;
            _config = config;
            _camera = scene.GameCamera;
            _vehicle = scene.Vehicle.transform;
        }

        public bool TryGetTargetYaw(out float targetYaw)
        {
            targetYaw = default;
            if (!_input.IsInputFired)
            {
                return false;
            }

            var pointerRay = _camera.ScreenPointToRay(_input.PointerScreenPosition);
            var roadPlane = new Plane(Vector3.up, _vehicle.position);
            if (!roadPlane.Raycast(pointerRay, out var distance))
            {
                return false;
            }

            var aimPoint = pointerRay.GetPoint(distance);
            var worldDirection = aimPoint - _vehicle.position;
            worldDirection.y = 0f;
            if (worldDirection.sqrMagnitude < 0.001f)
            {
                return false;
            }

            var localDirection = _vehicle.InverseTransformDirection(worldDirection);
            localDirection.z = Mathf.Max(localDirection.z, MinimumForwardDistance);

            targetYaw = Mathf.Clamp(
                Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg,
                -_config.TurretMaxYawAngle,
                _config.TurretMaxYawAngle);
            return true;
        }
    }
}
