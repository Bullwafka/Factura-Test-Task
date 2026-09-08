using System;
using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class GameSceneContext
    {
        public GameSceneContext(
            VehicleView vehicle,
            EnemyView enemyPrefab,
            LevelChunkView levelChunkPrefab,
            FinishGateView finishGate,
            Transform levelRoot,
            HudView hud,
            Camera gameCamera,
            ImpactEffectView enemyDeathEffectPrefab,
            Transform projectileRoot)
        {
            Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
            EnemyPrefab = enemyPrefab ?? throw new ArgumentNullException(nameof(enemyPrefab));
            LevelChunkPrefab = levelChunkPrefab ?? throw new ArgumentNullException(nameof(levelChunkPrefab));
            FinishGate = finishGate ?? throw new ArgumentNullException(nameof(finishGate));
            LevelRoot = levelRoot ?? throw new ArgumentNullException(nameof(levelRoot));
            Hud = hud ?? throw new ArgumentNullException(nameof(hud));
            GameCamera = gameCamera ?? throw new ArgumentNullException(nameof(gameCamera));
            EnemyDeathEffectPrefab = enemyDeathEffectPrefab ?? throw new ArgumentNullException(nameof(enemyDeathEffectPrefab));
            ProjectileRoot = projectileRoot ?? throw new ArgumentNullException(nameof(projectileRoot));
        }

        public VehicleView Vehicle { get; }
        public EnemyView EnemyPrefab { get; }
        public LevelChunkView LevelChunkPrefab { get; }
        public FinishGateView FinishGate { get; }
        public Transform LevelRoot { get; }
        public HudView Hud { get; }
        public Camera GameCamera { get; }
        public ImpactEffectView EnemyDeathEffectPrefab { get; }
        public Transform ProjectileRoot { get; }
    }
}
