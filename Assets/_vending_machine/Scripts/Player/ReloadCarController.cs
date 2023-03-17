using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ReloadCarController : MonoBehaviour
{
    private Transform m_Xform;
    private Vector3 m_DefaultPosition;
    
    public PlayerController CtrlPlayer { private get; set; }
    
    private const float DefaultPositionX = 4.0f;
    private const float ArrivalPositionX = 0.4f;
    private const float DurationArrival = 0.8f;
    private const float DelayReload = 1.0f;

    private void Awake()
    {
        m_Xform = transform;
        m_DefaultPosition = m_Xform.localPosition;
    }

    public void InReload()
    {
        m_Xform.localPosition = m_DefaultPosition;
        m_Xform.DOLocalMoveX(ArrivalPositionX, DurationArrival)
            .SetEase(Ease.OutSine)
            .OnComplete(() => Reload().Forget());
    }

    private async UniTask Reload()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DelayReload));
        
        PlayData.IncrementBullet();
        if (PlayData.IsMaxBullet)
        {
            OutReload();
        }
        else
        {
            Reload().Forget();
        }
    }

    private void OutReload()
    {
        m_Xform.DOLocalMoveX(-DefaultPositionX, DurationArrival)
            .SetEase(Ease.InSine)
            .OnComplete(() => CtrlPlayer.RotateAndMove());
    }
}
