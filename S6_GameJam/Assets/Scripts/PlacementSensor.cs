using System.Collections;
using UnityEngine;

public class PlacementSensor : MonoBehaviour
{
    [Tooltip("Transforms representing exact snap positions (place cubes here)")]
    public Transform[] snapPoints;

    [Tooltip("Renderer / visual on each snapPoint (optional). Assign if you want previews via material swap.")]
    public Renderer[] snapPointRenderers; // optional: use for preview highlight

    [Tooltip("Material used to show a preview/highlight on a free snap point.")]
    public Material previewMaterial;

    bool[] occupied;
    Material[] originalMaterials;

    void Awake()
    {
        if (snapPoints == null) snapPoints = new Transform[0];
        occupied = new bool[snapPoints.Length];

        // Setup renderers array to match count (if left empty, try to find in snapPoints)
        if (snapPointRenderers == null || snapPointRenderers.Length != snapPoints.Length)
        {
            snapPointRenderers = new Renderer[snapPoints.Length];
            for (int i = 0; i < snapPoints.Length; i++)
            {
                if (snapPoints[i] != null)
                    snapPointRenderers[i] = snapPoints[i].GetComponentInChildren<Renderer>();
            }
        }

        originalMaterials = new Material[snapPointRenderers.Length];
        for (int i = 0; i < snapPointRenderers.Length; i++)
            if (snapPointRenderers[i] != null)
                originalMaterials[i] = snapPointRenderers[i].sharedMaterial;
    }

    /// <summary>
    /// Returns nearest free snap index, or -1 if none free.
    /// </summary>
    public int GetNearestFreeSnapIndex(Vector3 position)
    {
        int best = -1;
        float bestDist = float.MaxValue;
        for (int i = 0; i < snapPoints.Length; i++)
        {
            if (occupied[i]) continue;
            float d = (snapPoints[i].position - position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    /// <summary>
    /// Snap the cube to the snap point (with optional magnet lerp).
    /// NOTE: We do NOT parent immediately if we are lerping; we parent at the end
    /// and force localScale = Vector3.one so child scale becomes 1,1,1 (important).
    /// We also CLEAR the preview so the sensor visuals don't stick.
    /// </summary>
    public void SnapCubeToPoint(SnowCube cube, int index, float magnetSeconds = 0f)
    {
        if (index < 0 || index >= snapPoints.Length || occupied[index]) return;
        Transform snapT = snapPoints[index];

        // mark occupied immediately
        occupied[index] = true;

        // clear velocities and optionally lerp into position for magnet effect
        cube.rb.velocity = Vector3.zero;
        cube.rb.angularVelocity = Vector3.zero;

        // Clear preview right away so visuals don't stick (important when the cube remains
        // inside the sensor afterwards because the snap points are children of the sensor)
        ClearPreview();

        if (magnetSeconds <= 0f)
        {
            // place instantly and parent
            cube.transform.SetParent(snapT, true); // parent keeping world position (we'll fix local transform next)
            // force exact local alignment and scale 1,1,1 to avoid inherited scale issues
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = Vector3.one;

            cube.rb.isKinematic = true; // freeze in place
            cube.currentSnapIndex = index;
            cube.currentSensor = null;
        }
        else
        {
            // start lerp in world space then parent at end
            cube.rb.isKinematic = false; // let coroutine move transform
            StartCoroutine(MagnetLerpAndParent(cube, snapT, index, magnetSeconds));
        }

        // restore snapPoint renderer material for this slot
        if (snapPointRenderers != null && index < snapPointRenderers.Length && snapPointRenderers[index] != null)
        {
            if (originalMaterials[index] != null)
                snapPointRenderers[index].material = originalMaterials[index];
        }
    }

    IEnumerator MagnetLerpAndParent(SnowCube cube, Transform snapT, int index, float t)
    {
        float elapsed = 0f;
        Vector3 startPos = cube.transform.position;
        Quaternion startRot = cube.transform.rotation;
        while (elapsed < t)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / t));
            cube.transform.position = Vector3.Lerp(startPos, snapT.position, a);
            cube.transform.rotation = Quaternion.Slerp(startRot, snapT.rotation, a);
            yield return null;
        }

        // final position
        cube.transform.position = snapT.position;
        cube.transform.rotation = snapT.rotation;

        cube.rb.velocity = Vector3.zero;
        cube.rb.angularVelocity = Vector3.zero;

        // now parent and fix local transform/scale so it becomes (0,0,0) with localScale 1,1,1
        cube.transform.SetParent(snapT, true);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = Vector3.one;

        cube.rb.isKinematic = true;
        cube.currentSnapIndex = index;
        cube.currentSensor = null;
    }

    public void FreeSnap(int index)
    {
        if (index < 0 || index >= occupied.Length) return;
        occupied[index] = false;

        if (snapPointRenderers != null && index < snapPointRenderers.Length && snapPointRenderers[index] != null)
            if (originalMaterials[index] != null)
                snapPointRenderers[index].material = originalMaterials[index];
    }

    public void ShowPreviewAt(Vector3 position)
    {
        if (previewMaterial == null) return;
        int nearest = GetNearestFreeSnapIndex(position);

        // if no valid snap, restore all originals
        if (nearest == -1)
        {
            ClearPreview();
            return;
        }

        for (int i = 0; i < snapPointRenderers.Length; i++)
        {
            if (snapPointRenderers[i] == null) continue;
            if (occupied[i])
            {
                if (originalMaterials[i] != null) snapPointRenderers[i].material = originalMaterials[i];
            }
            else
            {
                snapPointRenderers[i].material = (i == nearest) ? previewMaterial : originalMaterials[i];
            }
        }
    }

    public void ClearPreview()
    {
        if (snapPointRenderers == null || originalMaterials == null) return;
        for (int i = 0; i < snapPointRenderers.Length; i++)
        {
            if (snapPointRenderers[i] != null)
            {
                // restore original if we have it, otherwise try to set to sharedMaterial to be safe
                if (originalMaterials.Length > i && originalMaterials[i] != null)
                    snapPointRenderers[i].material = originalMaterials[i];
                else
                    snapPointRenderers[i].material = snapPointRenderers[i].sharedMaterial;
            }
        }
    }
}