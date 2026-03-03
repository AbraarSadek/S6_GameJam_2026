using UnityEngine;

public class Shovel : MonoBehaviour
{
    public GameObject snowBlockPrefab;
    public Transform spawnPoint;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnSnowBlock()
    {
        GameObject snowBlock = Instantiate(snowBlockPrefab, spawnPoint.position, spawnPoint.rotation);
            
    }

    void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.CompareTag("snowpile"))
        {
            SpawnSnowBlock();
        }
    }

}
