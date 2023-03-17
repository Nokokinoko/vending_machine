using TMPro;
using UniRx;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextScore;
    [SerializeField] private TextMeshProUGUI m_TextHiScore;

    private const string PrevTextScore = "Score:";
    private const string PrevTextHiScore = "Hi-Score:";

    private void Awake()
    {
        UpdateScore();
        m_TextHiScore.text = PrevTextHiScore + SaveData.HiScore;

        this.ObserveEveryValueChanged(_ => PlayData.Score)
            .Subscribe(_ => UpdateScore())
            .AddTo(this);
    }

    private void UpdateScore()
    {
        m_TextScore.text = PrevTextScore + PlayData.Score;
    }
}
