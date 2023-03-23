using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : SingletonMonoBehaviour<AdsManager>
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private const string IdBanner = "ca-app-pub-7370150359464678/4839205583";
    private const string IdInter = "ca-app-pub-7370150359464678/6004662188";
#elif UNITY_ANDROID
    private const string IdBanner = "ca-app-pub-7370150359464678/5339624805";
    private const string IdInter = "ca-app-pub-7370150359464678/2467701848";
#endif

    private InterstitialAd m_Inter;
    private BannerView m_Banner;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        MobileAds.Initialize(status => { });
    }

    public void LoadInter()
    {
        m_Inter?.Destroy();
        
        AdRequest _req = new AdRequest.Builder().Build();
        InterstitialAd.Load(
            IdInter, _req, ((ad, error) => {
                if (error != null || ad == null)
                {
#if UNITY_EDITOR
                    Debug.Log("Interstitial ad failed: " + error);
#endif
                    return;
                }

                m_Inter = ad;

                m_Inter.OnAdFullScreenContentClosed += () => LoadInter();
                m_Inter.OnAdFullScreenContentFailed += _ => LoadInter();
            })
        );
    }

    public void ShowInter()
    {
        if (m_Inter != null && m_Inter.CanShowAd())
        {
            m_Inter.Show();
        }
    }

    public void ShowBanner()
    {
        m_Banner?.Destroy();
        
        m_Banner = new BannerView(IdBanner, AdSize.Banner, AdPosition.Bottom);

        AdRequest _req = new AdRequest.Builder().Build();
        m_Banner.LoadAd(_req);
    }

    public void HideBanner()
    {
        m_Banner?.Destroy();
    }
}
