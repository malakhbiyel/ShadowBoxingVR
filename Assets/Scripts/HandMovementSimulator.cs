using UnityEngine;

public class HandMovementSimulator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Vitesse de déplacement de la main")]
    public float moveSpeed = 15f;
    
    [Tooltip("Vitesse du coup de poing automatique")]
    public float punchSpeed = 20f;

    [Header("Contrôles")]
    [Tooltip("Touche pour coup de poing avant rapide")]
    public KeyCode punchKey = KeyCode.A;

    private Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool isPunching = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Mouvement manuel AMÉLIORÉ
        Vector3 movement = Vector3.zero;
        
        // W, S, A, D pour Haut, Bas, Gauche, Droite
        if (Input.GetKey(KeyCode.W)) movement.y += 1;      // Haut
        if (Input.GetKey(KeyCode.S)) movement.y -= 1;      // Bas
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;      // Gauche
        if (Input.GetKey(KeyCode.D)) movement.x += 1;      // Droite
        
        // Q, E pour Avant, Arrière
        if (Input.GetKey(KeyCode.Q)) movement.z += 1;      // Avant
        if (Input.GetKey(KeyCode.E)) movement.z -= 1;      // Arrière

        // Applique le mouvement
        transform.localPosition += movement * moveSpeed * Time.deltaTime;

        // Simulation coup de poing avec touche configurée
        if (Input.GetKeyDown(punchKey) && !isPunching)
        {
            StartCoroutine(SimulatePunch());
        }
    }

    System.Collections.IEnumerator SimulatePunch()
    {
        isPunching = true;
        Vector3 startPos = transform.localPosition;
        Vector3 punchPos = startPos + Vector3.forward * 0.5f;

        // Mouvement rapide vers l'avant
        float elapsed = 0f;
        float duration = 0.1f;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, punchPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Retour à la position initiale
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(punchPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
        isPunching = false;
    }
}
