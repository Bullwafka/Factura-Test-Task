using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class ProjectileSystem : IProjectileService, ITickable, IDisposable
    {
        private sealed class ProjectileState
        {
            public ProjectileView View;
            public Vector3 Direction;
            public float Lifetime;
        }

        private sealed class ImpactState
        {
            public ImpactEffectView View;
            public float Lifetime;
        }

        private readonly struct ProjectileHit
        {
            public ProjectileHit(Collider collider, Vector3 point, Vector3 normal)
            {
                Collider = collider;
                Point = point;
                Normal = normal;
            }

            public Collider Collider { get; }
            public Vector3 Point { get; }
            public Vector3 Normal { get; }
        }

        private readonly GameConfig _config;
        private readonly IDamageService _damageService;
        private readonly ProjectileView _projectilePrefab;
        private readonly ImpactEffectView _impactPrefab;

        private readonly List<ProjectileState> _activeProjectiles = new();
        private readonly List<ImpactState> _activeImpacts = new();
        private readonly ObjectPool<ImpactEffectView> _impactPool;
        private readonly ObjectPool<ProjectileView> _projectilePool;
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[16];
        private readonly Collider[] _overlapBuffer = new Collider[16];

        public ProjectileSystem(GameConfig config,
            IDamageService damageService,
            ProjectileView projectilePrefab,
            ImpactEffectView impactPrefab)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _damageService = damageService ?? throw new ArgumentNullException(nameof(damageService));

            _projectilePrefab = projectilePrefab ?? throw new ArgumentNullException(nameof(projectilePrefab));
            _impactPrefab = impactPrefab ?? throw new ArgumentNullException(nameof(impactPrefab));

            _projectilePool = new(_projectilePrefab, 10);
            _impactPool = new(_impactPrefab, 10);
        }

        public void Fire(Vector3 position, Vector3 direction)
        {
            var projectile = _projectilePool.Get();
            projectile.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction, Vector3.up));
            projectile.Trail.Clear();

            _activeProjectiles.Add(new ProjectileState
            {
                View = projectile,
                Direction = direction.normalized,
                Lifetime = _config.ProjectileLifetime
            });
        }

        public void Tick()
        {
            var deltaTime = Time.deltaTime;
            UpdateProjectiles(deltaTime);
            UpdateImpacts(deltaTime);
        }

        private void UpdateProjectiles(float deltaTime)
        {
            for (var index = _activeProjectiles.Count - 1; index >= 0; index--)
            {
                var state = _activeProjectiles[index];
                var travelDistance = _config.ProjectileSpeed * deltaTime;

                if (TryGetHit(state.View.transform.position, state.Direction, travelDistance, out var hit))
                {
                    state.View.transform.position = hit.Point;
                    _damageService.TryApplyDamage(hit.Collider, _config.TurretDamage);
                    SpawnImpact(hit.Point, hit.Normal);
                    ReleaseProjectile(index, state.View);
                    continue;
                }

                state.View.transform.position += state.Direction * travelDistance;
                state.Lifetime -= deltaTime;
                if (state.Lifetime <= 0f)
                {
                    ReleaseProjectile(index, state.View);
                }
            }
        }

        private void UpdateImpacts(float deltaTime)
        {
            for (var index = _activeImpacts.Count - 1; index >= 0; index--)
            {
                var state = _activeImpacts[index];
                state.Lifetime -= deltaTime;
                if (state.Lifetime > 0f)
                {
                    continue;
                }

                _impactPool.Add(state.View);
                _activeImpacts.RemoveAt(index);
            }
        }

        private bool TryGetHit(
            Vector3 origin,
            Vector3 direction,
            float distance,
            out ProjectileHit closestHit)
        {
            var overlapCount = Physics.OverlapSphereNonAlloc(
                origin,
                _config.ProjectileRadius,
                _overlapBuffer,
                _config.ProjectileHitMask,
                QueryTriggerInteraction.Ignore);

            if (overlapCount > 0)
            {
                var collider = _overlapBuffer[0];
                var point = collider.ClosestPoint(origin);
                if ((point - origin).sqrMagnitude <= 0.0001f)
                    point = origin;

                closestHit = new ProjectileHit(collider, point, -direction);
                return true;
            }

            var hitCount = Physics.SphereCastNonAlloc(
                origin,
                _config.ProjectileRadius,
                direction,
                _hitBuffer,
                distance,
                _config.ProjectileHitMask,
                QueryTriggerInteraction.Ignore);

            var hitResult = default(RaycastHit);
            var closestDistance = float.PositiveInfinity;
            for (var index = 0; index < hitCount; index++)
            {
                var hit = _hitBuffer[index];
                if (hit.collider == null || hit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hit.distance;
                hitResult = hit;
            }

            if (closestDistance < float.PositiveInfinity)
            {
                closestHit = new ProjectileHit(hitResult.collider, hitResult.point, hitResult.normal);
                return true;
            }

            closestHit = default;
            return false;
        }

        private void ReleaseProjectile(int index, ProjectileView projectile)
        {
            _projectilePool.Add(projectile);
            _activeProjectiles.RemoveAt(index);
        }

        private void SpawnImpact(Vector3 position, Vector3 normal)
        {
            var impact = _impactPool.Get();

            impact.transform.SetPositionAndRotation(
                position,
                Quaternion.FromToRotation(Vector3.up, normal));
            
            _activeImpacts.Add(new ImpactState
            {
                View = impact,
                Lifetime = _config.ProjectileImpactLifetime
            });
        }

        public void Dispose()
        {
            foreach (var state in _activeProjectiles.ToArray())
                Object.Destroy(state.View.gameObject);

            foreach (var state in _activeImpacts.ToArray())
                Object.Destroy(state.View.gameObject);

            _projectilePool.Dispose();
            _impactPool.Dispose();
        }
    }
}
