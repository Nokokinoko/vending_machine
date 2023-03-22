using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private List<Transform> m_List = new List<Transform>();

    private const float MoveZ = -10.0f;
    private const float Interval = 50.0f;
    
    private void Awake()
    {
        foreach (Transform _child in transform)
        {
            m_List.Add(_child);
        }
        
        this.ObserveEveryValueChanged(_ => PlayData.Score)
            .Subscribe(_ => {
                Transform _move = m_List.FirstOrDefault(_child => _child.localPosition.z + MoveZ < PlayData.Score);
                if (_move)
                {
                    Vector3 _position = _move.localPosition;
                    _position.z += Interval;
                    _move.localPosition = _position;
                }
            }).AddTo(this);
    }
}
