using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextScore;
    [SerializeField] private TextMeshProUGUI m_TextHiScore;

    private const string PrevTextScore = "Score:";
    private const string PrevTextHiScore = "Hi-Score:";

    private void Awake()
    {
        m_TextScore.text = PrevTextScore + "0";
        m_TextHiScore.text = PrevTextHiScore + SaveData.Score;
    }
}
