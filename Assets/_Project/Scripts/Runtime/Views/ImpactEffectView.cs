using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class ImpactEffectView : MonoBehaviour, IPoolable
    {
        [SerializeField] private ParticleSystem _particles;

        public ParticleSystem Particles => _particles;

        public void ChangePooledState(bool isPooled)
        {
            gameObject.SetActive(!isPooled);

            if (isPooled)
                Particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            else
                Particles.Play(true);
        }
    }
}
