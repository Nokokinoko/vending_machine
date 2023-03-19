public class EnemySnake : AbstractEnemy
{
    protected override void UpdateMaterial()
    {
        
    }

    protected override bool HasIdle() => true;
    protected override bool HasDeath() => false;
}
