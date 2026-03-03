using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject snowballPrefab;
    public Transform snowballLocation;
    public float throwForce = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Throw()
    {
        Vector3 spawnPosition = snowballLocation.position;

        GameObject snowball = Instantiate(
            snowballPrefab,
            spawnPosition,
            snowballLocation.rotation
        );

        Rigidbody rb = snowball.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }
}
