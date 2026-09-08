using Factura.Gameplay.StateMachine;

namespace Factura.Gameplay.Enemies.States
{
    public sealed class EnemyDeathState : BaseEnemyState
    {
        public EnemyDeathState(EnemyEntity enemy, 
            GameConfig config, 
            StateMachine<BaseEnemyState> stateMachine) : base(enemy, config, stateMachine)
        { }

        public override void Start()
        {
            Enemy.View.Animator.SetFloat(SpeedParameter, 0f);
            Enemy.View.Animator.SetBool(DeadParameter, true);
            Enemy.View.HitCollider.enabled = false;
        }

        public override void Stop()
        {
            Enemy.View.DisableAnimations();
        }

        public override void Update()
        {
        }
    }
}
