using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyCreator))]
public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Transform> m_ListPoint = new List<Transform>();

    private EnemyCreator m_CrtEnemy;
    public PlayerController CtrlPlayer { private get; set; }

    private readonly Dictionary<int, List<AbstractEnemy>> m_DictEnemy = new Dictionary<int, List<AbstractEnemy>>();

    private CancellationTokenSource m_Cts;

    private const int MaxEnemy = 10;
    private const float DelayCreateEnemy = 4.0f;
    private const float Distance = 120.0f;

    private void Awake()
    {
        m_CrtEnemy = GetComponent<EnemyCreator>();
        
        for (int i = 0; i < m_ListPoint.Count; i++)
        {
            m_DictEnemy.Add(i, new List<AbstractEnemy>());
        }
        
        m_Cts = new CancellationTokenSource();
        
        GameEventManager.OnReceivedAsObservable(GameEvent.GameStart)
            .Subscribe(_ => CreateEnemy(m_Cts.Token).Forget())
            .AddTo(this);

        GameEventManager.OnReceivedAsObservable(GameEvent.GameDead)
            .Subscribe(_ => m_Cts.Cancel())
            .AddTo(this);
    }

    public bool HasAttackingEnemy()
    {
        foreach (var _pair in m_DictEnemy)
        {
            if (_pair.Value.Exists(_enemy => _enemy.IsAttacking))
            {
                return true;
            }
        }
        return false;
    }

    private int CountEnemy()
    {
        int _count = 0;
        foreach (var _pair in m_DictEnemy)
        {
            _count += _pair.Value.Count;
        }
        return _count;
    }

    private async UniTask CreateEnemy(CancellationToken token)
    {
        while (true)
        {
            await UniTask.WaitUntil(() => CountEnemy() < MaxEnemy, cancellationToken: token);
            
            await UniTask.Delay(TimeSpan.FromSeconds(DelayCreateEnemy), cancellationToken: token);

            int _point = Random.Range(0, m_ListPoint.Count);
            Transform _parent = m_ListPoint[_point];

            AbstractEnemy _enemy = m_CrtEnemy.Create();
            _enemy.transform.parent = _parent;

            Vector3 _position = _enemy.transform.localPosition;
            _enemy.transform.localPosition = new Vector3(0.0f, _position.y, CtrlPlayer.PositionFloatZ + Distance);

            List<AbstractEnemy> _list = m_DictEnemy[_point];
            _list.Add(_enemy);
            _enemy.ListAttached = _list;

            _enemy.RxOnDeath
                .First()
                .Subscribe(_ => {
                    _list.Remove(_enemy);
                    Destroy(_enemy.gameObject);
                }).AddTo(this);
        }
    }
}
