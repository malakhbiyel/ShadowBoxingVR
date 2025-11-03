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
    public float obstacleChance = 30f;
    
    [Header("Movement")]
    public float projectileSpeed = 3f;
    public float lifeTime = 5f;
    
    void Start()
    {
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
        
        GameObject prefab = spawnObstacle ? obstaclePrefab : targetPrefab;
        
        if (prefab == null)
        {
            Debug.LogWarning("⚠️ Prefab manquant !");
            return;
        }
        
        // Créer l'objet
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
        
        // Ajouter le script Projectile
        Projectile proj = obj.AddComponent<Projectile>();
        proj.speed = projectileSpeed;
        proj.lifeTime = lifeTime;
        proj.isObstacle = spawnObstacle;
        
        Debug.Log($"✨ Spawné: {(spawnObstacle ? "OBSTACLE" : "CIBLE")}");
    }
}
