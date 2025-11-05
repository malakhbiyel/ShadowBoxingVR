using UnityEngine;
using TMPro; // ⚠️ IMPORTANT : Pour utiliser TextMeshPro

public class ScoreUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Référence au TextMeshPro pour afficher le score")]
    public TextMeshProUGUI scoreText;
    
    [Header("Settings")]
    [Tooltip("Préfixe à afficher avant le score")]
    public string scorePrefix = "Score: ";
    
    private GameManager gameManager;
    
    void Start()
    {
        // 1️⃣ Trouve le GameManager dans la scène
        gameManager = FindFirstObjectByType<GameManager>();
        
        // 2️⃣ Vérification de sécurité
        if (gameManager == null)
        {
            Debug.LogError("❌ GameManager introuvable ! Assure-toi qu'il est dans la scène.");
            return;
        }
        
        if (scoreText == null)
        {
            Debug.LogError("❌ ScoreText (TextMeshPro) non assigné dans l'Inspector !");
            return;
        }
        
        // 3️⃣ S'abonne à l'événement de changement de score
        gameManager.OnScoreChanged += UpdateScoreDisplay;
        
        // 4️⃣ Initialise l'affichage avec le score actuel
        UpdateScoreDisplay(gameManager.GetScore());
        
        Debug.Log("✅ ScoreUI initialisé avec succès !");
    }
    
    void OnDestroy()
    {
        // 5️⃣ Se désabonne pour éviter les erreurs quand l'objet est détruit
        if (gameManager != null)
        {
            gameManager.OnScoreChanged -= UpdateScoreDisplay;
        }
    }
    
    // 6️⃣ Cette méthode est appelée automatiquement quand le score change
    void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + newScore.ToString();
            Debug.Log($"🎯 UI mise à jour : {scoreText.text}");
        }
    }
}
