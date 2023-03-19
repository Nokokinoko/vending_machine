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
public abstract class AbstractEnemy : MonoBehaviour
{
    [SerializeField] private int m_Life;
    [SerializeField] private List<Material> m_ListMaterial = new List<Material>();

    [Space]
    [SerializeField] private float m_IntervalAttack;
    [SerializeField] private TextMeshPro m_TextDanger;

    private bool m_MoveZ = true;
    public bool IsEncount => !m_MoveZ;

    private Transform m_Xform;
    private Animator m_Animator;
    private Collider m_Collider;

    private CancellationTokenSource m_Cts;
    
    private readonly Subject<Unit> m_RxOnDeath = new Subject<Unit>();
    public IObservable<Unit> RxOnDeath => m_RxOnDeath.AsObservable();

    private const float MovePositionZ = -0.02f;
    
    private const string BoolIdle = "Idle";
    
    private const float Danger = 3.0f;
    private const string TriggerAttack = "Attack";
    private const string StateAttack = "Attack";
    
    private const string StateDeath = "Death";
    private const float DelayDeath = 0.2f;

    private void Awake()
    {
        m_Xform = transform;
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();

        m_Life = Mathf.Max(m_Life, 1);
        m_Life = Mathf.Min(m_Life, m_ListMaterial.Count);
        UpdateMaterial();

        m_Cts = new CancellationTokenSource();
        
        m_Collider.OnTriggerEnterAsObservable()
            .Subscribe(
                _collider =>
                {
                    if (_collider.gameObject.CompareTag(GameDefinitions.TagBullet))
                    {
                        m_Life--;

                        if (m_Life <= 0)
                        {
                            ToDeath().Forget();
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
            .Subscribe(_ => {
                Vector3 _position = m_Xform.position;
                _position.z += MovePositionZ;
                m_Xform.position = _position;
            }).AddTo(this);
        
        this.OnDestroyAsObservable()
            .Subscribe(_ => m_Cts.Dispose())
            .AddTo(this);
    }

    protected abstract void UpdateMaterial();
    protected abstract bool HasIdle();

    private void ToIdle()
    {
        m_MoveZ = false;
        
        if (HasIdle())
        {
            m_Animator.SetBool(BoolIdle, true);
        }
    }

    private async UniTask ToAttack(CancellationToken token)
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(m_IntervalAttack), cancellationToken: token);
            
            m_TextDanger.gameObject.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(Danger), cancellationToken: token);
            
            m_Animator.SetTrigger(TriggerAttack);

            await UniTask.WaitUntil(
                () => m_Animator.GetCurrentAnimatorStateInfo(0).IsName(StateAttack), cancellationToken: token
            );

            await UniTask.WaitUntil(
                () => 1.0f <= m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, cancellationToken: token
            );
            
            PlayData.DecrementLife();
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
            m_Animator.Play(StateDeath);
            Fall();
            
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
