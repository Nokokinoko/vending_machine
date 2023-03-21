using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class ReloadManager : MonoBehaviour
{
    public static ReloadManager Instance = null;
        
    [SerializeField] private CanvasGroup m_Fader;

    private const float DURATION_FADE = 0.2f;

    private void Awake()
    {
        m_Fader.alpha = 0.0f;
            
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void Reload()
    {
        m_Fader.DOFade(1.0f, DURATION_FADE)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                UnitySceneManager.activeSceneChanged +=
                    (Scene before, Scene after) => m_Fader.DOFade(0.0f, DURATION_FADE).SetEase(Ease.Linear);
                    
                var _scene = UnitySceneManager.GetActiveScene();
                UnitySceneManager.LoadScene(_scene.buildIndex);
            });
    }
}
