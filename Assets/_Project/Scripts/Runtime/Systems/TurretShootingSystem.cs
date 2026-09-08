using System;
using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class TurretShootingSystem : ITickable
    {
        private readonly IProjectileService _projectiles;
        private readonly IPlayerInputService _inputService;
        private readonly IEnemyTargetProvider _enemyTargets;
        private readonly GameConfig _config;
        private readonly VehicleEntity _vehicle;
        private readonly Transform _muzzle;
        private float _fireCooldown;

        public TurretShootingSystem(
            IProjectileService projectiles,
            IPlayerInputService inputService,
            IEnemyTargetProvider enemyTargets,
            GameConfig config,
            VehicleEntity vehicle)
        {
            _projectiles = projectiles ?? throw new ArgumentNullException(nameof(projectiles));
            _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            _enemyTargets = enemyTargets ?? throw new ArgumentNullException(nameof(enemyTargets));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
            _muzzle = vehicle.View.Muzzle;
        }

        public void Tick()
        {
            if (_vehicle.Health.IsDead)
                return;

            _fireCooldown -= Time.deltaTime;
            if (_fireCooldown > 0f || _config.ProjectileFireRate <= 0f)
            {
                return;
            }

            if (_inputService.IsInputFired)
            {
                Fire();
                _fireCooldown = 1f / _config.ProjectileFireRate;
            }
        }

        private void Fire()
        {
            var horizontalDirection = Vector3.ProjectOnPlane(_muzzle.forward, Vector3.up).normalized;

            if (horizontalDirection.sqrMagnitude <= 0.001f)
                return;

            var targetHeight = FindTargetHeight(horizontalDirection, out var aimDistance);
            var aimPoint = _muzzle.position + horizontalDirection * aimDistance;
            aimPoint.y = targetHeight;

            _projectiles.Fire(_muzzle.position, (aimPoint - _muzzle.position).normalized);
        }

        private float FindTargetHeight(Vector3 horizontalDirection, out float aimDistance)
        {
            var closestForwardDistance = float.PositiveInfinity;
            var targetHeight = _config.ProjectileFallbackTargetHeight;
            aimDistance = _config.ProjectileAimDistance;

            foreach (var enemy in _enemyTargets.ActiveEnemies)
            {
                if (enemy.Health.IsDead || !enemy.View.gameObject.activeInHierarchy)
                {
                    continue;
                }

                var toTarget = enemy.View.HitPoint.position - _muzzle.position;
                var planarOffset = Vector3.ProjectOnPlane(toTarget, Vector3.up);
                var forwardDistance = Vector3.Dot(planarOffset, horizontalDirection);
                if (forwardDistance <= 0f || forwardDistance >= closestForwardDistance)
                {
                    continue;
                }

                var lateralDistance = (planarOffset - horizontalDirection * forwardDistance).magnitude;
                if (lateralDistance > _config.ProjectileHeightSampleRadius)
                {
                    continue;
                }

                closestForwardDistance = forwardDistance;
                targetHeight = enemy.View.HitPoint.position.y;
                aimDistance = forwardDistance;
            }

            return targetHeight;
        }
    }
}
