using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class FinishGateView : MonoBehaviour
    {
        [SerializeField] private FinishTriggerView _trigger;

        public FinishTriggerView Trigger => _trigger;
    }
}
