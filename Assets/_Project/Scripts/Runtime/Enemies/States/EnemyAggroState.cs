using Factura.Gameplay.StateMachine;
using UnityEngine;

namespace Factura.Gameplay.Enemies.States
{
    public sealed class EnemyAggroState : BaseEnemyState
    {
        public EnemyAggroState(EnemyEntity enemy, 
            GameConfig config, 
            StateMachine<BaseEnemyState> stateMachine) : base(enemy, config, stateMachine) 
        { 
        }

        public override void Start()
        {
            Enemy.View.Animator.SetFloat(SpeedParameter, 0.5f);
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
            var deltaTime = Time.fixedDeltaTime;
            FaceVehicle(deltaTime);

            var direction = Enemy.Vehicle.View.Body.position - Enemy.View.Body.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            var position = Enemy.View.Body.position +
                           direction.normalized * Enemy.Config.EnemyMovementSpeed * deltaTime;
            Enemy.View.Body.MovePosition(position);

            if(Enemy.DistanceToVehicle <= Config.EnemyAttackRadius)
                StateMachine.ChangeState<EnemyAttackState>();
        }
    }
}
