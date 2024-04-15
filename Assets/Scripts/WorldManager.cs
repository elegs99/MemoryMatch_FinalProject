using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private List<GameObject> changedObjects = new();
    public GameObject world;
    public ObjectSelection scriptRefSelectL, scriptRefSelectR;
    public GodMovement scripRefMovement;

    public string levelDifficulty;
    public int lives = 3;
    private int counter = 30;


    public TMPro.TextMeshProUGUI timerText;
    public TMPro.TextMeshProUGUI livesText;

    private void Start()
    {
        // levelDifficulty = StateNameController.difficulty;

        foreach (Transform child in world.transform)
        {
            if (child.name.Contains("Face"))
            {
                child.GetComponent<LowPolyTerrainGenerator>().GenerateWorld();
            }
        }
        if (levelDifficulty.ToLower() == "normal")
        {
            // Handle normal difficulty
            // Show original world by default

        }
        else if (levelDifficulty.ToLower() == "challenge")
        {
            // Handle hard difficulty
            // 30 second timer to look at original world
            // Then show duplicate world
            Debug.Log("In challenge mode");
            StartCoroutine(nameof(SpawnChallengeMode));
        }
    }

    IEnumerator SpawnChallengeMode()
    {
        while (counter > 0)
        {
            timerText.text = $"Time until swap: {counter}";
            counter--;
            yield return new WaitForSeconds(1);
        }

        GameObject dupWorld = Instantiate(world, world.transform.position, world.transform.rotation);
        scripRefMovement.setCloneWorld(dupWorld);

        world.SetActive(false);

        foreach (Transform child in dupWorld.transform)
        {
            if (child.name.Contains("Face"))
            {
                LowPolyTerrainGenerator dupWorldTerrainGen = child.GetComponent<LowPolyTerrainGenerator>();
                foreach (Transform transform in child)
                {
                    dupWorldTerrainGen.placedObjects.Add(transform.gameObject);
                }
                dupWorldTerrainGen.worldManager = this;
                dupWorldTerrainGen.ChangeBiomeAssets();
            }
        }

        timerText.text = $"Objects changed in scene: {changedObjects.Count}";
    }

    public void AddChangedObject(GameObject obj)
    {
        changedObjects.Add(obj);
    }

    public void RemoveChangedObject(GameObject obj)
    {
        changedObjects.Remove(obj);
    }

    public int GetChangedObjectCount()
    {
        scriptRefSelectL.SetChangedObjectList(changedObjects);
        scriptRefSelectR.SetChangedObjectList(changedObjects);
        return changedObjects.Count;
    }

    public void RemoveLife()
    {
        lives--;
        if (lives == 0)
        {
            // Game over
            return;
        }
        livesText.text = $"Lives: {lives}";
    }
}
