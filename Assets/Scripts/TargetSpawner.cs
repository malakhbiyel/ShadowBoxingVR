using UnityEngine;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;
    public GameObject hologranPrefab; // 🆕 Nouveau type

    [Header("Spawn Settings")]
    [Tooltip("Temps entre chaque spawn")]
    public float spawnInterval = 2f;

    [Tooltip("% chance d'obstacle (0-100)")]
    [Range(0, 100)]
    public float obstacleChance = 30f;

    [Tooltip("% chance de hologran (0-100)")]
    [Range(0, 100)]
    public float hologranChance = 20f; // 🆕 Nouveau paramètre

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
        float random = Random.Range(0f, 100f);
        GameObject prefabToSpawn;

        // 🧮 Distribution des chances :
        if (random < obstacleChance)
        {
            prefabToSpawn = obstaclePrefab;
        }
        else if (random < obstacleChance + hologranChance)
        {
            prefabToSpawn = hologranPrefab;
        }
        else
        {
            prefabToSpawn = targetPrefab;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("⚠️ Prefab manquant !");
            return;
        }

        // Instanciation
        GameObject obj = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

        // Script Projectile
        Projectile proj = obj.AddComponent<Projectile>();
        proj.speed = projectileSpeed;
        proj.lifeTime = lifeTime;

        // Type logique
        proj.isObstacle = (prefabToSpawn == obstaclePrefab);
        proj.isHologran = (prefabToSpawn == hologranPrefab); // 🆕

        Debug.Log($"✨ Spawné: {(proj.isObstacle ? "OBSTACLE" : proj.isHologran ? "HOLOGRAN" : "CIBLE")}");
    }
}
