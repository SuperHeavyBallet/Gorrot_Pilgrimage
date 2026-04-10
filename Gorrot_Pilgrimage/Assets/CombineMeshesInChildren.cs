using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CombineMeshesInChildren : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] bool combineOnStart = true;
    [SerializeField] bool disableChildrenAfterCombine = true;
    [SerializeField] bool destroyChildrenAfterCombine = false;
    [SerializeField] bool includeInactiveChildren = false;

    [Header("Debug")]
    [SerializeField] bool logDetails = true;

    MeshFilter parentMeshFilter;
    MeshRenderer parentMeshRenderer;

    void Start()
    {
        if (combineOnStart)
        {
            CombineChildren();
        }
    }

    [ContextMenu("Combine Children")]
    public void CombineChildren()
    {
        parentMeshFilter = GetComponent<MeshFilter>();
        parentMeshRenderer = GetComponent<MeshRenderer>();

        MeshFilter[] childMeshFilters = GetComponentsInChildren<MeshFilter>(includeInactiveChildren);
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        Material chosenMaterial = null;
        int meshCount = 0;
        int vertexCount = 0;

        foreach (MeshFilter childMeshFilter in childMeshFilters)
        {
            if (childMeshFilter == parentMeshFilter)
                continue;

            if (childMeshFilter.sharedMesh == null)
                continue;

            MeshRenderer childRenderer = childMeshFilter.GetComponent<MeshRenderer>();
            if (childRenderer == null || !childRenderer.enabled)
                continue;

            if (chosenMaterial == null && childRenderer.sharedMaterial != null)
            {
                chosenMaterial = childRenderer.sharedMaterial;
            }

            CombineInstance combine = new CombineInstance
            {
                mesh = childMeshFilter.sharedMesh,

                // Convert child mesh from world space into THIS object's local space
                transform = transform.worldToLocalMatrix * childMeshFilter.transform.localToWorldMatrix
            };

            combineInstances.Add(combine);

            meshCount++;
            vertexCount += childMeshFilter.sharedMesh.vertexCount;
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogWarning($"[{name}] No valid child meshes found.");
            return;
        }

        Mesh combinedMesh = new Mesh
        {
            name = name + "_Combined",
            indexFormat = IndexFormat.UInt32
        };

        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        combinedMesh.RecalculateBounds();
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateTangents();

        parentMeshFilter.sharedMesh = combinedMesh;

        if (chosenMaterial != null)
        {
            parentMeshRenderer.sharedMaterial = chosenMaterial;
        }

        parentMeshRenderer.enabled = true;

        if (logDetails)
        {
            Debug.Log(
                $"[{name}] Combined {meshCount} child meshes. " +
                $"Approx source vertices: {vertexCount}. " +
                $"Combined bounds center: {combinedMesh.bounds.center}, size: {combinedMesh.bounds.size}"
            );
        }

        foreach (MeshFilter childMeshFilter in childMeshFilters)
        {
            if (childMeshFilter == parentMeshFilter)
                continue;

            if (destroyChildrenAfterCombine)
            {
                Destroy(childMeshFilter.gameObject);
            }
            else if (disableChildrenAfterCombine)
            {
                childMeshFilter.gameObject.SetActive(false);
            }
        }
    }
}