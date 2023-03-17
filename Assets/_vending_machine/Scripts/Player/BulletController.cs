using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    private const float FallY = -10.0f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        this.UpdateAsObservable()
            .Where(_ => transform.position.y < FallY)
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }

    public void Shoot(Vector3 dir)
    {
        m_Rigidbody.AddForce(dir, ForceMode.Impulse);
    }
}
