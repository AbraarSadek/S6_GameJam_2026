using System;
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
    public float throwBoost = 3.5f;
    public bool inHand = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }


    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnHold);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnHold);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnHold(SelectEnterEventArgs args)
    {
        inHand = true;
    }
    void OnRelease(SelectExitEventArgs args)
    {
        rb.linearVelocity *= throwBoost;
        inHand = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (inHand)
        {
            return;
        }
        
        Health otherHealth = collision.gameObject.GetComponent<Health>();
        if (otherHealth != null)
        {
            otherHealth.TakeDamage(1f);
        }
        
        if (rb.linearVelocity.magnitude >= minImpactVelocity)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(gameObject);
        }
    }
}