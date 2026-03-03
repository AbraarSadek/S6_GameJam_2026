using UnityEngine;

public class MobDeath : MonoBehaviour
{
    [HideInInspector] public WaveManager manager;

    // Call this when the mob dies (health hits 0)
    public void Die()
    {
        if (manager != null)
            manager.MobDied();

        Destroy(gameObject);
    }
}