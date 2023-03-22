using System;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform m_Model;
    [SerializeField] private ShootManager m_MgrShoot;
    [SerializeField] private ReloadCarController m_CtrlCar;
    [SerializeField] private ImageTargetController m_CtrlTarget;
    [SerializeField] private EnemyManager m_MgrEnemy;

    public Vector2 PositionShoot => m_CtrlTarget.PositionShoot;

    private Transform m_Xform;
    private bool m_Start = false;
    private bool m_MoveZ = false;
    private Sequence m_Seq;

    private CinemachineImpulseSource m_Impulse;

    private CancellationTokenSource m_Cts;

    public float PositionFloatZ => m_Xform.position.z;
    public int PositionIntZ => Mathf.FloorToInt(PositionFloatZ);
    
    private readonly Subject<Unit> m_RxOnRotateFwd = new Subject<Unit>();
    public IObservable<Unit> RxOnRotateFwd => m_RxOnRotateFwd.AsObservable();

    private const float MovePositionZ = 0.05f;
    private const float DurationRotate = 0.5f;
    private const float MoveRotationZ = 2.0f;
    private const float DurationRotationZ = 0.5f;

    private const float DurationPunchScale = 0.2f;
    
    private const float DelayShoot = 1.0f;

    private void Awake()
    {
        m_MgrShoot.CtrlPlayer = this;
        m_CtrlCar.CtrlPlayer = this;
        m_MgrEnemy.CtrlPlayer = this;
        m_Xform = transform;

        m_Impulse = GetComponent<CinemachineImpulseSource>();

        m_Cts = new CancellationTokenSource();

        this.UpdateAsObservable()
            .Where(_ => m_MoveZ)
            .Subscribe(_ => {
                Vector3 _position = m_Xform.position;
                _position.z += MovePositionZ;
                m_Xform.position = _position;
            }).AddTo(this);

        this.ObserveEveryValueChanged(_ => PlayData.Life)
            .Where(_ => PlayData.Life < GameDefinitions.MaxLife)
            .Subscribe(_ => m_Impulse.GenerateImpulse())
            .AddTo(this);

        this.ObserveEveryValueChanged(_ => m_MgrEnemy.HasAttackingEnemy())
            .Where(_ => m_Start)
            .Subscribe(_encount => Move(_encount))
            .AddTo(this);
        
        GameEventManager.OnReceivedAsObservable(GameEvent.GameStart)
            .Subscribe(_ => RotateAndMove())
            .AddTo(this);

        GameEventManager.OnReceivedAsObservable(GameEvent.GameDead)
            .Subscribe(_ => {
                m_Cts.Cancel();
                m_CtrlTarget.CanTarget = false;
                m_Start = false;
            })
            .AddTo(this);
        
        Shoot(m_Cts.Token).Forget();

        this.OnDestroyAsObservable()
            .Subscribe(_ => m_Cts.Dispose())
            .AddTo(this);
    }

    private void Move(bool encount)
    {
        if (encount)
        {
            Stop();
        }
        else
        {
            GoFwd();
        }
    }

    public void RotateAndMove()
    {
        m_RxOnRotateFwd.OnNext(Unit.Default);
        
        m_Model.DOLocalRotate(Vector3.zero, DurationRotate)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                m_Start = true; // 初回のみ有効化
                Move(m_MgrEnemy.HasAttackingEnemy());
            });
    }

    private void GoFwd()
    {
        if (m_MoveZ)
        {
            return;
        }
        
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
        Stop();
        m_CtrlTarget.CanTarget = false;
        
        m_Model.DOLocalRotate(new Vector3(0.0f, 180.0f, 0.0f), DurationRotate)
            .SetEase(Ease.Linear)
            .OnComplete(() => m_CtrlCar.InReload());
    }

    private void Stop()
    {
        if (!m_MoveZ)
        {
            return;
        }

        m_MoveZ = false;
        m_Seq?.Kill();

        m_Model.DOLocalRotate(Vector3.zero, DurationRotationZ).SetEase(Ease.Linear);
    }

    public void PunchScale()
    {
        m_Model.DOPunchScale(Vector3.one * 0.01f, DurationPunchScale);
    }

    private async UniTask Shoot(CancellationToken token)
    {
        while (true)
        {
            await UniTask.WaitUntil(() => m_CtrlTarget.CanShoot && !PlayData.IsZeroBullet, cancellationToken: token);

            m_MgrShoot.DoShoot();

            await UniTask.Delay(TimeSpan.FromSeconds(DelayShoot), cancellationToken: token);
        }
    }
}
