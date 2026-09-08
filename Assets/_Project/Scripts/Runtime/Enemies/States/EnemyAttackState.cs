using Factura.Gameplay.StateMachine;
using UnityEngine;

namespace Factura.Gameplay.Enemies.States
{
    public sealed class EnemyAttackState : BaseEnemyState
    {
        private float _attackCooldown;

        public EnemyAttackState(EnemyEntity enemy,
            GameConfig config, 
            StateMachine<BaseEnemyState> stateMachine) : base(enemy, config, stateMachine)
        { }

        public override void Start()
        {
            Enemy.View.Animator.SetFloat(SpeedParameter, 0f);
            _attackCooldown = 0f;
        }

        public override void Stop()
        {
            Enemy.View.Animator.ResetTrigger(AttackParameter);
        }

        public override void Update()
        {
            if (Enemy.Vehicle.Health.IsDead)
            {
                StateMachine.ChangeState<EnemyIdleState>();
                return;
            }

            if (Enemy.DistanceToVehicle > Config.EnemyAttackRadius)
            {
                StateMachine.ChangeState<EnemyAggroState>();
                return;
            }

            var deltaTime = Time.fixedDeltaTime;
            FaceVehicle(deltaTime);

            _attackCooldown -= deltaTime;
            
            if (_attackCooldown > 0f)
                return;

            Enemy.View.Animator.SetTrigger(AttackParameter);
            Enemy.Vehicle.Health.TakeDamage(Enemy.Config.EnemyDamage);
            _attackCooldown = Enemy.Config.EnemyAttackInterval;
        }
    }
}
