using System.Collections;
using UnityEngine;

public class PlacementSensor : MonoBehaviour
{
    public Transform[] snapPoints;
    public Renderer[] snapPointRenderers;
    public Material previewMaterial;

    bool[] occupied;
    Material[] originalMaterials;
    Material[][] originalMaterialsArray;

    void Awake()
    {
        GameObject[] piles = GameObject.FindGameObjectsWithTag("SnowPileSensor");
        snapPoints = new Transform[piles.Length];
        for (int i = 0; i < piles.Length; i++)
            snapPoints[i] = piles[i].transform;

        occupied = new bool[snapPoints.Length];

        if (snapPointRenderers == null || snapPointRenderers.Length != snapPoints.Length)
            snapPointRenderers = new Renderer[snapPoints.Length];

        for (int i = 0; i < snapPoints.Length; i++)
        {
            if (snapPoints[i] != null && snapPointRenderers[i] == null)
                snapPointRenderers[i] = snapPoints[i].GetComponentInChildren<Renderer>();
        }

        originalMaterialsArray = new Material[snapPointRenderers.Length][];
        originalMaterials = new Material[snapPointRenderers.Length];
        for (int i = 0; i < snapPointRenderers.Length; i++)
        {
            if (snapPointRenderers[i] != null)
            {
                originalMaterialsArray[i] = snapPointRenderers[i].sharedMaterials;
                originalMaterials[i] = originalMaterialsArray[i].Length > 0 ? originalMaterialsArray[i][0] : null;
            }
            else
            {
                originalMaterialsArray[i] = new Material[0];
                originalMaterials[i] = null;
            }
        }
    }

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

    public void SnapCubeToPoint(SnowCube cube, int index, float magnetSeconds = 0f)
    {
        if (index < 0 || index >= snapPoints.Length || occupied[index]) return;
        Transform snapT = snapPoints[index];
        occupied[index] = true;
        cube.rb.linearVelocity = Vector3.zero;
        cube.rb.angularVelocity = Vector3.zero;

        ClearPreview();

        if (magnetSeconds <= 0f)
        {
            cube.transform.SetParent(snapT, true); 
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = Vector3.one;

            cube.rb.isKinematic = true; 
            cube.currentSnapIndex = index;
            cube.currentSensor = null;
        }
        else
        {
            cube.rb.isKinematic = false; 
            StartCoroutine(MagnetLerpAndParent(cube, snapT, index, magnetSeconds));
        }
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

        cube.transform.position = snapT.position;
        cube.transform.rotation = snapT.rotation;

        cube.rb.linearVelocity = Vector3.zero;
        cube.rb.angularVelocity = Vector3.zero;

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
                snapPointRenderers[i].sharedMaterials = originalMaterialsArray[i];
            }
            else
            {
                if (i == nearest)
                {
                    snapPointRenderers[i].sharedMaterials = new Material[] { previewMaterial };
                }
                else
                {
                    snapPointRenderers[i].sharedMaterials = originalMaterialsArray[i];
                }
            }
        }
    }
    public void ShowAllPreviews()
    {
        if (previewMaterial == null || snapPointRenderers == null || originalMaterialsArray == null) return;

        for (int i = 0; i < snapPointRenderers.Length; i++)
        {
            var r = snapPointRenderers[i];
            if (r == null)
            {
                if (i < originalMaterialsArray.Length)
                    r.sharedMaterials = originalMaterialsArray[i];
                continue;
            }

            if (i < occupied.Length && occupied[i])
            {
                r.sharedMaterials = originalMaterialsArray[i];
            }
            else
            {
                r.sharedMaterials = new Material[] { previewMaterial };
            }
        }
    }
    public void ClearPreview()
    {
        if (snapPointRenderers == null || originalMaterialsArray == null) return;
        for (int i = 0; i < snapPointRenderers.Length; i++)
        {
            if (snapPointRenderers[i] == null) continue;
            snapPointRenderers[i].sharedMaterials = originalMaterialsArray[i];
        }
    }
}