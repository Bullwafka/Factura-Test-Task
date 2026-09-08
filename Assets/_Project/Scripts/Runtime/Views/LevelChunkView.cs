using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class LevelChunkView : MonoBehaviour, IPoolable
    {
        [SerializeField, Min(0.01f)] private float _length = 74.8f;
        [SerializeField] private float _finishOffset = 35.4f;

        public float Length => _length;
        public float FinishOffset => _finishOffset;

        public void ChangePooledState(bool isPooled)
        {
            gameObject.SetActive(!isPooled);
        }
    }
}
