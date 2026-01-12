using UnityEngine;

public class ShapesPuzzle : MonoBehaviour
{
    [Header("Materiais")]
    public Material targetDoorMat;
    public Material playerTableMat;

    [Header("Frequências (0=quad,1=tri,2=circ)")]
    public float targetFrequency = 1f;
    public float playerFrequency = 0f;

    void Start()
    {
        SetTargetFrequency(targetFrequency);
        SetPlayerFrequency(playerFrequency);
    }

    public void SetTargetFrequency(float freq)
    {
        targetFrequency = Mathf.Clamp(freq, 0, 2);
        targetDoorMat.SetFloat("_Frequency", targetFrequency);
    }

    public void SetPlayerFrequency(float freq)
    {
        playerFrequency = Mathf.Clamp(freq, 0, 2);
        playerTableMat.SetFloat("_Frequency", playerFrequency);
    }

    public bool IsSolved()
    {
        return Mathf.Approximately(targetFrequency, playerFrequency);
    }
}

