using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class ProjectileView : MonoBehaviour, IPoolable
    {
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private ParticleSystem _flightParticles;

        public Transform VisualRoot => _visualRoot;
        public TrailRenderer Trail => _trail;
        public ParticleSystem FlightParticles => _flightParticles;

        public void ChangePooledState(bool isPooled)
        {
            gameObject.SetActive(!isPooled);

            if (isPooled)
                FlightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            else
                FlightParticles.Play(true);
        }
    }
}
