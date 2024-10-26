//using UnityEngine;
//using System.Collections.Generic;
//using JBooth.MicroVerseCore;
//using UnityEditor;

//public class RoadMeshExporter : MonoBehaviour
//{
//    public string exportPath = "Assets/RoadMeshes/";
//    public LayerMask terrainLayerMask; // Public variable to define the layer mask

//    // Adjust this value as per your needs
//    public float raycastDistance = 10f; // Distance above the object to cast the ray

//    [ContextMenu("Begin exporting Microverse road object meshes")]
//    public void BeginExporting()
//    {
//        // Iterate over each terrain
//        Terrain[] terrains = gameObject.GetComponentsInChildren<Terrain>();
//        foreach (Terrain terrain in terrains)
//        {
//            ExportRoadMeshes(terrain);
//        }
//    }

//    void ExportRoadMeshes(Terrain terrain)
//    {
//        // Create a new parent GameObject for this terrain
//        GameObject terrainParent = new GameObject("RoadMeshes_" + terrain.name);
//        terrainParent.transform.parent = terrain.transform;

//        // List to keep track of copied meshes for saving
//        //List<MeshFilter> copiedMeshFilters = new List<MeshFilter>();

//        RaycastHit hit;

//        //Iterate over all road objects in the scene
//        Road[] roadObjects = FindObjectsOfType<Road>(); // Implement RoadObject class according to your project
//        int n = 0;
//        foreach (Road roadObject in roadObjects)
//        {
//            // Check if roadObject is over the current terrain
//            if (IsOverTerrain(roadObject.gameObject, terrain, out hit))
//            {
//                // Create a copy of the roadObject's mesh
//                MeshFilter[] originalMeshFilters = roadObject.GetComponentsInChildren<MeshFilter>();
//                int i = 0;
//                foreach (MeshFilter MF in originalMeshFilters)
//                {
//                    Mesh originalMesh = MF.sharedMesh;
//                    GameObject copiedObject = new GameObject(terrain.name + "_" + roadObject.name + "_"+n+"_"+"_" + i + "_" + "_mesh");
//                    string filePath = exportPath + copiedObject.name + ".asset";
//                    copiedObject.transform.position = roadObject.transform.position;
//                    copiedObject.transform.rotation = roadObject.transform.rotation;
//                    copiedObject.transform.localScale = roadObject.transform.localScale;
//                    copiedObject.transform.parent = terrainParent.transform;

//                    MeshFilter copiedMeshFilter = copiedObject.AddComponent<MeshFilter>();
//                    copiedMeshFilter.sharedMesh = Instantiate(originalMesh);
//                    AssetDatabase.CreateAsset(originalMesh, filePath);

//                    // Save the copied mesh filter for later saving
//                    //copiedMeshFilters.Add(copiedMeshFilter);
//                }
//            }
//        }

//        Intersection[] intersectionObjects = FindObjectsOfType<Intersection>(); // Implement RoadObject class according to your project
//        foreach (Intersection intersectionObject in intersectionObjects)
//        {
//            // Check if roadObject is over the current terrain
//            int i = 0;
//            if (IsOverTerrain(intersectionObject.gameObject, terrain, out hit))
//            {
//                // Create a copy of the roadObject's mesh
//                MeshFilter originalMeshFilter = intersectionObject.GetComponentInChildren<MeshFilter>();
//                Mesh originalMesh = originalMeshFilter.sharedMesh;
//                GameObject copiedObject = new GameObject(terrain.name + "_" + intersectionObject.name + "_" + n + "_" + "_" + i + "_" + "_mesh");
//                string filePath = exportPath + copiedObject.name + ".asset";
//                copiedObject.transform.position = intersectionObject.transform.position;
//                copiedObject.transform.rotation = intersectionObject.transform.rotation;
//                copiedObject.transform.localScale = intersectionObject.transform.localScale;
//                copiedObject.transform.parent = terrainParent.transform;

//                MeshFilter copiedMeshFilter = copiedObject.AddComponent<MeshFilter>();
//                copiedMeshFilter.sharedMesh = Instantiate(originalMesh);
//                AssetDatabase.CreateAsset(originalMesh, filePath);

//                // Save the copied mesh filter for later saving
//                //copiedMeshFilters.Add(copiedMeshFilter);
//            }
//        }


//        // Save all copied meshes to files
//        //foreach (MeshFilter meshFilter in copiedMeshFilters)
//        //{
//        //    Mesh mesh = meshFilter.sharedMesh;
//        //    string filePath = exportPath + mesh.name + ".asset";
//        //    AssetDatabase.CreateAsset(mesh, filePath);
//        //}
//    }

//    bool IsOverTerrain(GameObject obj, Terrain terrain, out RaycastHit hit)
//    {
//        Ray ray = new Ray(obj.transform.position + Vector3.up * raycastDistance, Vector3.down);
//        return Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask) && hit.collider.GetComponent<Terrain>() == terrain;
//    }
//}
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
    
