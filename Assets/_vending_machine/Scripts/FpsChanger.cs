using UnityEngine;

public class FpsChanger : MonoBehaviour
{
    private const int FrameRate = 60;
    
    private void Awake()
    {
        int _frameRate = FrameRate;
        
#if UNITY_WEBGL
        QualitySettings.vSyncCount = 0;
        
        _frameRate--;
#endif
        
        Application.targetFrameRate = _frameRate;
    }
}
