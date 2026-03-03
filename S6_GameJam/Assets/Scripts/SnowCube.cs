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
    public float magnetTime = 0f; 

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
        if (grab != null && grab.isSelected && currentSensor != null)
        {
            currentSensor.ShowAllPreviews();
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
                currentSensor.ShowAllPreviews();
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
        if (currentSnapIndex != -1 && currentSensor != null)
        {
            currentSensor.FreeSnap(currentSnapIndex);
            currentSnapIndex = -1;
        }

        rb.isKinematic = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("SnowCube: Released");
        if (currentSensor != null)
        {
            int nearestFree = currentSensor.GetNearestFreeSnapIndex(transform.position);
            if (nearestFree != -1)
            {
                currentSensor.ClearPreview();

                currentSensor.SnapCubeToPoint(this, nearestFree, magnetTime);

                currentSensor = null;
                return;
            }
        }

        StartCoroutine(FreezeWhenRestCoroutine());
    }

    IEnumerator FreezeWhenRestCoroutine()
    {
        yield return new WaitForSeconds(restCheckDelay);

        while (rb != null && rb.linearVelocity.magnitude > restVelocityThreshold)
            yield return null;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            Debug.Log("SnowCube: Frozen at rest");
        }
    }
}