using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject[] baseObjectPrefabs;
    public GameObject[] accessoryObjectPrefabs;
    public float cellSize = 1.0f;

    private void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 spawnPosition = new Vector3(x * cellSize, 0, z * cellSize);
                InstantiateRandomBaseObject(spawnPosition);
            }
        }
    }

    void InstantiateRandomBaseObject(Vector3 position)
    {
        int index = Random.Range(0, baseObjectPrefabs.Length);
        GameObject selectedPrefab = baseObjectPrefabs[index];
        GameObject baseObject = Instantiate(selectedPrefab, position, Quaternion.identity);

        BaseObject baseObjectScript = baseObject.GetComponent<BaseObject>();
        if (baseObjectScript != null)
        {
            baseObjectScript.accessoryPrefabs = new System.Collections.Generic.List<GameObject>(accessoryObjectPrefabs);
            if (Random.value > 0.5f)
            {
                baseObjectScript.PlaceRandomAccessory();
            }
        }
    }
}
