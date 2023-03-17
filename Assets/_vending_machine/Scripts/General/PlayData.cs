using UnityEngine;

public static class PlayData
{
    private static int m_Score;
    private static int m_Life;
    private static int m_Bullet;

    public static int Score => m_Score;
    
    public static int Life => m_Life;
    public static bool IsMaxLife => GameDefinitions.MaxLife <= m_Life;
    public static bool IsZeroLife => m_Life <= 0;
    
    public static int Bullet => m_Bullet;
    public static bool IsMaxBullet => GameDefinitions.MaxBullet <= m_Bullet;
    public static bool IsZeroBullet => m_Bullet <= 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        m_Score = 0;
        m_Life = GameDefinitions.MaxLife;
        m_Bullet = GameDefinitions.MaxBullet;
    }

    public static void AddScore(int add)
    {
        m_Score += add;
    }

    public static void IncrementLife()
    {
        m_Life++;
        m_Life = Mathf.Min(m_Life, GameDefinitions.MaxLife);
    }

    public static void DecrementLife()
    {
        m_Life--;
        m_Life = Mathf.Max(m_Life, 0);
    }

    public static void IncrementBullet()
    {
        m_Bullet++;
        m_Bullet = Mathf.Min(m_Bullet, GameDefinitions.MaxBullet);
    }

    public static void DecrementBullet()
    {
        m_Bullet--;
        m_Bullet = Mathf.Max(m_Bullet, 0);
    }
}
