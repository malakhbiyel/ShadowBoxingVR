using UnityEngine;
using System.Collections;

public class ShuttleIntroMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Vitesse de déplacement sur l'axe Z")]
    public float moveSpeed = 10f;
    
    [Tooltip("Position de départ sur Z")]
    public float startZ = -100f;
    
    [Tooltip("Position d'arrivée sur Z")]
    public float endZ = -45f;
    
    [Header("Rotation Settings")]
    [Tooltip("Angle de rotation sur Y (en degrés)")]
    public float rotationAngle = 90f;
    
    [Tooltip("Durée de la rotation (en secondes)")]
    public float rotationDuration = 1.5f;
    
    [Header("Optional Effects")]
    public ParticleSystem engineParticles;
    public AudioSource engineSound;
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent onMovementComplete;
    public UnityEngine.Events.UnityEvent onRotationComplete;
    
    private bool isMoving = false;
    private bool hasCompleted = false;

    void Start()
    {
        // Place le shuttle à la position de départ
        Vector3 startPos = transform.position;
        startPos.z = startZ;
        transform.position = startPos;
        
        // Démarre l'animation
        StartCoroutine(ShuttleSequence());
    }

    IEnumerator ShuttleSequence()
    {
        isMoving = true;
        
        // Active les effets
        if (engineParticles != null)
            engineParticles.Play();
        
        if (engineSound != null)
            engineSound.Play();
        
        // Phase 1 : Déplacement sur l'axe Z
        yield return StartCoroutine(MoveAlongZ());
        
        // Événement de fin de mouvement
        onMovementComplete?.Invoke();
        
        // Petite pause avant la rotation
        yield return new WaitForSeconds(0.3f);
        
        // Phase 2 : Rotation de 90° sur l'axe Y
        yield return StartCoroutine(RotateOnY());
        
        // Événement de fin de rotation
        onRotationComplete?.Invoke();
        
        // Arrête les effets
        if (engineParticles != null)
            engineParticles.Stop();
        
        if (engineSound != null)
        {
            // Fade out du son
            float startVolume = engineSound.volume;
            float fadeTime = 1f;
            float elapsed = 0f;
            
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                engineSound.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
                yield return null;
            }
            
            engineSound.Stop();
            engineSound.volume = startVolume;
        }
        
        isMoving = false;
        hasCompleted = true;
        
        Debug.Log("Shuttle intro sequence completed!");
    }

    IEnumerator MoveAlongZ()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position;
        targetPosition.z = endZ;
        
        // Calcule la distance et le temps nécessaire
        float distance = Mathf.Abs(endZ - startZ);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        Debug.Log($"Moving shuttle from Z={startZ} to Z={endZ} over {duration:F2} seconds");
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Mouvement avec ease-in-out pour plus de fluidité
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);
            
            yield return null;
        }
        
        // Assure qu'on est exactement à la position finale
        transform.position = targetPosition;
        
        Debug.Log($"Shuttle reached Z={endZ}");
    }

    IEnumerator RotateOnY()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, rotationAngle, 0f);
        
        float elapsed = 0f;
        
        Debug.Log($"Rotating shuttle {rotationAngle}° on Y axis over {rotationDuration:F2} seconds");
        
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;
            
            // Rotation avec ease-in-out
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, smoothT);
            
            yield return null;
        }
        
        // Assure qu'on est exactement à la rotation finale
        transform.rotation = targetRotation;
        
        Debug.Log($"Rotation complete! New Y rotation: {transform.rotation.eulerAngles.y:F2}°");
    }

    // Méthode pour redémarrer l'animation (utile pour les tests)
    public void RestartSequence()
    {
        if (isMoving)
        {
            StopAllCoroutines();
            isMoving = false;
        }
        
        hasCompleted = false;
        Start();
    }
    
    // Méthode pour arrêter l'animation
    public void StopSequence()
    {
        StopAllCoroutines();
        isMoving = false;
        
        if (engineParticles != null)
            engineParticles.Stop();
        
        if (engineSound != null)
            engineSound.Stop();
    }
}