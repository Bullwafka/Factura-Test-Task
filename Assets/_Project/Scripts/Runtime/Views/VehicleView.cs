using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class VehicleView : MonoBehaviour
    {
        [SerializeField] private Transform _modelRoot;
        [SerializeField] private Transform _turretYawPivot;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private Collider _hitCollider;

        public Transform ModelRoot => _modelRoot;
        public Transform TurretYawPivot => _turretYawPivot;
        public Transform Muzzle => _muzzle;
        public Rigidbody Body => _body;
        public Collider HitCollider => _hitCollider;
    }
}
