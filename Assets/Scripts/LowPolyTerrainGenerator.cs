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
    }

    private void PlaceBiomeAssets()
    {
        BiomePrefabs selectedBiomePrefabs = GetSelectedBiomePrefabs();
        Transform terrainTransform = this.transform;

        for (int i = 0; i < Mathf.Min(selectedBiomePrefabs.assets.Count, assetCount); i++)
        {
            bool placed = false;
            while (!placed)
            {
                int gridX = Random.Range(0, 10);
                int gridZ = Random.Range(0, 10);

                if (!gridOccupied[gridX, gridZ])
                {
                    gridOccupied[gridX, gridZ] = true;
                    Vector3 localPos = new Vector3(
                        (gridX - 4.5f) * planeSize / 4.5f,
                        0,
                        (gridZ - 4.5f) * planeSize / 4.5f
                    );
                    Vector3 worldPos = terrainTransform.TransformPoint(localPos) + planeNormal * 50f;
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, planeNormal);

                    RaycastHit hit;
                    if (Physics.Raycast(worldPos, -planeNormal, out hit, Mathf.Infinity))
                    {
                        GameObject asset = Instantiate(selectedBiomePrefabs.assets[i], hit.point, rotation, terrainTransform);
                        asset.transform.localScale = new Vector3(
                            1 / terrainTransform.localScale.x * .2f,
                            1 / terrainTransform.localScale.y * .2f,
                            1 / terrainTransform.localScale.z * .2f
                        );

                        AdjustHeightToTerrain(asset, planeNormal);
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
            float pivotOffset = 0.6f; 
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
