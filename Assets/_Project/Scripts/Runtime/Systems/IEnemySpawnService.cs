using UnityEngine;

namespace Factura.Gameplay
{
    public interface IEnemySpawnService
    {
        EnemyEntity Spawn(Vector3 position);
        void Despawn(EnemyEntity enemy);
    }
}
