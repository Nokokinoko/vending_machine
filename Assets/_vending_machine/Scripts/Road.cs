using UnityEngine;

public class Road : MonoBehaviour
{
    public int PositionZ => Mathf.FloorToInt(transform.localPosition.z);
}
