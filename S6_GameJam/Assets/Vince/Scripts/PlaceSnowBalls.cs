using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class PlaceSnowBall : MonoBehaviour
{
    [Header("CHANGE SNOWWALL PREFAB")]
    public GameObject spawnSnowBall;
    public Transform spawnSnow;
    public int amount = 10;
    private bool canSpawn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnSnowBalls()
    {

        Debug.Log("Spawning Snow Wall");
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            Instantiate(spawnSnowBall, spawnSnow.position + randomOffset, spawnSnow.rotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("snowblocks"))
        {

            Destroy(collision.gameObject);

            SpawnSnowBalls();

        }

    }

}
