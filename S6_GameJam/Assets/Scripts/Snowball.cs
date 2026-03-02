using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class Snowball : MonoBehaviour
{
    public GameObject impactEffect;
    public float minImpactVelocity = 0.5f;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    public float throwBoost = 1.5f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }


    void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        rb.velocity *= throwBoost;
    }

    void OnCollisionEnter(Collision collision)
    {
        /*if (grabInteractable != null && grabInteractable.isSelected)
            return;*/
        if (rb.linearVelocity.magnitude >= minImpactVelocity)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(gameObject);
        }
    }
}