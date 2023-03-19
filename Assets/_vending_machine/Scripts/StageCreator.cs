using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    [SerializeField] private Transform m_PrefabRoad;
    [SerializeField] private PlayerController m_CtrlPlayer;
    
    private TransformPool m_RoadPool;
    private readonly List<Road> m_ListRoad = new List<Road>();
    private int m_RoadZ = 0;

    private const int BorderCreate = 120;
    private const int BorderDelete = -20;
    
    private void Awake()
    {
        m_RoadPool = new TransformPool(m_PrefabRoad, transform);

        this.OnDestroyAsObservable()
            .Subscribe(_ => m_RoadPool.Dispose())
            .AddTo(this);
    }

    private void Start()
    {
        foreach (Transform _child in transform)
        {
            Road _road = _child.GetComponent<Road>();
            if (_road)
            {
                m_RoadZ = Mathf.Max(m_RoadZ, _road.PositionZ);
            }
        }

        this.ObserveEveryValueChanged(_ => m_CtrlPlayer.PositionIntZ)
            .Subscribe(_ => CheckPlayerPosition())
            .AddTo(this);
    }

    private void CheckPlayerPosition()
    {
        int _z = m_CtrlPlayer.PositionIntZ;
        PlayData.SetScore(_z);
        
        // create
        if (m_RoadZ <= _z + BorderCreate)
        {
            CreateRoad();
        }

        // delete
        Road _min = m_ListRoad.OrderBy(_road => _road.PositionZ).FirstOrDefault();
        if (_min && _min.PositionZ < _z + BorderDelete)
        {
            m_RoadPool.Return(_min.transform);
            m_ListRoad.Remove(_min);
        }
    }

    private void CreateRoad()
    {
        m_RoadZ += GameDefinitions.FloorZ;
        float _z = m_RoadZ;

        Transform _pool = m_RoadPool.Rent();
        _pool.parent = transform;
        _pool.localPosition = new Vector3(0.0f, 0.0f, _z);

        m_ListRoad.Add(_pool.GetComponent<Road>());
    }
}
