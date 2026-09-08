using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class EnemyDeathEffectSystem : IEnemyDeathEffectService, ITickable, IDisposable
    {
        private sealed class EffectState
        {
            public ImpactEffectView View;
            public float Lifetime;
        }

        private readonly GameConfig _config;
        private readonly ObjectPool<ImpactEffectView> _pool;
        private readonly List<EffectState> _activeEffects = new();

        public EnemyDeathEffectSystem(GameConfig config, GameSceneContext scene)
        {
            _config = config;
            _pool = new ObjectPool<ImpactEffectView>(scene.EnemyDeathEffectPrefab, 6);
        }

        public void Spawn(Vector3 position)
        {
            var effect = _pool.Get();
            effect.transform.SetPositionAndRotation(position, Quaternion.identity);

            _activeEffects.Add(new EffectState
            {
                View = effect,
                Lifetime = _config.EnemyDeathEffectLifetime
            });
        }

        public void Tick()
        {
            for (var index = _activeEffects.Count - 1; index >= 0; index--)
            {
                var state = _activeEffects[index];
                state.Lifetime -= Time.deltaTime;
                if (state.Lifetime > 0f)
                    continue;

                _pool.Add(state.View);
                _activeEffects.RemoveAt(index);
            }
        }

        public void Dispose()
        {
            foreach (var state in _activeEffects)
                Object.Destroy(state.View.gameObject);

            _activeEffects.Clear();
            _pool.Dispose();
        }
    }
}
