using UnityEngine;

public class Snowball : MonoBehaviour
{
    public GameObject impactEffect;

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Instantiate(impactEffect, contact.point, Quaternion.identity);
        Destroy(gameObject);
    }
}