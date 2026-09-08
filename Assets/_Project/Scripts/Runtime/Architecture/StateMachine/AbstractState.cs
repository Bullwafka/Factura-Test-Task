
namespace Factura.Gameplay.StateMachine
{
    public abstract class AbstractState
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();
    }
}
