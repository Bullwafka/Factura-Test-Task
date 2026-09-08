using System.Collections.Generic;

namespace Factura.Gameplay
{
    public interface IEnemyTargetProvider
    {
        IReadOnlyList<EnemyEntity> ActiveEnemies { get; }
    }
}
