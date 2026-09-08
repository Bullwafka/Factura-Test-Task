using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class EnemyView : MonoBehaviour
    {
        [SerializeField] private Transform _modelRoot;
        [SerializeField] private Transform _hitPoint;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private Collider _hitCollider;
        [SerializeField] private Animator _animator;

        public Transform ModelRoot => _modelRoot;
        public Transform HitPoint => _hitPoint;
        public Rigidbody Body => _body;
        public Collider HitCollider => _hitCollider;
        public Animator Animator => _animator;
    }
}
