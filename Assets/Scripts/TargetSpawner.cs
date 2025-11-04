using UnityEngine;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("Temps entre chaque spawn")]
    public float spawnInterval = 2f;
    
    [Tooltip("% chance d'obstacle (0-100)")]
    [Range(0, 100)]
    public float obstacleChance = 100f;
    
    [Header("Movement")]
    public float projectileSpeed = 3f;
    public float lifeTime = 5f;
    
    void Start()
    {
        // Vérification initiale des prefabs
        Debug.Log("=== VERIFICATION PREFABS ===");
        Debug.Log($"Target Prefab: {(targetPrefab != null ? targetPrefab.name : "NULL")}");
        Debug.Log($"Obstacle Prefab: {(obstaclePrefab != null ? obstaclePrefab.name : "NULL")}");
        Debug.Log($"Spawner Position: {transform.position}");
        Debug.Log("============================");
        
        StartCoroutine(SpawnRoutine());
    }
    
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnProjectile();
        }
    }
    
    void SpawnProjectile()
    {
        // Décide obstacle ou cible
        float random = Random.Range(0f, 100f);
        bool spawnObstacle = (random < obstacleChance);
        
        Debug.Log($"\n[SPAWN] Random: {random:F1} | Seuil: {obstacleChance} | Type: {(spawnObstacle ? "OBSTACLE" : "CIBLE")}");
        
        GameObject prefab = spawnObstacle ? obstaclePrefab : targetPrefab;
        
        if (prefab == null)
        {
            Debug.LogError($"[ERROR] Prefab {(spawnObstacle ? "OBSTACLE" : "TARGET")} est NULL!");
            return;
        }
        
        Debug.Log($"[OK] Prefab found: {prefab.name}");
        
        // Créer l'objet
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
        
        // === DEBUG DÉTAILLÉ ===
        Debug.Log($"[CREATED] Object: {obj.name}");
        Debug.Log($"   Position: {obj.transform.position}");
        Debug.Log($"   Active: {obj.activeSelf}");
        Debug.Log($"   Children: {obj.transform.childCount}");
        
        // Vérifier les composants
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Collider col = obj.GetComponent<Collider>();
        Debug.Log($"   Rigidbody: {(rb != null ? "YES" : "NO")}");
        Debug.Log($"   Collider: {(col != null ? col.GetType().Name : "NO")}");
        
        // Vérifier le child (pour l'obstacle)
        if (obj.transform.childCount > 0)
        {
            Transform child = obj.transform.GetChild(0);
            Debug.Log($"   Child name: {child.name}");
            Debug.Log($"      Active: {child.gameObject.activeSelf}");
            Debug.Log($"      Local Position: {child.localPosition}");
            Debug.Log($"      Local Scale: {child.localScale}");
            
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Debug.Log($"      MeshRenderer: YES (enabled: {mr.enabled})");
                if (mr.material != null)
                {
                    Debug.Log($"      Material: {mr.material.name}");
                    Debug.Log($"      Shader: {mr.material.shader.name}");
                    Color color = mr.material.HasProperty("_Color") ? mr.material.color : Color.white;
                    Debug.Log($"      Color: {color}");
                }
                else
                {
                    Debug.LogError("      [ERROR] Material is NULL!");
                }
            }
            else
            {
                Debug.LogError("      [ERROR] No MeshRenderer on child!");
            }
            
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Debug.Log($"      Mesh: {mf.sharedMesh.name} ({mf.sharedMesh.vertexCount} vertices)");
            }
            else
            {
                Debug.LogError("      [ERROR] No Mesh or MeshFilter!");
            }
        }
        
        // Ajouter le script Projectile
        Projectile proj = obj.AddComponent<Projectile>();
        proj.speed = projectileSpeed;
        proj.lifeTime = lifeTime;
        proj.isObstacle = spawnObstacle;
        
        Debug.Log($"[CONFIGURED] Projectile: speed={projectileSpeed}, lifetime={lifeTime}, isObstacle={spawnObstacle}");
        Debug.Log("==================\n");
    }
    
    // Visual helper to see spawn position in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}