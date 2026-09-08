using System;
using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class FinishTriggerView : MonoBehaviour
    {
        public event Action VehicleEntered;

        private bool _triggered;

        public void Arm()
        {
            _triggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered || other.GetComponentInParent<VehicleView>() == null)
                return;

            _triggered = true;
            VehicleEntered?.Invoke();
        }
    }
}
