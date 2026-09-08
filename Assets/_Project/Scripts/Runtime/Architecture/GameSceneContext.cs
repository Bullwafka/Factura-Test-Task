using System;
using System.Collections.Generic;
using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class GameSceneContext
    {
        public GameSceneContext(
            VehicleView vehicle,
            IReadOnlyList<EnemyView> enemies,
            HudView hud,
            Camera gameCamera,
            Transform projectileRoot)
        {
            Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
            Enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));
            Hud = hud ?? throw new ArgumentNullException(nameof(hud));
            GameCamera = gameCamera ?? throw new ArgumentNullException(nameof(gameCamera));
            ProjectileRoot = projectileRoot ?? throw new ArgumentNullException(nameof(projectileRoot));
        }

        public VehicleView Vehicle { get; }
        public IReadOnlyList<EnemyView> Enemies { get; }
        public HudView Hud { get; }
        public Camera GameCamera { get; }
        public Transform ProjectileRoot { get; }
    }
}
