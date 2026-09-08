using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class VehicleCameraFollowSystem : ILateTickable
    {
        private readonly Transform _vehicle;
        private readonly Transform _camera;
        private readonly float _forwardOffset;

        public VehicleCameraFollowSystem(GameSceneContext scene)
        {
            _vehicle = scene.Vehicle.transform;
            _camera = scene.GameCamera.transform;
            _forwardOffset = _camera.position.z - _vehicle.position.z;
        }

        public void LateTick()
        {
            var position = _camera.position;
            position.z = _vehicle.position.z + _forwardOffset;
            _camera.position = position;
        }
    }
}
