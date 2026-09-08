using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class EnemySystem : IEnemySpawnService, IEnemyTargetProvider, IFixedTickable, IDisposable
    {
        private readonly List<EnemyEntity> _enemies = new();
        private readonly IEnemyDeathEffectService _deathEffects;
        private readonly VehicleEntity _vehicle;
        private readonly GameConfig _config;
        private readonly IDamageService _damageService;
        private readonly ObjectPool<EnemyView> _pool;

        public EnemySystem(
            GameSceneContext scene,
            VehicleEntity vehicle,
            GameConfig config,
            IDamageService damageService,
            IEnemyDeathEffectService deathEffects)
        {
            _vehicle = vehicle;
            _config = config;
            _damageService = damageService;
            _deathEffects = deathEffects;
           
            _pool = new ObjectPool<EnemyView>(scene.EnemyPrefab, config.EnemyCountPerChunkRange.max * 2);
        }

        public IReadOnlyList<EnemyEntity> ActiveEnemies => _enemies;

        public EnemyEntity Spawn(Vector3 position)
        {
            var view = _pool.Get();
            view.Body.position = position;

            var enemy = new EnemyEntity(view, _vehicle, _config, _damageService);
            enemy.Died += OnEnemyDied;
            enemy.Start();
            _enemies.Add(enemy);
            return enemy;
        }

        public void Despawn(EnemyEntity enemy)
        {
            if (enemy == null || !_enemies.Remove(enemy))
                return;

            enemy.Died -= OnEnemyDied;
            enemy.Dispose();
            _pool.Add(enemy.View);
        }

        public void FixedTick()
        {
            for (var index = 0; index < _enemies.Count; index++)
                _enemies[index].FixedTick();
        }

        public void Dispose()
        {
            while (_enemies.Count > 0)
                Despawn(_enemies[_enemies.Count - 1]);

            _pool.Dispose();
        }

        private void OnEnemyDied(EnemyEntity enemy)
        {
            _deathEffects.Spawn(enemy.View.HitPoint.position);
        }
    }
}
