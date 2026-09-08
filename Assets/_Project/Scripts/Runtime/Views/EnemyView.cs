using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class EnemyView : MonoBehaviour, IPoolable
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

        public void ChangePooledState(bool isPooled)
        {
            _animator.enabled = false;

            _hitCollider.enabled = !isPooled;
            gameObject.SetActive(!isPooled);
        }

        public void EnableAnimations()
        {
            _animator.enabled = true;
            _animator.Rebind();
            _animator.Update(0f);
        }

        public void DisableAnimations()
        {
            _animator.enabled = false;
        }
    }
}
