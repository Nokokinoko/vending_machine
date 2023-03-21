using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextScore;
    [SerializeField] private TextMeshProUGUI m_TextValue;
    [SerializeField] private Button m_ButtonRetry;

    private const float BeforeScale = 1.2f;
    private const float Delay = 0.5f;
    private const float DurationFade = 1.0f;
    
    private void Awake()
    {
        m_TextScore.alpha = 0.0f;
        m_TextValue.rectTransform.localScale = Vector3.one * BeforeScale;
        m_TextValue.alpha = 0.0f;
        m_ButtonRetry.gameObject.SetActive(false);

        m_ButtonRetry.OnClickAsObservable()
            .First()
            .Subscribe(_ => ReloadManager.Instance.Reload())
            .AddTo(this);
    }

    public async UniTask Enable()
    {
        m_TextValue.text = PlayData.Score.ToString();
        if (SaveData.HiScore < PlayData.Score)
        {
            SaveData.HiScore = PlayData.Score;
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(Delay));

        m_TextScore.alpha = 1.0f;
        
        await UniTask.Delay(TimeSpan.FromSeconds(Delay));

        DOTween.Sequence()
            .Append(m_TextValue.DOFade(1.0f, DurationFade).SetEase(Ease.Linear))
            .Join(m_TextValue.rectTransform.DOScale(Vector3.one, DurationFade).SetEase(Ease.Linear));

        await UniTask.Delay(TimeSpan.FromSeconds(DurationFade));

        //AdsManager.Instance.ShowInter();
        m_ButtonRetry.gameObject.SetActive(true);
    }
}
