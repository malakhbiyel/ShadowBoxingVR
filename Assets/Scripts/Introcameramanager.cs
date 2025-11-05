using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroCameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("Caméra qui filme d'en haut pendant le mouvement du shuttle")]
    public Camera overheadCamera;
    
    [Tooltip("XR Origin (caméra VR) qui prend le relais après")]
    public GameObject xrOrigin;
    
    [Header("Shuttle Reference")]
    [Tooltip("Le shuttle à surveiller")]
    public ShuttleIntroMovement shuttleScript;
    
    [Header("Transition Settings")]
    [Tooltip("Délai après l'arrêt du shuttle avant de changer de caméra")]
    public float delayBeforeSwitch = 1f;
    
    [Tooltip("Durée du fondu noir entre les caméras")]
    public float fadeDuration = 1f;
    
    [Header("Player Positioning")]
    [Tooltip("Position où placer le joueur VR après le switch")]
    public Transform playerStartPosition;
    
    private CanvasGroup fadePanel;
    private bool hasSwitched = false;

    void Start()
    {
        // Configure les caméras au démarrage
        SetupCameras();
        
        // Crée le panel de fondu
        CreateFadePanel();
        
        // S'abonne à l'événement de rotation du shuttle
        if (shuttleScript != null)
        {
            shuttleScript.onRotationComplete.AddListener(OnShuttleRotationComplete);
            Debug.Log("Subscribed to shuttle rotation complete event!");
        }
        else
        {
            Debug.LogError("ShuttleScript not assigned! Camera won't switch automatically.");
        }
    }

    void SetupCameras()
    {
        // Désactive TOUTES les caméras d'abord
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            cam.enabled = false;
        }
        
        // Active UNIQUEMENT la caméra overhead
        if (overheadCamera != null)
        {
            overheadCamera.enabled = true;
            Debug.Log("OverheadCamera activated!");
        }
        else
        {
            Debug.LogError("OverheadCamera is not assigned!");
        }
        
        // Désactive la VR complètement
        if (xrOrigin != null)
        {
            xrOrigin.SetActive(false);
            Debug.Log("XR Origin deactivated!");
        }
        else
        {
            Debug.LogError("XR Origin is not assigned!");
        }
    }

    void CreateFadePanel()
    {
        // Crée un Canvas pour le fondu noir
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvasObj.transform);
        
        Image image = panelObj.AddComponent<Image>();
        image.color = Color.black;
        
        RectTransform rect = panelObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        fadePanel = panelObj.AddComponent<CanvasGroup>();
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
        
        DontDestroyOnLoad(canvasObj);
    }

    // Appelé automatiquement quand le shuttle finit sa rotation
    void OnShuttleRotationComplete()
    {
        Debug.Log("Shuttle rotation complete! Starting camera switch...");
        StartCoroutine(WaitAndSwitch());
    }

    IEnumerator WaitAndSwitch()
    {
        // Attend le délai avant de switcher
        yield return new WaitForSeconds(delayBeforeSwitch);
        
        // Change de caméra
        yield return StartCoroutine(SwitchToVRCamera());
    }

    IEnumerator SwitchToVRCamera()
    {
        if (hasSwitched)
            yield break;
        
        hasSwitched = true;
        
        Debug.Log("Switching from overhead camera to VR camera...");
        
        // Fondu vers le noir
        yield return StartCoroutine(FadeToBlack());
        
        // Désactive la caméra overhead
        if (overheadCamera != null)
            overheadCamera.enabled = false;
        
        // Active et positionne la VR
        if (xrOrigin != null)
        {
            xrOrigin.SetActive(true);
            
            // Place le joueur à la position de départ
            if (playerStartPosition != null)
            {
                xrOrigin.transform.position = playerStartPosition.position;
                xrOrigin.transform.rotation = playerStartPosition.rotation;
            }
        }
        
        // Fondu depuis le noir
        yield return StartCoroutine(FadeFromBlack());
        
        Debug.Log("Camera switch complete! VR mode active.");
    }

    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        
        fadePanel.alpha = 1f;
    }

    IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        
        fadePanel.alpha = 0f;
    }
    
    // Méthode publique pour forcer le switch (utile pour les tests)
    public void ForceSwitchToVR()
    {
        if (!hasSwitched)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchToVRCamera());
        }
    }
}




