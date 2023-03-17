using UnityEngine;

public static partial class SaveData
{
    private const string KEY_HI_SCORE = "HiScore";

    public static int HiScore
    {
        get => GetData(KEY_HI_SCORE, 0);
        set => SetData(KEY_HI_SCORE, Mathf.Max(value, 0));
    }
}
