using UnityEngine;
using Factura.Gameplay.StateMachine;

namespace Factura.Gameplay.Enemies.States
{
    public abstract class BaseEnemyState : AbstractState
    {
        protected static readonly int SpeedParameter = Animator.StringToHash("Speed");
        protected static readonly int AttackParameter = Animator.StringToHash("Attack");
        protected static readonly int DeadParameter = Animator.StringToHash("Dead");

        protected readonly EnemyEntity Enemy;
        protected readonly GameConfig Config;
        protected readonly StateMachine<BaseEnemyState> StateMachine;

        protected BaseEnemyState(EnemyEntity enemy, GameConfig config, StateMachine<BaseEnemyState> stateMachine)
        {
            Enemy = enemy;
            Config = config;
            StateMachine = stateMachine;
        }

        protected void FaceVehicle(float deltaTime)
        {
            var direction = Enemy.Vehicle.View.Body.position - Enemy.View.Body.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            var rotation = Quaternion.RotateTowards(
                Enemy.View.Body.rotation,
                targetRotation,
                Enemy.Config.EnemyTurnSpeed * deltaTime);
            Enemy.View.Body.MoveRotation(rotation);
        }
    }
}
