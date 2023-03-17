using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private List<Image> m_ListHearts = new List<Image>(GameDefinitions.MaxLife);
    
    [Space]
    [SerializeField] private Sprite m_Heart;
    [SerializeField] private Sprite m_Heartbreak;

    [SerializeField] private Color m_ColorHeart;
    [SerializeField] private Color m_ColorHeartbreak;

    [Space]
    [SerializeField] private TextMeshProUGUI m_TextShoot;

    [Space]
    [SerializeField] private Button m_ButtonReload;
    
    private readonly Subject<Unit> m_RxOnReload = new Subject<Unit>();
    public IObservable<Unit> RxOnReload => m_RxOnReload.AsObservable();

    private const string PrevTextShoot = "x ";

    private void Awake()
    {
        UpdateHeart();
        UpdateShoot();

        this.ObserveEveryValueChanged(_ => PlayData.Bullet)
            .Subscribe(_ => {
                UpdateShoot();
                m_ButtonReload.interactable = !PlayData.IsMaxBullet;
            }).AddTo(this);

        m_ButtonReload.OnClickAsObservable()
            .Subscribe(_ => {
                m_ButtonReload.interactable = false;
                m_RxOnReload.OnNext(Unit.Default);
            }).AddTo(this);
    }

    private void UpdateHeart()
    {
        for (int i = 0; i < GameDefinitions.MaxLife; i++)
        {
            bool _isLife = (i < PlayData.Life);
            m_ListHearts[i].sprite = _isLife ? m_Heart : m_Heartbreak;
            m_ListHearts[i].color = _isLife ? m_ColorHeart : m_ColorHeartbreak;
        }
    }

    private void UpdateShoot()
    {
        m_TextShoot.text = PrevTextShoot + PlayData.Bullet;
    }

    public void OnDamage()
    {
        PlayData.DecrementLife();
        if (PlayData.IsZeroLife)
        {
            GameEventManager.Notify(GameEvent.GameDead);
        }

        UpdateHeart();
    }

    public void OnLifeUp()
    {
        PlayData.IncrementLife();
        
        UpdateHeart();
    }
}
