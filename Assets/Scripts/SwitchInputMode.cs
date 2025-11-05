using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SwitchInputMode : MonoBehaviour
{
    public GameObject xrDeviceSimulator; // Glisse ton simulateur ici
    
    void Start()
    {
        // Détecte si un casque VR est connecté
        bool isVRConnected = UnityEngine.XR.XRSettings.isDeviceActive;
        
        if (isVRConnected)
        {
            // Casque détecté → Désactive le simulateur
            if (xrDeviceSimulator != null)
                xrDeviceSimulator.SetActive(false);
            
            Debug.Log("✅ Casque VR détecté, simulateur désactivé");
        }
        else
        {
            // Pas de casque → Active le simulateur
            if (xrDeviceSimulator != null)
                xrDeviceSimulator.SetActive(true);
            
            Debug.Log("🖥️ Pas de casque VR, simulateur activé");
        }
    }
}
