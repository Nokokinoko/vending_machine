using UniRx;
using UnityEngine;

public static partial class SaveData
{
    public static void Apply()
    {
        DataSerializer.Apply();
    }
    
    private static T GetData<T>(string key, T defaultValue)
    {
        if (!DataSerializer.ExistsData(key))
        {
            DataSerializer.SetData(key, defaultValue);
        }

        return DataSerializer.GetData<T>(key);
    }
    
    private static void SetData<T>(string key, T value)
    {
        DataSerializer.SetData(key, value);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Observable.Merge(
                MainThreadDispatcher.OnApplicationPauseAsObservable().Where(pause => pause).AsUnitObservable(),
                MainThreadDispatcher.OnApplicationQuitAsObservable()
            )
            .Subscribe(_ => Apply());
    }
}
