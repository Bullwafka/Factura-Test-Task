using UnityEngine;

namespace Factura.Gameplay
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Factura/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [field: SerializeField, Min(10f)] public float LevelLength { get; private set; } = 120f;
        [field: SerializeField, Min(1f)] public float VehicleSpeed { get; private set; } = 7f;
        [field: SerializeField, Min(0f)] public float VehicleLateralLimit { get; private set; } = 1.3f;
        [field: SerializeField, Min(0.01f)] public float VehicleLateralSpeed { get; private set; } = 0.45f;
        [field: SerializeField] public Vector2 VehicleLateralPauseRange { get; private set; } = new(2.5f, 5f);
        [field: SerializeField] public Vector2 VehicleLateralDurationRange { get; private set; } = new(1.5f, 3.5f);
        [field: SerializeField, Min(1f)] public float TurretTurnSpeed { get; private set; } = 360f;
        [field: SerializeField, Min(1f)] public float TurretDragAngle { get; private set; } = 160f;
        [field: SerializeField, Range(0f, 180f)] public float TurretMaxYawAngle { get; private set; } = 55f;
        [field: SerializeField, Min(0.1f)] public float ProjectileFireRate { get; private set; } = 5f;
        [field: SerializeField, Min(0.1f)] public float ProjectileSpeed { get; private set; } = 24f;
        [field: SerializeField, Min(0.1f)] public float ProjectileLifetime { get; private set; } = 3f;
        [field: SerializeField, Min(0.01f)] public float ProjectileRadius { get; private set; } = 0.09f;
        [field: SerializeField, Min(1f)] public float ProjectileAimDistance { get; private set; } = 20f;
        [field: SerializeField, Min(0f)] public float ProjectileFallbackTargetHeight { get; private set; } = 1.05f;
        [field: SerializeField, Min(0.1f)] public float ProjectileHeightSampleRadius { get; private set; } = 1f;
        [field: SerializeField, Min(0.1f)] public float ProjectileImpactLifetime { get; private set; } = 0.6f;
        [field: SerializeField] public LayerMask ProjectileHitMask { get; private set; } = ~0;
        [field: SerializeField, Min(1)] public int VehicleHealth { get; private set; } = 100;
        [field: SerializeField, Min(1)] public int EnemyHealth { get; private set; } = 30;
    }
}
