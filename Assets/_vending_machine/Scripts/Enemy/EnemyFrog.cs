using UnityEngine;

public class EnemyFrog : AbstractEnemy
{
    protected override void UpdateMaterial()
    {
        Material[] _mat = m_Renderer.materials;
        _mat[0] = GetMaterialByLife();
        m_Renderer.materials = _mat;
    }

    protected override bool HasDeath() => true;
}
