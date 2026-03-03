using UnityEngine;

public class SnowPile : MonoBehaviour
{
    int collisionCount = 0;
    [SerializeField]
    public int maxCount = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("shovel"))
        {
            collisionCount++;
            if (collisionCount == maxCount)
            {
                Destroy(gameObject);
            }
        }
    }
}
