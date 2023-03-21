using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField] private Transform m_PrefabFrog;
    [SerializeField] private Transform m_PrefabSnake;
    [SerializeField] private Transform m_PrefabWasp;

    private const int IntervalKind = 100;
    private const int IntervalLife = 500;

    public AbstractEnemy Create()
    {
        int _score = PlayData.Score;
        List<Transform> _list = new List<Transform>();
        _list.Add(m_PrefabFrog);
        
        int _valKind = Mathf.FloorToInt((float)_score / IntervalKind);
        switch (_valKind)
        {
            case 0: // -100
                break;
            case 1: // 100-200
                _list.Add(m_PrefabSnake);
                break;
            default: // 200-
                _list.Add(m_PrefabSnake);
                _list.Add(m_PrefabWasp);
                break;
        }
        
        int _rand = Random.Range(0, _list.Count);
        Transform _prefab = Instantiate(_list[_rand]);
        AbstractEnemy _enemy = _prefab.GetComponent<AbstractEnemy>();
        
        int _valLife = Mathf.FloorToInt((float)_score / IntervalLife) + 1;
        _valLife = Mathf.Min(_valLife, GameDefinitions.MaxEnemyLife);
        _enemy.SetEnemyLife(_valLife);
        
        return _enemy;
    }
}
