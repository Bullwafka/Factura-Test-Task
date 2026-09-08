using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class VehicleMovementSystem : IFixedTickable
    {
        private readonly GameConfig _config;
        private readonly VehicleEntity _vehicle;
        private readonly Rigidbody _body;
        private readonly float _roadCenterX;

        private float _lateralTargetX;
        private float _currentLateralSpeed;
        private float _maneuverTimeRemaining;
        private float _pauseTimeRemaining;

        public VehicleMovementSystem(
            GameConfig config,
            VehicleEntity vehicle)
        {
            _config = config;
            _vehicle = vehicle;
            _body = vehicle.View.Body;
            _roadCenterX = _body.position.x;
            _lateralTargetX = _roadCenterX;
            SchedulePause();
        }

        public void FixedTick()
        {
            if (_vehicle.Health.IsDead)
                return;

            var deltaTime = Time.fixedDeltaTime;
            UpdateLateralManeuver(deltaTime);

            var position = _body.position;
            position.x = Mathf.MoveTowards(
                position.x,
                _lateralTargetX,
                _currentLateralSpeed * deltaTime);
            position.x = Mathf.Clamp(
                position.x,
                _roadCenterX - _config.VehicleLateralLimit,
                _roadCenterX + _config.VehicleLateralLimit);
            position.z += _config.VehicleSpeed * deltaTime;

            _body.MovePosition(position);
        }

        private void UpdateLateralManeuver(float deltaTime)
        {
            if (_maneuverTimeRemaining > 0f)
            {
                _maneuverTimeRemaining -= deltaTime;
                if (_maneuverTimeRemaining <= 0f || Mathf.Approximately(_body.position.x, _lateralTargetX))
                {
                    _lateralTargetX = _body.position.x;
                    _currentLateralSpeed = 0f;
                    SchedulePause();
                }

                return;
            }

            _pauseTimeRemaining -= deltaTime;
            if (_pauseTimeRemaining <= 0f)
            {
                BeginLateralManeuver();
            }
        }

        private void BeginLateralManeuver()
        {
            var duration = RandomFromRange(_config.VehicleLateralDurationRange);
            _currentLateralSpeed = _config.VehicleLateralSpeed * Random.Range(0.7f, 1f);

            var currentOffset = _body.position.x - _roadCenterX;
            var direction = Random.value < 0.5f ? -1f : 1f;
            if (currentOffset >= _config.VehicleLateralLimit - 0.05f)
            {
                direction = -1f;
            }
            else if (currentOffset <= -_config.VehicleLateralLimit + 0.05f)
            {
                direction = 1f;
            }

            var potentialOffset = currentOffset + direction * _currentLateralSpeed * duration;
            var targetOffset = Mathf.Clamp(
                potentialOffset,
                -_config.VehicleLateralLimit,
                _config.VehicleLateralLimit);

            _lateralTargetX = _roadCenterX + targetOffset;
            _maneuverTimeRemaining = duration;
        }

        private void SchedulePause()
        {
            _pauseTimeRemaining = RandomFromRange(_config.VehicleLateralPauseRange);
        }

        private static float RandomFromRange(Vector2 range)
        {
            return Random.Range(Mathf.Min(range.x, range.y), Mathf.Max(range.x, range.y));
        }
    }
}
