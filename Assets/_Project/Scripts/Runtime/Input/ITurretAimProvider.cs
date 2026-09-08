namespace Factura.Gameplay
{
    public interface ITurretAimProvider
    {
        bool TryGetTargetYaw(out float targetYaw);
    }
}
