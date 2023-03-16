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

        m_MgrUI.RxOnReload
            .Subscribe(_ => {
                m_VCRun.enabled = false;
                m_CtrlPlayer.StopAndRotate();
            }).AddTo(this);

        m_CtrlPlayer.RxOnRotateFwd
            .Subscribe(_ => m_VCRun.enabled = true)
            .AddTo(this);

        this.ObserveEveryValueChanged(_ => m_CtrlPlayer.IsMaxShoot)
            .Subscribe(_isMax => m_MgrUI.EnableButtonReload(!_isMax))
            .AddTo(this);
    }

    private async UniTask Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        await UniTask.WaitUntil(() => InputManager.GetTouch() == ENUM_TOUCH.TOUCH_BEGAN);

        GameEventManager.Notify(GameEvent.GameStart);
    }
}
