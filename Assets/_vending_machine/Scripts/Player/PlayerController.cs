using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform m_Model;
    [SerializeField] private ShootManager m_MgrShoot;
    [SerializeField] private ReloadCarController m_CtrlCar;
    [SerializeField] private ImageTargetController m_CtrlTarget;

    private Transform m_Xform;
    private bool m_MoveZ = false;
    private Sequence m_Seq;
    
    private readonly Subject<Unit> m_RxOnRotateFwd = new Subject<Unit>();
    public IObservable<Unit> RxOnRotateFwd => m_RxOnRotateFwd.AsObservable();

    private const float MovePositionZ = 0.05f;
    private const float MoveRotationZ = 2.0f;
    private const float DurationRotationZ = 1.0f;
    private const float DurationRotate = 0.5f;

    public bool IsMaxShoot => m_MgrShoot.IsMax;
    public bool IsZeroShoot => m_MgrShoot.IsZero;
    public void ShootIncrement() => m_MgrShoot.Increment();

    private void Awake()
    {
        m_CtrlCar.CtrlPlayer = this;
        m_Xform = transform;
        
        GameEventManager.OnReceivedAsObservable(GameEvent.GameStart)
            .Subscribe(_ => RotateAndMove())
            .AddTo(this);

        this.UpdateAsObservable()
            .Subscribe(_ => {
                if (m_MoveZ)
                {
                    Vector3 _position = m_Xform.position;
                    _position.z += MovePositionZ;
                    m_Xform.position = _position;
                }

                if (IsZeroShoot)
                {
                    m_CtrlTarget.ShowReload(); // リロードが押下されるまで表示
                }

                if (m_CtrlTarget.CanShoot)
                {
                    m_MgrShoot.DoShoot(m_CtrlTarget.PositionShoot);
                }
            }).AddTo(this);
    }

    public void RotateAndMove()
    {
        m_RxOnRotateFwd.OnNext(Unit.Default);
        
        m_Model.DOLocalRotate(Vector3.zero, DurationRotate)
            .SetEase(Ease.Linear)
            .OnComplete(() => Move());
    }

    private void Move()
    {
        m_MoveZ = true;
        m_CtrlTarget.CanTarget = true;

        m_Seq = DOTween.Sequence()
            .Append(m_Model.DOLocalRotate(new Vector3(0.0f, 0.0f, MoveRotationZ), DurationRotationZ).SetEase(Ease.OutSine))
            .Append(m_Model.DOLocalRotate(Vector3.zero, DurationRotationZ).SetEase(Ease.InSine))
            .Append(m_Model.DOLocalRotate(new Vector3(0.0f, 0.0f, -MoveRotationZ), DurationRotationZ).SetEase(Ease.OutSine))
            .Append(m_Model.DOLocalRotate(Vector3.zero, DurationRotationZ).SetEase(Ease.InSine))
            .SetLoops(-1, LoopType.Restart);
    }

    public void StopAndRotate()
    {
        m_MoveZ = false;
        m_CtrlTarget.CanTarget = false;
        m_CtrlTarget.HideReload();
        m_Seq?.Kill();
        
        m_Model.DOLocalRotate(new Vector3(0.0f, 180.0f, 0.0f), DurationRotate)
            .SetEase(Ease.Linear)
            .OnComplete(() => m_CtrlCar.InReload());
    }
}
