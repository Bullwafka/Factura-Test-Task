using Factura.Gameplay.StateMachine;
using UnityEngine;

namespace Factura.Gameplay.Enemies.States
{
    public sealed class EnemyIdleState : BaseEnemyState
    {
        public EnemyIdleState(EnemyEntity enemy,
            GameConfig config,
            StateMachine<BaseEnemyState> stateMachine) : base(enemy, config, stateMachine)
        { }

        public override void Start()
        {
            Enemy.View.EnableAnimations();
            Enemy.View.Animator.SetFloat(SpeedParameter, 0f);
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
            if (Enemy.DistanceToVehicle <= Config.EnemyAggroRadius)
                StateMachine.ChangeState<EnemyAggroState>();
        }
    }
}
