using DG.Tweening;
using UnityEngine;

public class EnemyWasp : AbstractEnemy
{
    private const float DurationFall = 0.2f;
    
    protected override void UpdateMaterial()
    {
        Material[] _mat = m_Renderer.materials;
        _mat[2] = GetMaterialByLife();
        m_Renderer.materials = _mat;
    }

    protected override bool HasDeath() => true;

    protected override void Fall()
    {
        m_Xform.DOLocalMoveY(0.0f, DurationFall).SetEase(Ease.Linear);
    }
}
