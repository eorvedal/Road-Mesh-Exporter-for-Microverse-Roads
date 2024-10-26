using UnityEngine;
using System.Collections;
using JBooth.MicroVerseCore;
using UnityEditor;
using System;

public class RoadMeshExporter : MonoBehaviour
{
    public string exportPath = "Assets/RoadMeshes/";
    public LayerMask terrainLayerMask; 
    public float raycastDistance = 10f; 
    public bool ExportRoads = true;
    public bool ExportIntersections = true;
    [ContextMenu("Begin exporting Microverse road object meshes")]
    public void BeginExporting()
    {
        StartCoroutine(ExportRoadMeshesAsync());
    }

    IEnumerator ExportRoadMeshesAsync()
    {

        float totalObjects = 0;
        float processedObjects = 0;
        RaycastHit hit;
        Road[] roadObjects = GetComponentsInChildren<Road>();
        Intersection[] intersectionObjects = GetComponentsInChildren<Intersection>();

        if (ExportRoads)
        {
            totalObjects += roadObjects.Length;
        }
        if (ExportIntersections)
        { 
            totalObjects += intersectionObjects.Length;
        }
        if (ExportRoads)
        {
            int n = 0;
            foreach (Road roadObject in roadObjects)
            {
                n++;
                SavedRoadAsset savedFlag = roadObject.GetComponent<SavedRoadAsset>();
                if (savedFlag == null)
                {
                    MeshFilter[] originalMeshFilters = roadObject.GetComponentsInChildren<MeshFilter>();
                    int i = 0;
                    foreach (MeshFilter MF in originalMeshFilters)
                    {
                        Ray ray = new Ray(MF.transform.position + Vector3.up * raycastDistance, Vector3.down);
                        Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask);
                        Mesh originalMesh = MF.sharedMesh;
                        string meshPath = exportPath + "Road_" + hit.transform.gameObject.name + "_" + roadObject.name + "_" + n + "_" + "_" + i + "_" + "_mesh.asset";
                        try
                        {
                            AssetDatabase.CreateAsset(originalMesh, meshPath);
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"error : {e.Message}");
                        }
                        GameObject copiedObject = new GameObject("Road_" + hit.transform.gameObject.name + "_" + roadObject.name + "_" + n + "_" + "_" + i);
                        i++;
                        copiedObject.transform.position = MF.transform.position;
                        copiedObject.transform.rotation = MF.transform.rotation;
                        copiedObject.transform.localScale = MF.transform.localScale;
                        copiedObject.transform.parent = hit.transform;

                        Component[] scripts = MF.GetComponents<Component>();
                        foreach (Component script in scripts)
                        {
                            copiedObject.CopyComponent<Component>(script);
                        }
                        MeshFilter newMeshFilter = copiedObject.GetComponent<MeshFilter>();
                        if (newMeshFilter)
                            newMeshFilter.sharedMesh = MF.GetComponent<MeshFilter>().sharedMesh;
                        MeshRenderer renderer = copiedObject.GetComponent<MeshRenderer>();
                        if (renderer)
                        {
                            Material newMat = new Material(MF.GetComponent<MeshRenderer>().sharedMaterial);
                            renderer.sharedMaterial = newMat;
                        }
                        MeshCollider newMeshCollider = copiedObject.GetComponent<MeshCollider>();
                        if (newMeshCollider)
                            newMeshCollider.sharedMesh = MF.GetComponent<MeshCollider>().sharedMesh;
                    }
                    roadObject.gameObject.AddComponent<SavedRoadAsset>();
                    processedObjects++;
                    EditorUtility.DisplayProgressBar("Exporting Road Meshes", "Processing Objects...", processedObjects / totalObjects);

                    yield return null;
                }
            }
        }
        if (ExportIntersections)
        {
            int n = 0;
            foreach (Intersection intersectionObject in intersectionObjects)
            {
                n++;
                SavedRoadAsset savedFlag = intersectionObject.GetComponent<SavedRoadAsset>();
                if (savedFlag == null)
                {
                    MeshFilter[] originalMeshFilters = intersectionObject.GetComponentsInChildren<MeshFilter>();
                    int i = 0;
                    foreach (MeshFilter MF in originalMeshFilters)
                    {
                        Ray ray = new Ray(MF.transform.position + Vector3.up * raycastDistance, Vector3.down);
                        Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask);
                        Mesh originalMesh = MF.sharedMesh;
                        string meshPath = exportPath + "Intersection_" + hit.transform.gameObject.name + "_" + intersectionObject.name + "_" + n + "_" + "_" + i + "_" + "_mesh.asset";
                        try
                        {
                            AssetDatabase.CreateAsset(originalMesh, meshPath);
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"error : {e.Message}");
                        }
                        GameObject copiedObject = new GameObject("Intersection_" + hit.transform.gameObject.name + "_" + intersectionObject.name + "_" + n + "_" + "_" + i);
                        i++;
                        copiedObject.transform.position = MF.transform.position;
                        copiedObject.transform.rotation = MF.transform.rotation;
                        copiedObject.transform.localScale = intersectionObject.transform.localScale; //very important - meshes aren't scaled, the intersection parent is.
                        copiedObject.transform.parent = hit.transform;

                        Component[] scripts = MF.GetComponents<Component>();
                        foreach (Component script in scripts)
                        {
                            copiedObject.CopyComponent<Component>(script);
                        }
                        MeshFilter newMeshFilter = copiedObject.GetComponent<MeshFilter>();
                        if (newMeshFilter)
                            newMeshFilter.sharedMesh = MF.GetComponent<MeshFilter>().sharedMesh;
                        MeshRenderer renderer = copiedObject.GetComponent<MeshRenderer>();
                        if (renderer)
                        {
                            Material newMat = new Material(MF.GetComponent<MeshRenderer>().sharedMaterial);
                            renderer.sharedMaterial = newMat;
                        }
                        MeshCollider newMeshCollider = copiedObject.GetComponent<MeshCollider>();
                        if (newMeshCollider)
                            newMeshCollider.sharedMesh = MF.GetComponent<MeshCollider>().sharedMesh;
                    }
                    intersectionObject.gameObject.AddComponent<SavedRoadAsset>();
                    processedObjects++;
                    EditorUtility.DisplayProgressBar("Exporting Road Meshes", "Processing Objects...", processedObjects / totalObjects);

                    yield return null;
                }
            }
        }
        EditorUtility.ClearProgressBar();
        this.gameObject.SetActive(false);
    }
}
    
