using UnityEngine;

public class ShootManager : MonoBehaviour
{
    private int m_NumShoot = GameDefinitions.MaxShoot;
    public int NumShoot => m_NumShoot;

    public bool IsMax => GameDefinitions.MaxShoot <= m_NumShoot;
    public bool IsZero => m_NumShoot <= 0;

    public void Increment()
    {
        m_NumShoot++;
        m_NumShoot = Mathf.Min(m_NumShoot, GameDefinitions.MaxShoot);
    }

    public void DoShoot(Vector2 position)
    {
        if (IsZero)
        {
            return;
        }
        
        // decrement
        m_NumShoot--;
        m_NumShoot = Mathf.Max(m_NumShoot, 0);
    }
}
