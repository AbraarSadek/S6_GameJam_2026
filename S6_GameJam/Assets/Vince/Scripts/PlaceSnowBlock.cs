using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class PlaceSnowBlock : MonoBehaviour
{
    [Header("CHANGE SNOWWALL PREFAB")]
    public GameObject spawnSnowWall;
    public Transform spawnSnow;
    public int amount = 1;
    private bool canSpawn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnSnowWalls()
    {
        
        Debug.Log("Spawning Snow Wall");
        for(int i = 0; i < amount; i++) {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f),0,Random.Range(-1f, 1f));

            GameObject snowBlock = Instantiate(spawnSnowWall, spawnSnow.position + randomOffset, spawnSnow.rotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("snowblocks"))
        {
          
            Destroy(collision.gameObject);

            SpawnSnowWalls();

        }
        
    }

}
