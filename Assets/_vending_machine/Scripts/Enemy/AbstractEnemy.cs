using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class AbstractEnemy : MonoBehaviour
{
    [SerializeField] protected List<Material> m_ListMaterial = new List<Material>(GameDefinitions.MaxEnemyLife);

    [Space]
    [SerializeField] protected SkinnedMeshRenderer m_Renderer;
    
    [Space]
    [SerializeField] private float m_IntervalAttack;
    [SerializeField] private TextMeshPro m_TextDanger;
    
    private int m_Life;

    private bool m_MoveZ = true;
    private bool m_IsAttacking = false;
    public bool IsAttacking => m_IsAttacking;

    private List<AbstractEnemy> m_ListAttached;
    public List<AbstractEnemy> ListAttached
    {
        set
        {
            // EnemyManagerが管理する所属List
            m_ListAttached = value;

            // 所属数が減ったなら進行フラグを立てる
            this.ObserveEveryValueChanged(_ => m_ListAttached.Count)
                .Pairwise()
                .Where(pair => pair.Current < pair.Previous)
                .Subscribe(_ => m_MoveZ = true)
                .AddTo(this);
        }
    }

    protected Transform m_Xform;
    private float PositionZ => m_Xform.localPosition.z;
    private Animator m_Animator;
    private Collider m_Collider;

    private CancellationTokenSource m_Cts;
    
    private readonly Subject<Unit> m_RxOnDeath = new Subject<Unit>();
    public IObservable<Unit> RxOnDeath => m_RxOnDeath.AsObservable();

    private const float MovePositionZ = -0.05f;
    
    private const string BoolIdle = "Idle";
    
    private const float Danger = 2.0f;
    private const string BoolAttack = "Attack";
    private const string StateAttack = "Attack";
    
    private const string StateDeath = "Death";
    private const float DelayDeath = 0.2f;

    private void Awake()
    {
        m_Xform = transform;
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();

        m_Cts = new CancellationTokenSource();
        
        m_Collider.OnTriggerEnterAsObservable()
            .Subscribe(
                _collider =>
                {
                    if (_collider.gameObject.CompareTag(GameDefinitions.TagBullet))
                    {
                        m_Life--;
                        Destroy(_collider.gameObject);

                        if (m_Life <= 0)
                        {
                            ToDeath().Forget();
                        }
                        else
                        {
                            UpdateMaterial();
                        }
                    }
                    
                    if (_collider.gameObject.CompareTag(GameDefinitions.TagPlayer))
                    {
                        ToIdle();
                        ToAttack(m_Cts.Token).Forget();
                    }
                })
            .AddTo(this);
        
        m_TextDanger.gameObject.SetActive(false);
        
        this.UpdateAsObservable()
            .Where(_ => m_MoveZ)
            .Subscribe(_ => Move())
            .AddTo(this);
        
        this.OnDestroyAsObservable()
            .Subscribe(_ => m_Cts.Dispose())
            .AddTo(this);

        GameEventManager.OnReceivedAsObservable(GameEvent.GameDead)
            .Subscribe(_ => m_Cts.Cancel())
            .AddTo(this);
    }

    private void Move()
    {
        if (IsAttacking)
        {
            // 攻撃位置まで到達していたなら進行しない
            m_MoveZ = false;
            return;
        }
        
        float _prev = 0.0f;
        foreach (AbstractEnemy _enemy in m_ListAttached)
        {
            if (_enemy.PositionZ < PositionZ)
            {
                _prev = Mathf.Max(_prev, _enemy.PositionZ);
            }
        }

        if (0.0f < _prev && PositionZ <= _prev + GameDefinitions.IntervalEnemy)
        {
            ToIdle();
            return;
        }

        if (m_Animator.GetBool(BoolIdle))
        {
            m_Animator.SetBool(BoolIdle, false);
        }
        
        Vector3 _position = m_Xform.localPosition;
        _position.z += MovePositionZ;
        m_Xform.localPosition = _position;
    }

    public void SetEnemyLife(int life)
    {
        m_Life = life;
        m_Life = Mathf.Max(m_Life, 1);
        m_Life = Mathf.Min(m_Life, m_ListMaterial.Count);
        UpdateMaterial();
    }

    protected Material GetMaterialByLife() => m_ListMaterial[m_Life - 1];

    protected abstract void UpdateMaterial();

    private void ToIdle()
    {
        m_MoveZ = false;
        m_Animator.SetBool(BoolIdle, true);
    }

    private async UniTask ToAttack(CancellationToken token)
    {
        m_IsAttacking = true;
        
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(m_IntervalAttack), cancellationToken: token);
            
            m_TextDanger.gameObject.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(Danger), cancellationToken: token);
            
            m_Animator.SetBool(BoolAttack, true);

            await UniTask.WaitUntil(
                () => m_Animator.GetCurrentAnimatorStateInfo(0).IsName(StateAttack), cancellationToken: token
            );

            await UniTask.WaitUntil(
                () => 1.0f <= m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, cancellationToken: token
            );
            
            PlayData.DecrementLife();
            m_Animator.SetBool(BoolAttack, false);
            m_TextDanger.gameObject.SetActive(false);
        }
    }
    
    protected abstract bool HasDeath();

    private async UniTask ToDeath()
    {
        m_Cts.Cancel();
        m_Collider.enabled = false;
        m_TextDanger.gameObject.SetActive(false);

        if (HasDeath())
        {
            Fall();
            m_Animator.Play(StateDeath);
            
            await UniTask.WaitUntil(() => m_Animator.GetCurrentAnimatorStateInfo(0).IsName(StateDeath));

            await UniTask.WaitUntil(() => 1.0f <= m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(DelayDeath));
        }
        
        m_RxOnDeath.OnNext(Unit.Default);
    }

    protected virtual void Fall()
    {
        // do not process
    }
}
