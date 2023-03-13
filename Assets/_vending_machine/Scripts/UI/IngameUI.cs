using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private List<Image> m_ListHearts = new List<Image>();
    
    [Space]
    [SerializeField] private Sprite m_Heart;
    [SerializeField] private Sprite m_Heartbreak;

    [SerializeField] private Color m_ColorHeart;
    [SerializeField] private Color m_ColorHeartbreak;

    [Space]
    [SerializeField] private Button m_ButtonReload;
}
