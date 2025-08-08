using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    public UnityEvent<int> OnScoreChanged;
    public int Score { get; private set; }

    public void AddScore(int amount)
    {
        Score += amount;
        OnScoreChanged.Invoke(Score);
    }
}
