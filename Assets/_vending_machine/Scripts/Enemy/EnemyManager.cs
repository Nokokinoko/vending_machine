using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_CtrlPlayer;
    [SerializeField] private Transform m_PrefabFrog;
    [SerializeField] private Transform m_PrefabSnake;
    [SerializeField] private Transform m_PrefabWasp;

    private readonly List<Transform> m_ListPrefab = new List<Transform>();
    private readonly List<AbstractEnemy> m_ListEnemy = new List<AbstractEnemy>();

    private void Awake()
    {
        m_ListPrefab.Add(m_PrefabFrog);
        m_ListPrefab.Add(m_PrefabSnake);
        m_ListPrefab.Add(m_PrefabWasp);
        
        this.ObserveEveryValueChanged(_ => m_CtrlPlayer.PositionDivideZ)
            .Subscribe(_ => { })
            .AddTo(this);
    }

    public bool HasEncountEnemy()
    {
        return m_ListEnemy.Exists(_enemy => _enemy.IsEncount);
    }

    public void Create()
    {
        int _rand = Random.Range(0, m_ListPrefab.Count);
        Transform _prefab = Instantiate(m_ListPrefab[_rand], transform);

        AbstractEnemy _enemy = _prefab.GetComponent<AbstractEnemy>();
        m_ListEnemy.Add(_enemy);

        _enemy.RxOnDeath
            .First()
            .Subscribe(_ => {
                m_ListEnemy.Remove(_enemy);
                Destroy(_enemy.gameObject);
            }).AddTo(this);
    }
}
