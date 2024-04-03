using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LowPolyTerrainGenerator : MonoBehaviour
{
    [System.Serializable]
    public class BiomePrefabs
    {
        public List<GameObject> assets = new List<GameObject>();
    }

    public int assetCount = 10;

    public BiomePrefabs beachPrefabs;
    public BiomePrefabs desertPrefabs;
    public BiomePrefabs forestPrefabs;

    public enum BiomeType
    {
        Beach,
        Desert,
        Forest
    }

    public BiomeType selectedBiome = BiomeType.Forest;

    public Texture2D beachTexture;
    public Texture2D desertTexture;
    public Texture2D forestTexture;

    public float terrainScale = 5f;
    public float heightMultiplier = 2f;
    public float edgeBoundary = 0.5f;

    private bool[,] gridOccupied = new bool[10, 10];
    private Vector3 planeNormal;
    private float planeSize;

    private void Start()
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
        }

        if (selectedTexture != null)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = selectedTexture;
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
                        GameObject asset = Instantiate(selectedBiomePrefabs.assets[Random.Range(0, selectedBiomePrefabs.assets.Count)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), terrainTransform);
                        placed = true;
                    }
                }
            }
        }
    }

    private void AdjustHeightToTerrain(GameObject asset, Vector3 planeNormal)
    {
        RaycastHit hit;
        if (Physics.Raycast(asset.transform.position + planeNormal * 50, -planeNormal, out hit, Mathf.Infinity))
        {
            float pivotOffset = 0.1f; 
            asset.transform.position = hit.point + planeNormal * pivotOffset;
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
            default:
                return null;
        }
    }
}
