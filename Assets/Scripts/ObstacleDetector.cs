using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;

    [Header("Dodge Settings")]
    [Tooltip("Distance minimale pour esquiver (en mètres)")]
    public float dodgeDistance = 0.25f;

    [Header("Obstacle Info")]
    public bool isObstacle = false;
    public Vector3 obstacleDirection;

    private bool hasBeenDodged = false;
    private bool hasHitPlayer = false;
    private float closestDistance = float.MaxValue;

    void Start()
    {
        // Trouve la caméra
        if (playerHead == null)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            if (cam != null)
            {
                playerHead = cam.transform;
                Debug.Log("✅ Main Camera trouvée");
            }
            else
            {
                Debug.LogError("❌ Main Camera introuvable ! Vérifie le tag 'MainCamera'");
            }
        }

        DetermineObstacleDirection();
    }

    void DetermineObstacleDirection()
    {
        if (playerHead == null) return;

        Vector3 relativePos = transform.position - playerHead.position;
        obstacleDirection = relativePos.normalized;

        Debug.Log($"🎯 Obstacle direction: {GetDirectionName()}");
    }

    string GetDirectionName()
    {
        if (Mathf.Abs(obstacleDirection.x) < 0.3f)
            return "CENTRE (se baisser)";
        else if (obstacleDirection.x < 0)
            return "GAUCHE (esquiver à DROITE)";
        else
            return "DROITE (esquiver à GAUCHE)";
    }

    void Update()
    {
        if (!isObstacle) return;
        if (hasBeenDodged || hasHitPlayer) return;
        if (playerHead == null) return;

        float distance = Vector3.Distance(transform.position, playerHead.position);

        // Suivi distance minimale
        if (distance < closestDistance)
        {
            closestDistance = distance;
        }

        // DEBUG
        Debug.Log($"📏 Distance: {distance:F2}m (min: {closestDistance:F2}m)");

        // Zone de détection élargie
        if (distance < 2.5f && distance > 0.2f)
        {
            CheckDodge();
        }

        // Vérifie si passé derrière
        if (transform.position.z < playerHead.position.z - 1.5f)
        {
            if (!hasHitPlayer && !hasBeenDodged)
            {
                OnMissedObstacle();
            }
        }
    }

    void CheckDodge()
    {
        Vector3 relativePos = playerHead.position - transform.position;

        // Gauche → Droite
        if (obstacleDirection.x < -0.3f && relativePos.x > dodgeDistance)
        {
            OnSuccessfulDodge("DROITE");
        }
        // Droite → Gauche
        else if (obstacleDirection.x > 0.3f && relativePos.x < -dodgeDistance)
        {
            OnSuccessfulDodge("GAUCHE");
        }
        // Centre → Bas
        else if (Mathf.Abs(obstacleDirection.x) < 0.3f && relativePos.y < -dodgeDistance)
        {
            OnSuccessfulDodge("BAS");
        }
    }

    void OnSuccessfulDodge(string direction)
    {
        if (hasBeenDodged) return;

        hasBeenDodged = true;
        Debug.Log($"✅ ESQUIVE RÉUSSIE ! Direction: {direction} (+5 points)");

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.AddScore(5);

        Destroy(gameObject);
    }

    void OnMissedObstacle()
    {
        Debug.Log($"⚠️ Obstacle manqué - Distance min: {closestDistance:F2}m");
        Destroy(gameObject);
    }

    // ✅ UTILISE OnTriggerEnter POUR LES COLLIDERS EN MODE TRIGGER
    void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer || hasBeenDodged) return;

        Debug.Log($"🔍 Trigger avec: {other.gameObject.name} (Tag: {other.tag})");

        // Tête
        if (other.CompareTag("MainCamera"))
        {
            hasHitPlayer = true;
            Debug.Log("💀 OBSTACLE TOUCHÉ LA TÊTE ! (-20 points)");

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.AddScore(-20);

            Destroy(gameObject);
        }
        // Main
        else if (other.CompareTag("Hand"))
        {
            hasHitPlayer = true;
            Debug.Log("❌ OBSTACLE TOUCHÉ AVEC LA MAIN ! (-10 points)");

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.AddScore(-10);

            Destroy(gameObject);
        }
    }

    // ✅ GARDE AUSSI OnCollisionEnter AU CAS OÙ
    void OnCollisionEnter(Collision collision)
    {
        if (hasHitPlayer || hasBeenDodged) return;

        Debug.Log($"🔍 Collision avec: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        // Tête
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            hasHitPlayer = true;
            Debug.Log("💀 OBSTACLE TOUCHÉ LA TÊTE ! (-20 points)");

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.AddScore(-20);

            Destroy(gameObject);
        }
        // Main
        else if (collision.gameObject.CompareTag("Hand"))
        {
            hasHitPlayer = true;
            Debug.Log("❌ OBSTACLE TOUCHÉ AVEC LA MAIN ! (-10 points)");

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.AddScore(-10);

            Destroy(gameObject);
        }
    }
}
