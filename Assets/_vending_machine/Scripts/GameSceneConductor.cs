using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameSceneConductor : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_VCFwd;
    [SerializeField] private CinemachineVirtualCamera m_VCRun;

    [Space]
    [SerializeField] private UIManager m_MgrUI;
    [SerializeField] private PlayerController m_CtrlPlayer;

    private void Awake()
    {
        m_VCFwd.enabled = true;
        m_VCRun.enabled = false;

        /*
        m_CtrlPlayer.RxOnDash
            .Subscribe(_ => m_VCDash.enabled = true)
            .AddTo(this);

        m_CtrlPlayer.RxDefault
            .Subscribe(_ => m_VCDash.enabled = false)
            .AddTo(this);

        m_CtrlPlayer.RxOnCountUp
            .Subscribe(_ => m_MgrUI.TimeCountUp())
            .AddTo(this);

        this.ObserveEveryValueChanged(_ => m_CtrlPlayer.PositionIntZ)
            .Subscribe(z => m_MgrUI.SetScoreValue(z))
            .AddTo(this);
            */
    }

    private async UniTask Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        await UniTask.WaitUntil(() => InputManager.GetTouch() == ENUM_TOUCH.TOUCH_BEGAN);
        
        GameEventManager.Notify(GameEvent.GameStart);
    }
}
