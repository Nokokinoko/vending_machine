public class EnemyWasp : AbstractEnemy
{
    protected override void UpdateMaterial()
    {
        
    }

    protected override bool HasIdle() => false;
    protected override bool HasDeath() => true;

    protected override void Fall()
    {
        
    }
}
