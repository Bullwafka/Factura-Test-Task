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

        private readonly GameConfig _config;
        private readonly ProjectileView _projectilePrefab;
        private readonly ImpactEffectView _impactPrefab;

        private readonly List<ProjectileState> _activeProjectiles = new();
        private readonly List<ImpactState> _activeImpacts = new();
        private readonly ObjectPool<ImpactEffectView> _impactPool;
        private readonly ObjectPool<ProjectileView> _projectilePool;
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[16];

        public ProjectileSystem(GameConfig config, 
            ProjectileView projectilePrefab,
            ImpactEffectView impactPrefab)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

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
                    state.View.transform.position = hit.point;
                    SpawnImpact(hit.point, hit.normal);
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
            out RaycastHit closestHit)
        {
            var hitCount = Physics.SphereCastNonAlloc(
                origin,
                _config.ProjectileRadius,
                direction,
                _hitBuffer,
                distance,
                _config.ProjectileHitMask,
                QueryTriggerInteraction.Ignore);

            closestHit = default;
            var closestDistance = float.PositiveInfinity;
            for (var index = 0; index < hitCount; index++)
            {
                var hit = _hitBuffer[index];
                if (hit.collider == null || hit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hit.distance;
                closestHit = hit;
            }

            return closestDistance < float.PositiveInfinity;
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
