using System;
using Factura.Gameplay.Enemies.States;
using Factura.Gameplay.StateMachine;
using UnityEngine;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class EnemyEntity : IDisposable
    {
        private readonly IDamageService _damageService;
        private readonly StateMachine<BaseEnemyState> _stateMachine;

        public event Action<EnemyEntity> Died;

        public EnemyEntity(
            EnemyView view,
            VehicleEntity vehicle,
            GameConfig config,
            IDamageService damageService)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _damageService = damageService ?? throw new ArgumentNullException(nameof(damageService));

            Health = new Health(config.EnemyHealth);
            Health.Died += OnDied;
            _damageService.Register(View.HitCollider, Health);

            _stateMachine = CreateStateMachine();
        }

        public EnemyView View { get; }
        public VehicleEntity Vehicle { get; }
        public GameConfig Config { get; }
        public Health Health { get; }
        public float DistanceToVehicle => Vector3.Distance(View.transform.position, Vehicle.View.transform.position);

        public void Start()
        {
            _stateMachine.Start();
        }

        public void FixedTick()
        {
            _stateMachine.Update();
        }

        public void Dispose()
        {
            Health.Died -= OnDied;
            _damageService.Unregister(View.HitCollider);
            _stateMachine.Stop();
        }

        private void OnDied()
        {
            _damageService.Unregister(View.HitCollider);
            _stateMachine.ChangeState<EnemyDeathState>();
            Died?.Invoke(this);
        }

        private StateMachine<BaseEnemyState> CreateStateMachine()
        {
            StateMachine<BaseEnemyState> stateMachine = new();

            stateMachine.Initialize(
                new EnemyIdleState(this, Config, stateMachine),
                new EnemyAggroState(this, Config, stateMachine),
                new EnemyAttackState(this, Config, stateMachine),
                new EnemyDeathState(this, Config, stateMachine));

            return stateMachine;
        }
    }
}
