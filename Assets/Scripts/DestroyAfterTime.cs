using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float delay = 1f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
