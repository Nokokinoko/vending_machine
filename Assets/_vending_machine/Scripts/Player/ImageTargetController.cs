using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageTargetController : MonoBehaviour
{
    [SerializeField] private CanvasScaler m_Scaler;
    [SerializeField] private TextMeshProUGUI m_TextReload;
    
    private Image m_ImageTarget;
    public bool CanShoot => m_ImageTarget && m_ImageTarget.enabled;
    public Vector2 PositionShoot => m_ImageTarget.rectTransform.position;

    private Vector2 m_TouchBegan = Vector2.zero;
    private bool IsTouchBeganZero => m_TouchBegan == Vector2.zero;

    private bool m_CanTarget = false;
    public bool CanTarget
    {
        set
        {
            m_CanTarget = value;
            if (!m_CanTarget)
            {
                m_TouchBegan = Vector2.zero;
                m_ImageTarget.enabled = false;
            }
        }
    }

    private Sequence m_Seq;

    private const float UnderY = 150.0f;
    private const float DurationFade = 0.3f;
    private const float DelayFade = 1.0f;

    private void Awake()
    {
        m_ImageTarget = GetComponent<Image>();
        m_ImageTarget.enabled = false;
        m_TextReload.gameObject.SetActive(false);

        this.UpdateAsObservable()
            .Where(_ => m_CanTarget)
            .Subscribe(_ => {
                if (PlayData.IsZeroBullet)
                {
                    ShowReload(); // リロードが押下されるまで表示
                }
                else
                {
                    HideReload();
                    
                    if (m_CanTarget)
                    {
                        TouchProcess();
                    }
                }
            }).AddTo(this);
    }

    private void TouchProcess()
    {
        switch (InputManager.GetTouch())
        {
            case ENUM_TOUCH.TOUCH_BEGAN:
            {
                Vector2 _position = InputManager.GetPosition();
                if (_position.y < UnderY)
                {
                    break;
                }
                
                m_TouchBegan = _position;
                m_ImageTarget.rectTransform.position = m_TouchBegan;
                m_ImageTarget.enabled = true;
            }
                break;
            case ENUM_TOUCH.TOUCH_MOVED:
            {
                if (!IsTouchBeganZero)
                {
                    var _tuple = GetDistAndRad(InputManager.GetPosition());
                    
                    float _dist = _tuple.distance * GameDefinitions.TouchCoef;

                    Vector2 _position = m_TouchBegan;
                    _position.x += _dist * Mathf.Cos(_tuple.radian);
                    _position.y += _dist * Mathf.Sin(_tuple.radian);
                    _position = InRange(_position);
                    
                    m_ImageTarget.rectTransform.position = _position;
                }
            }
                break;
            case ENUM_TOUCH.TOUCH_ENDED:
                m_TouchBegan = Vector2.zero;
                m_ImageTarget.enabled = false;
                break;
        }
    }
    
    private (float distance, float radian) GetDistAndRad(Vector2 position)
    {
        if (IsTouchBeganZero)
        {
            return (0.0f, 0.0f);
        }
        
        float _distance = Vector2.Distance(m_TouchBegan, position); // 距離
        
        Vector2 _diff = position - m_TouchBegan;
        float _radian = Mathf.Atan2(_diff.y, _diff.x); // 角度

        return (_distance, _radian);
    }

    private Vector2 InRange(Vector2 vec2)
    {
        vec2.x = Mathf.Max(vec2.x, 0.0f);
        vec2.x = Mathf.Min(vec2.x, m_Scaler.referenceResolution.x);
        
        vec2.y = Mathf.Max(vec2.y, 0.0f);
        vec2.y = Mathf.Min(vec2.y, m_Scaler.referenceResolution.y);

        return vec2;
    }

    private void ShowReload()
    {
        if (m_TextReload.gameObject.activeSelf)
        {
            return; // 既に表示済み
        }

        CanTarget = false;
        
        m_TextReload.alpha = 1.0f;
        m_TextReload.gameObject.SetActive(true);

        m_Seq = DOTween.Sequence()
            .Append(m_TextReload.DOFade(0.0f, DurationFade).SetEase(Ease.InSine))
            .Append(m_TextReload.DOFade(1.0f, DurationFade).SetEase(Ease.OutSine))
            .Append(m_TextReload.DOFade(0.0f, DurationFade).SetEase(Ease.InSine))
            .Append(m_TextReload.DOFade(1.0f, DurationFade).SetEase(Ease.OutSine))
            .AppendInterval(DelayFade)
            .SetLoops(-1, LoopType.Restart);
    }

    private void HideReload()
    {
        m_Seq?.Kill();
        
        if (!m_TextReload.gameObject.activeSelf)
        {
            return; // 既に非表示済み
        }

        m_TextReload.gameObject.SetActive(false);
    }
}
