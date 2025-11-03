using UnityEngine;
// Ajoute cette ligne uniquement si tu utilises XR Toolkit
#if UNITY_XR_MANAGEMENT
using UnityEngine.XR.Interaction.Toolkit;
#endif

public class Projectile : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float lifeTime = 5f;

    [Header("Gameplay")]
    public bool isObstacle = false;
    public bool isHologran = false; // 🆕 Nouveau type
    public int targetScore = 10;
    public int obstaclePenalty = -10;
    public int hologranPenalty = -5; // 🆕 Peut ajuster le score du hologran
    [Tooltip("Vitesse minimale pour valider un coup")]
    public float requiredSpeed = 0.3f;

    [Header("Impact Feedback")]
    public GameObject impactEffectPrefab;
    public AudioClip impactSound;
    public float impactVolume = 0.8f;

    private AudioSource audioSource;

    void Start()
    {
        // Auto-détruit après X secondes
        Destroy(gameObject, lifeTime);

        // Configure l’audio
        if (impactSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        // Fait avancer le projectile vers le joueur (-Z)
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🚀 Trigger détecté avec: {other.name}, Tag: {other.tag}");

        // Vérifie que c'est bien la main
        if (!other.CompareTag("Hand"))
        {
            Debug.Log("⚠️ Ce n'est pas la main, aucune action.");
            return;
        }

        Debug.Log("✋ Collision avec la main confirmée !");
        GameManager gm = FindFirstObjectByType<GameManager>();
        PunchDetector punch = other.GetComponent<PunchDetector>();

        if (punch != null)
            Debug.Log($"💨 Vitesse de la main: {punch.speed:F2} m/s");

        // 🧠 Gestion des différents types de projectiles
        if (isObstacle)
        {
            gm?.AddScore(obstaclePenalty);
            Debug.Log($"❌ OBSTACLE TOUCHÉ ! Score: {gm?.GetScore()}");
        }
        else if (isHologran)
        {
            gm?.AddScore(hologranPenalty);
            Debug.Log($"🌈 HOLOGRAN TOUCHÉ ! {hologranPenalty} points — Score total: {gm?.GetScore()}");
        }
        else if (punch != null && punch.speed >= requiredSpeed)
        {
            gm?.AddScore(targetScore);
            Debug.Log($"🎯 CIBLE TOUCHÉE ! +{targetScore} points — Score total: {gm?.GetScore()}");
        }
        else
        {
            Debug.Log($"⚠️ Coup trop lent ({punch?.speed:F2} m/s) — Aucun point.");
            return;
        }

        // Effets visuels / sonores
        ShowImpactEffect();
        PlayImpactSound();

        // Optionnel : vibration XR
        TrySendHaptic(other);

        // Détruit la cible
        Destroy(gameObject);
    }

    void ShowImpactEffect()
    {
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void PlayImpactSound()
    {
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound, impactVolume);
        }
    }

#if UNITY_XR_MANAGEMENT
    void TrySendHaptic(Collider hand)
    {
        var controller = hand.GetComponent<XRController>();
        if (controller != null && controller.haptics != null)
        {
            controller.haptics.SendHapticImpulse(0.8f, 0.15f);
        }
    }
#else
    void TrySendHaptic(Collider hand) { /* Pas de XR -> rien */ }
#endif
}
