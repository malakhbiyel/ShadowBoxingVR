using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float lifeTime = 5f;

    [Header("Type")]
    public bool isObstacle = false;

    private bool isDestroyed = false;

    void Start()
    {
        // Auto-destruction après X secondes
        Destroy(gameObject, lifeTime);

        // Si c'est un obstacle, ajoute le script ObstacleDetector
        if (isObstacle)
        {
            gameObject.AddComponent<ObstacleDetector>().isObstacle = true;
        }
    }

    void Update()
    {
        // Avance vers le joueur (direction -Z)
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;

        // Si c'est un OBSTACLE, laisse ObstacleDetector gérer
        if (isObstacle)
        {
            // Géré par ObstacleDetector.cs
            return;
        }

        // Si c'est une CIBLE, détecte le punch
        if (collision.gameObject.CompareTag("Hand"))
        {
            Debug.Log($"🔍 COLLISION avec : {collision.gameObject.name}");

            PunchDetector punch = collision.gameObject.GetComponent<PunchDetector>();

            if (punch != null && punch.speed >= 1.5f)
            {
                Debug.Log($"🎯 CIBLE TOUCHÉE ! +10 points (vitesse: {punch.speed:F2})");
                
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null) gm.AddScore(10);

                // Effet visuel (flash blanc)
                Renderer rend = GetComponent<Renderer>();
                if (rend != null) rend.material.color = Color.white;

                // Détruit la cible
                Destroy(gameObject);
                isDestroyed = true;
            }
            else
            {
                Debug.Log($"⚠️ Coup trop faible (vitesse: {punch.speed:F2})");
            }
        }
    }
}
