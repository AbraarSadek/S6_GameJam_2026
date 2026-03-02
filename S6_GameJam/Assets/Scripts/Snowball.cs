using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class Snowball : MonoBehaviour
{
    public GameObject impactEffect;
    public float minImpactVelocity = 2.5f;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (grabInteractable != null && grabInteractable.isSelected)
            return;
        if (rb.velocity.magnitude >= minImpactVelocity)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(gameObject);
        }
    }
}