using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class LevelGenerationSystem : IStartable, ITickable, IDisposable
    {
        private sealed class ChunkState
        {
            public LevelChunkView View;
            public readonly List<EnemyEntity> Enemies = new();
            public bool HasFinish;
        }

        private readonly GameConfig _config;
        private readonly VehicleEntity _vehicle;
        private readonly IEnemySpawnService _enemySpawner;
        private readonly LevelState _levelState;
        private readonly Transform _levelRoot;
        private readonly FinishGateView _finishGate;
        private readonly ObjectPool<LevelChunkView> _chunkPool;
        private readonly Queue<ChunkState> _activeChunks = new();
        private readonly float _chunkLength;
        private readonly float _finishOffset;
        private readonly int _levelChunkCount;
        private readonly int _totalChunkCount;

        private float _originZ;
        private float _startZ;
        private float _finishZ;
        private float _nextRecycleZ;
        private int _nextChunkIndex;

        public LevelGenerationSystem(
            GameConfig config,
            GameSceneContext scene,
            VehicleEntity vehicle,
            IEnemySpawnService enemySpawner,
            LevelState levelState)
        {
            _config = config;
            _vehicle = vehicle;
            _enemySpawner = enemySpawner;
            _levelState = levelState;
            _levelRoot = scene.LevelRoot;
            _finishGate = scene.FinishGate;
            _chunkLength = scene.LevelChunkPrefab.Length;
            _finishOffset = scene.LevelChunkPrefab.FinishOffset;
            _levelChunkCount = Mathf.Max(5, config.LevelChunkCount);
            _totalChunkCount = _levelChunkCount + Mathf.Max(1, config.PostFinishChunkCount);
            _chunkPool = new ObjectPool<LevelChunkView>(
                scene.LevelChunkPrefab,
                Mathf.Max(2, config.ActiveLevelChunkCount),
                _levelRoot);
        }

        public void Start()
        {
            _finishGate.gameObject.SetActive(false);

            _startZ = _vehicle.View.Body.position.z;
            _originZ = Mathf.Round(_startZ / _chunkLength) * _chunkLength;
            _finishZ = _originZ + (_levelChunkCount - 1) * _chunkLength + _finishOffset;
            _nextRecycleZ = _originZ + _chunkLength * 1.5f;

            var initialChunkCount = Mathf.Min(
                Mathf.Max(2, _config.ActiveLevelChunkCount),
                _totalChunkCount);

            for (var index = 0; index < initialChunkCount; index++)
                SpawnChunk(index);

            _nextChunkIndex = initialChunkCount;
            UpdateProgress(_startZ);
        }

        public void Tick()
        {
            float vehicleZ = _vehicle.View.Body.position.z;

            while (vehicleZ >= _nextRecycleZ && _activeChunks.Count > 0)
            {
                RecycleChunk(_activeChunks.Dequeue());

                if (_nextChunkIndex < _totalChunkCount)
                    SpawnChunk(_nextChunkIndex++);

                _nextRecycleZ += _chunkLength;
            }

            UpdateProgress(vehicleZ);
        }

        public void Dispose()
        {
            while (_activeChunks.Count > 0)
                RecycleChunk(_activeChunks.Dequeue());

            _finishGate.Trigger.VehicleEntered -= OnFinishEntered;
            _finishGate.gameObject.SetActive(false);
            _chunkPool.Dispose();
        }

        private void SpawnChunk(int chunkIndex)
        {
            var chunk = _chunkPool.Get();
            chunk.transform.position = new Vector3(0f, 0f, _originZ + chunkIndex * _chunkLength);

            var state = new ChunkState { View = chunk };

            if (chunkIndex < _levelChunkCount && chunkIndex > 0)
                SpawnEnemies(state, chunkIndex);

            if (chunkIndex == _levelChunkCount - 1)
                AttachFinish(state);

            _activeChunks.Enqueue(state);
        }

        private void SpawnEnemies(ChunkState chunkState, int chunkIndex)
        {
            var enemyCount = _config.EnemyCountPerChunkRange.random;

            if (enemyCount == 0)
                return;

            float halfLength = _chunkLength * 0.5f;
            float padding = _config.EnemySpawnPadding.random;
            Vector3 chunkPosition = chunkState.View.transform.position;
            float minZ = chunkPosition.z - halfLength + padding;
            float maxZ = chunkPosition.z + halfLength - padding;

            if (chunkIndex == 0)
                minZ = Mathf.Max(_vehicle.View.Body.position.z, minZ);

            for (var index = 0; index < enemyCount; index++)
            {
                float t = (index + 1f) / (enemyCount + 1f);
                float side = ((chunkIndex + index) & 1) == 0 ? -1f : 1f;
                float x = chunkPosition.x + side * _config.EnemyLaneOffset.random;
                float z = Mathf.Lerp(minZ, maxZ, t);

                Vector3 position = new Vector3(x, chunkPosition.y, z);

                EnemyEntity enemy = _enemySpawner.Spawn(
                    position);

                chunkState.Enemies.Add(enemy);
            }
        }

        private void AttachFinish(ChunkState chunkState)
        {
            var chunkTransform = chunkState.View.transform;
            var finishPosition = chunkTransform.position;
            finishPosition.z += _finishOffset;
            _finishGate.transform.SetPositionAndRotation(finishPosition, chunkTransform.rotation);
            _finishGate.Trigger.Arm();
            _finishGate.Trigger.VehicleEntered += OnFinishEntered;
            _finishGate.gameObject.SetActive(true);
            chunkState.HasFinish = true;

            _finishZ = finishPosition.z;
        }

        private void RecycleChunk(ChunkState chunkState)
        {
            foreach (var enemy in chunkState.Enemies)
                _enemySpawner.Despawn(enemy);

            chunkState.Enemies.Clear();

            if (chunkState.HasFinish)
            {
                _finishGate.Trigger.VehicleEntered -= OnFinishEntered;
                _finishGate.gameObject.SetActive(false);
            }

            _chunkPool.Add(chunkState.View);
        }

        private void UpdateProgress(float vehicleZ)
        {
            if (_levelState.IsCompleted)
                return;

            _levelState.SetProgress(Mathf.InverseLerp(_startZ, _finishZ, vehicleZ));
        }

        private void OnFinishEntered()
        {
            _levelState.Complete();
        }
    }
}
