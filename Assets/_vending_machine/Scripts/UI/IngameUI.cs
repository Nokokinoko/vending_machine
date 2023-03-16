using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private List<Image> m_ListHearts = new List<Image>();
    
    [Space]
    [SerializeField] private Sprite m_Heart;
    [SerializeField] private Sprite m_Heartbreak;

    [SerializeField] private Color m_ColorHeart;
    [SerializeField] private Color m_ColorHeartbreak;

    [Space]
    [SerializeField] private TextMeshProUGUI m_TextShoot;
    [SerializeField] private ShootManager m_MgrShoot;

    [Space]
    [SerializeField] private Button m_ButtonReload;

    public void EnableButtonReload(bool enable) => m_ButtonReload.interactable = enable;

    private int m_MaxLife;
    private int m_Life;
    
    private readonly Subject<Unit> m_RxOnReload = new Subject<Unit>();
    public IObservable<Unit> RxOnReload => m_RxOnReload.AsObservable();

    private const string PrevTextShoot = "x ";

    private void Awake()
    {
        m_MaxLife = m_Life = m_ListHearts.Count;
        UpdateHeart();
        UpdateShoot();

        this.ObserveEveryValueChanged(_ => m_MgrShoot.NumShoot)
            .Subscribe(_ => UpdateShoot())
            .AddTo(this);

        m_ButtonReload.OnClickAsObservable()
            .Subscribe(_ => {
                EnableButtonReload(false);
                m_RxOnReload.OnNext(Unit.Default);
            }).AddTo(this);
    }

    private void UpdateHeart()
    {
        for (int i = 0; i < m_MaxLife; i++)
        {
            bool _isLife = (i < m_Life);
            m_ListHearts[i].sprite = _isLife ? m_Heart : m_Heartbreak;
            m_ListHearts[i].color = _isLife ? m_ColorHeart : m_ColorHeartbreak;
        }
    }

    private void UpdateShoot()
    {
        m_TextShoot.text = PrevTextShoot + m_MgrShoot.NumShoot;
    }

    public void OnDamage()
    {
        m_Life--;
        if (m_Life <= 0)
        {
            m_Life = 0;
            GameEventManager.Notify(GameEvent.GameDead);
        }

        UpdateHeart();
    }

    public void OnLifeUp()
    {
        m_Life++;
        m_Life = Mathf.Min(m_Life, m_MaxLife);
        
        UpdateHeart();
    }
}
