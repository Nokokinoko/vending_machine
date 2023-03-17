using System;
using UniRx;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_Tutorial;
    [SerializeField] private ScoreUI m_ScoreUI;
    [SerializeField] private IngameUI m_IngameUI;
    [SerializeField] private EndUI m_EndUI;

    public IObservable<Unit> RxOnReload => m_IngameUI.RxOnReload;

    private void Awake()
    {
        m_Tutorial.SetActive(true);
        m_ScoreUI.gameObject.SetActive(true);
        m_IngameUI.gameObject.SetActive(false);
        m_EndUI.gameObject.SetActive(false);

        GameEventManager
            .OnReceivedAsObservable(GameEvent.GameStart)
            .Subscribe(_ => Ingame())
            .AddTo(this);

        GameEventManager
            .OnReceivedAsObservable(GameEvent.GameDead)
            .Subscribe(_ => End())
            .AddTo(this);

        GameEventManager
            .OnReceivedAsObservable(GameEvent.GameTimeUp)
            .Subscribe(_ => End())
            .AddTo(this);
        
        //AdsManager.Instance.LoadInter();
        //AdsManager.Instance.ShowBanner();
    }

    private void Ingame()
    {
        //AdsManager.Instance.HideBanner();
        
        m_Tutorial.SetActive(false);
        m_ScoreUI.gameObject.SetActive(true);
        m_IngameUI.gameObject.SetActive(true);
        m_EndUI.gameObject.SetActive(false);
    }

    private void End()
    {
        m_Tutorial.SetActive(false);
        m_ScoreUI.gameObject.SetActive(false);
        m_IngameUI.gameObject.SetActive(false);
        m_EndUI.gameObject.SetActive(true);
        
        //m_EndUI.Enable(m_GameUI.GetScoreValue()).Forget();
    }
}
