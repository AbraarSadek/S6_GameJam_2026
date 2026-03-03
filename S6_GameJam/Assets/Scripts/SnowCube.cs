using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class SnowCube : MonoBehaviour
{
    [HideInInspector] public PlacementSensor currentSensor;   // assigned when inside a sensor trigger
    [HideInInspector] public int currentSnapIndex = -1;
    [HideInInspector] public Rigidbody rb;

    [Header("Freeze settings")]
    public float restVelocityThreshold = 0.05f;   // considered resting
    public float restCheckDelay = 0.1f;

    [Header("Magnet")]
    public float magnetTime = 0.12f; // how fast the snap lerp is (0 = instant)

    XRGrabInteractable grab;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
        grab.selectExited.AddListener(OnRelease);
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectExited.RemoveListener(OnRelease);
            grab.selectEntered.RemoveListener(OnGrab);
        }
    }

    void Update()
    {
        // When held and inside a sensor, update the preview to nearest free snap
        if (grab != null && grab.isSelected && currentSensor != null)
        {
            currentSensor.ShowPreviewAt(transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlacementSensor ps = other.GetComponent<PlacementSensor>();
        if (ps != null)
        {
            currentSensor = ps;
            Debug.Log($"SnowCube: Entered sensor {ps.name}");
            if (grab != null && grab.isSelected)
                currentSensor.ShowPreviewAt(transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlacementSensor ps = other.GetComponent<PlacementSensor>();
        if (ps != null && currentSensor == ps)
        {
            Debug.Log($"SnowCube: Exited sensor {ps.name} - clearing preview");
            currentSensor.ClearPreview();
            currentSensor = null;
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // If it was snapped, free that snap slot so player can pick it up again
        if (currentSnapIndex != -1 && currentSensor != null)
        {
            currentSensor.FreeSnap(currentSnapIndex);
            currentSnapIndex = -1;
        }

        // While held, make sure physics isn't locked by being kinematic
        rb.isKinematic = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("SnowCube: Released");

        // If inside a sensor and there's a free snap nearby, snap to it
        if (currentSensor != null)
        {
            int nearestFree = currentSensor.GetNearestFreeSnapIndex(transform.position);
            if (nearestFree != -1)
            {
                // Clear preview immediately (defensive)
                currentSensor.ClearPreview();

                // Snap with magnet LERP (PlacementSensor should handle parenting after lerp)
                currentSensor.SnapCubeToPoint(this, nearestFree, magnetTime);

                // ensure we don't hold onto a stale sensor reference
                currentSensor = null;
                return;
            }
        }

        // Not snapped: allow physics and freeze when it comes to rest
        StartCoroutine(FreezeWhenRestCoroutine());
    }

    IEnumerator FreezeWhenRestCoroutine()
    {
        // ensure physics active for a little while
        yield return new WaitForSeconds(restCheckDelay);

        // wait until very close to rest (use rb.velocity, not linearVelocity)
        while (rb != null && rb.linearVelocity.magnitude > restVelocityThreshold)
            yield return null;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // freeze into place (becomes static)
            rb.isKinematic = true;
            Debug.Log("SnowCube: Frozen at rest");
        }
    }
}