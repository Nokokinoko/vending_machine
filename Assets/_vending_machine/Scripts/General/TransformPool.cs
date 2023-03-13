using UniRx.Toolkit;
using UnityEngine;

public class TransformPool : ObjectPool<Transform>
{
    private readonly Transform _prefab;
    private readonly Transform _parent;

    public TransformPool(Transform prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
        
    protected override Transform CreateInstance()
    {
        var instance = Object.Instantiate(_prefab, _parent);
        instance.gameObject.SetActive(false);
        return instance;
    }

    protected override void OnBeforeRent(Transform instance)
    {
        base.OnBeforeRent(instance);
        instance.gameObject.SetActive(true);
    }

    protected override void OnBeforeReturn(Transform instance)
    {
        base.OnBeforeReturn(instance);
        instance.gameObject.SetActive(false);
    }
}
