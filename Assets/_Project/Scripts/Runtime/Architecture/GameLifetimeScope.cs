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
        [SerializeField] private EnemyView[] _enemies;
        [SerializeField] private HudView _hud;
        [SerializeField] private Camera _gameCamera;

        [Header("Projectile assets")]
        [SerializeField] private ProjectileView _projectilePrefab;
        [SerializeField] private ImpactEffectView _projectileImpactPrefab;
        [SerializeField] private Transform _projectileRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterInstances(builder);
            RegisterSystems(builder);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PlayerInputService>().As<IPlayerInputService>();
            builder.RegisterEntryPoint<VehicleMovementSystem>();
            builder.RegisterEntryPoint<TurretControlSystem>();
            builder.RegisterEntryPoint<VehicleCameraFollowSystem>();
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
                _enemies,
                _hud,
                _gameCamera,
                _projectileRoot));
        }
    }
}
