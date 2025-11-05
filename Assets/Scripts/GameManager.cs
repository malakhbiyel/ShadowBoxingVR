using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int score = 0;

    // Événement pour notifier l'UI quand le score change
    public event System.Action<int> OnScoreChanged;

    void Start()
    {
        // Initialise l'affichage du score à 0
        OnScoreChanged?.Invoke(score);
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"📊 Score: {score} ({points:+0;-#})");

        // Notifie l'UI que le score a changé
        OnScoreChanged?.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }
}
