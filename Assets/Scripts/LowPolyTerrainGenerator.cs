using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LowPolyTerrainGenerator : MonoBehaviour
{
    public WorldManager worldManager;

    [System.Serializable]
    public class BiomePrefabs
    {
        public List<GameObject> assets = new List<GameObject>();
    }

    public int assetCount = 10;

    // Outdoors
    public BiomePrefabs beachPrefabs;
    public BiomePrefabs desertPrefabs;
    public BiomePrefabs forestPrefabs;
    public BiomePrefabs farmPrefabs;

    // Room
    public BiomePrefabs bedroomPrefabs;
    public BiomePrefabs kitchenPrefabs;
    public BiomePrefabs livingroomPrefabs;
    public BiomePrefabs bathroomPrefabs;

    // Mine
    public BiomePrefabs aboveGroundPrefabs;
    public BiomePrefabs underGroundPrefabs;


    public List<GameObject> placedObjects = new List<GameObject>();

    public enum BiomeType
    {
        Beach,
        Desert,
        Forest,
        Farm,
        Bedroom,
        Kitchen,
        Livingroom,
        Bathroom,
        Above,
        Under
    }

    public BiomeType selectedBiome = BiomeType.Forest;

    public Texture2D beachTexture;
    public Texture2D desertTexture;
    public Texture2D forestTexture;
    public Texture2D farmTexture;
    public Texture2D bedroomTexture;
    public Texture2D kitchenTexture;
    public Texture2D livingroomTexture;
    public Texture2D bathroomTexture;
    public Texture2D aboveTexture;
    public Texture2D underTexture;


    public float terrainScale = 5f;
    public float heightMultiplier = 2f;
    public float edgeBoundary = 0.5f;

    private bool[,] gridOccupied = new bool[10, 10];
    private Vector3 planeNormal;
    private float planeSize;


    public void GenerateWorld()
    {
        ApplyBiomeTexture();
        GenerateHeight();
        planeNormal = this.transform.up;
        Bounds bounds = GetComponent<MeshFilter>().mesh.bounds;
        planeSize = Mathf.Max(bounds.size.x, bounds.size.z) * 0.5f;
        PlaceBiomeAssets();
    }

    private void ApplyBiomeTexture()
    {
        Texture2D selectedTexture = null;
        switch (selectedBiome)
        {
            case BiomeType.Beach:
                selectedTexture = beachTexture;
                break;
            case BiomeType.Desert:
                selectedTexture = desertTexture;
                break;
            case BiomeType.Forest:
                selectedTexture = forestTexture;
                break;
            case BiomeType.Farm:
                selectedTexture = farmTexture;
                break;
            case BiomeType.Bedroom:
                selectedTexture = bedroomTexture;
                break;
            case BiomeType.Kitchen:
                selectedTexture = kitchenTexture;
                break;
            case BiomeType.Livingroom:
                selectedTexture = livingroomTexture;
                break;
            case BiomeType.Bathroom:
                selectedTexture = bathroomTexture;
                break;
            case BiomeType.Above:
                selectedTexture = aboveTexture;
                break;
            case BiomeType.Under:
                selectedTexture = underTexture;
                break;
        }

        if (selectedTexture != null)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = selectedTexture;
                renderer.material.shader = Shader.Find("Unlit/Texture");
            }
        }
    }

    private void GenerateHeight()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        float minX = Mathf.Infinity, minZ = Mathf.Infinity;
        float maxX = -Mathf.Infinity, maxZ = -Mathf.Infinity;
        foreach (Vector3 vert in vertices)
        {
            if (vert.x < minX) minX = vert.x;
            if (vert.x > maxX) maxX = vert.x;
            if (vert.z < minZ) minZ = vert.z;
            if (vert.z > maxZ) maxZ = vert.z;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            float edgeFactorX = Mathf.InverseLerp(minX, minX + edgeBoundary, Mathf.Abs(vertices[i].x));
            edgeFactorX = Mathf.Min(edgeFactorX, Mathf.InverseLerp(maxX, maxX - edgeBoundary, Mathf.Abs(vertices[i].x)));

            float edgeFactorZ = Mathf.InverseLerp(minZ, minZ + edgeBoundary, Mathf.Abs(vertices[i].z));
            edgeFactorZ = Mathf.Min(edgeFactorZ, Mathf.InverseLerp(maxZ, maxZ - edgeBoundary, Mathf.Abs(vertices[i].z)));

            float edgeFactor = Mathf.Min(edgeFactorX, edgeFactorZ);

            float height = Mathf.PerlinNoise(vertices[i].x / terrainScale, vertices[i].z / terrainScale) * heightMultiplier * edgeFactor;
            vertices[i].y = height;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Update the MeshCollider with the new mesh
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null; // Force refresh
            meshCollider.sharedMesh = mesh;
        }
    }

    private void PlaceBiomeAssets()
    {
        BiomePrefabs selectedBiomePrefabs = GetSelectedBiomePrefabs();
        Transform terrainTransform = this.transform;

        for (int i = 0; i < assetCount; i++)
        {
            bool placed = false;
            while (!placed)
            {
                int gridX = Random.Range(2, 10);
                int gridZ = Random.Range(2, 10);

                if (!gridOccupied[gridX, gridZ])
                {
                    gridOccupied[gridX, gridZ] = true;
                    Vector3 localPos = new Vector3(
                        (gridX - 5) * planeSize / 5,
                        0,
                        (gridZ - 5) * planeSize / 5
                    );
                    Vector3 worldPos = terrainTransform.TransformPoint(localPos) + planeNormal * 5;
                    if (Physics.Raycast(worldPos, -planeNormal * 10, out RaycastHit hit, 10))
                    {
                        if (hit.transform.gameObject.tag != "prop") {
                            GameObject asset = Instantiate(selectedBiomePrefabs.assets[Random.Range(0, selectedBiomePrefabs.assets.Count)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), terrainTransform);
                            placed = true;
                            asset.gameObject.tag = "prop";
                            foreach (Transform child in asset.transform) {
                                child.gameObject.tag = "prop";
                            }
                            placedObjects.Add(asset);
                        }
                    }
                }
            }
        }
    }

    private BiomePrefabs GetSelectedBiomePrefabs()
    {
        switch (selectedBiome)
        {
            case BiomeType.Beach:
                return beachPrefabs;
            case BiomeType.Desert:
                return desertPrefabs;
            case BiomeType.Forest:
                return forestPrefabs;
            case BiomeType.Farm:
                return farmPrefabs;
            case BiomeType.Bedroom:
                return bedroomPrefabs;
            case BiomeType.Kitchen:
                return kitchenPrefabs;
            case BiomeType.Livingroom:
                return livingroomPrefabs;
            case BiomeType.Bathroom:
                return bathroomPrefabs;
            case BiomeType.Above:
                return aboveGroundPrefabs;
            case BiomeType.Under:
                return underGroundPrefabs;
            default:
                return null;
        }
    }
    private BiomeType GetRandomBiome(BiomeType currentBiome)
    {
        int rand = Random.Range(1,6);
        switch (rand)
        {
            case 1:
                return BiomeType.Beach;
            case 2:
                return BiomeType.Desert;
            case 3:
                return BiomeType.Forest;
            case 4:
                return BiomeType.Farm;
            case 5:
                return BiomeType.Under;
            default:
                return BiomeType.Farm;
        }
    }

    public void ChangeBiomeAssets()
    {
        /* Coin flip to decide if we're changing assets on this plane, if we decide no but there aren't
         * any assets changed yet we'll override this to make sure there's at least one asset changed always */
        if(Random.Range(0, 2) == 0 && worldManager.GetChangedObjectCount() >= 1)
        {
            return;
        }

        GameObject randomObject = placedObjects[Random.Range(0, placedObjects.Count)];
        GameObject changedObject;
        int assetChangeType = Random.Range(0, 3);

        Transform terrainTransform = this.transform;
        Vector3 localPos;
        Vector3 worldPos;
        switch (assetChangeType)
        {
            case 0: // SCALE
                Debug.Log("CHANGING SCALE OF OBJECT " + randomObject.name);
                changedObject = Instantiate(randomObject, randomObject.transform.position, randomObject.transform.rotation, randomObject.transform.parent);
                float scale = Random.Range(0.5f, 2.0f);
                changedObject.transform.localScale *= scale;

                localPos = changedObject.transform.position;
                worldPos = terrainTransform.TransformPoint(localPos) + planeNormal * 5;

                if (Physics.Raycast(worldPos, -planeNormal * 10, out RaycastHit hit, 10))
                {
                    changedObject.transform.position = hit.point;
                    changedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }

                worldManager.AddChangedObject(changedObject);
                changedObject.name = "CHANGEDSCALE" + randomObject.name;
                changedObject.tag = "prop";
                break;
            case 1: // COLOR
                Debug.Log("CHANGING COLOR OF OBJECT " + randomObject.name);
                changedObject = Instantiate(randomObject, randomObject.transform.position, randomObject.transform.rotation, randomObject.transform.parent);
                if(changedObject.GetComponent<MeshRenderer>() == null)
                {
                    Material mat = changedObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
                    mat.color = new Color(
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f)
                    );
                }
                else
                {
                    Material material = changedObject.GetComponent<MeshRenderer>().material;
                    material.color = new Color(
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f)
                    );
                }
                changedObject.name = "CHANGEDCOLOR" + randomObject.name;
                changedObject.gameObject.tag = "prop";
                worldManager.AddChangedObject(changedObject);
                break;
            case 2: // ENTIRE OBJECT
                Debug.Log("CHANGING ENTIRE OBJECT " + randomObject.name);
                BiomeType currentBiome = selectedBiome;
                BiomeType newRandomBiome;
                do {
                    newRandomBiome = GetRandomBiome(currentBiome);
                }while (newRandomBiome == currentBiome);
                selectedBiome = newRandomBiome;
                changedObject = Instantiate(GetSelectedBiomePrefabs().assets[Random.Range(0, GetSelectedBiomePrefabs().assets.Count)], randomObject.transform.position, randomObject.transform.rotation, randomObject.transform.parent);
                changedObject.name = "CHANGEDOBJECT" + randomObject.name;
                changedObject.gameObject.tag = "prop";
                localPos = changedObject.transform.position;
                worldPos = terrainTransform.TransformPoint(localPos) + planeNormal * 5;
                if (Physics.Raycast(worldPos, -planeNormal * 10, out RaycastHit hit2, 10))
                {
                    changedObject.transform.position = hit2.point;
                    changedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit2.normal);
                }
                worldManager.AddChangedObject(changedObject);

                break;
        }

        Destroy(randomObject);
    }
}