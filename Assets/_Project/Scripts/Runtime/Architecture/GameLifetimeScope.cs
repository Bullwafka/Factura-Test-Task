using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig _config;

        [Header("Serialized scene references")]
        [SerializeField] private VehicleView _vehicle;
        [SerializeField] private HudView _hud;
        [SerializeField] private Camera _gameCamera;
        [SerializeField] private FinishGateView _finishGate;
        [SerializeField] private Transform _levelRoot;

        [Header("Level assets")]
        [SerializeField] private LevelChunkView _levelChunkPrefab;
        [SerializeField] private EnemyView _enemyPrefab;

        [Header("Projectile assets")]
        [SerializeField] private ProjectileView _projectilePrefab;
        [SerializeField] private ImpactEffectView _projectileImpactPrefab;
        [SerializeField] private ImpactEffectView _enemyDeathEffectPrefab;
        [SerializeField] private Transform _projectileRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterInstances(builder);
            RegisterSystems(builder);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<DamageService>(Lifetime.Singleton).AsSelf().As<IDamageService>();
            builder.Register<VehicleEntity>(Lifetime.Singleton).AsSelf();
            builder.Register<LevelState>(Lifetime.Singleton).AsSelf();

            builder.RegisterEntryPoint<PlayerInputService>().As<IPlayerInputService>();
            builder.RegisterEntryPoint<GameFlowSystem>();
            builder.RegisterEntryPoint<VehicleMovementSystem>();
            builder.RegisterEntryPoint<TurretControlSystem>();
            builder.RegisterEntryPoint<VehicleCameraFollowSystem>();
            builder.RegisterEntryPoint<VehicleHealthHudSystem>();
            builder.RegisterEntryPoint<EnemyDeathEffectSystem>().As<IEnemyDeathEffectService>();
            builder.RegisterEntryPoint<EnemySystem>()
                .As<IEnemySpawnService>()
                .As<IEnemyTargetProvider>();
            builder.RegisterEntryPoint<LevelGenerationSystem>();
            builder.RegisterEntryPoint<LevelHudSystem>();
            builder.RegisterEntryPoint<ProjectileSystem>().As<IProjectileService>();
            builder.RegisterEntryPoint<TurretShootingSystem>();

            RegisterAimProvider(builder);
        }

        private void RegisterAimProvider(IContainerBuilder builder)
        {
            switch (_config.ControlType)
            {
                case ControlType.Swipe:
                    builder.Register<SwipeTurretAimProvider>(Lifetime.Singleton).As<ITurretAimProvider>();
                    break;
                case ControlType.Pointer:
                    builder.Register<PointerTurretAimProvider>(Lifetime.Singleton).As<ITurretAimProvider>();
                    break;
                default:
                    break;
            }
        }

        private void RegisterInstances(IContainerBuilder builder)
        {
            builder.RegisterInstance(_config);
            builder.RegisterInstance(_projectilePrefab);
            builder.RegisterInstance(_projectileImpactPrefab);

            builder.RegisterInstance(new GameSceneContext(
                _vehicle,
                _enemyPrefab,
                _levelChunkPrefab,
                _finishGate,
                _levelRoot,
                _hud,
                _gameCamera,
                _enemyDeathEffectPrefab,
                _projectileRoot));
        }
    }
}
