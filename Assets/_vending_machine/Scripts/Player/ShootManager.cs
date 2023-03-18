using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShootManager : MonoBehaviour
{
    [SerializeField] private List<Transform> m_ListBullet = new List<Transform>();

    private Camera m_Camera;
    
    public PlayerController CtrlPlayer { private get; set; }

    private const float Power = 100.0f;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    public void DoShoot()
    {
        int _rand = Random.Range(0, m_ListBullet.Count);
        Transform _bullet = Instantiate(m_ListBullet[_rand], transform);

        Ray _ray = m_Camera.ScreenPointToRay(CtrlPlayer.PositionShoot);
        Vector3 _dir = _ray.direction;
        _bullet.GetComponent<BulletController>().Shoot(_dir.normalized * Power);

        PlayData.DecrementBullet();
    }
}
