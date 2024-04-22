using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{
    private List<GameObject> changedObjects = new();
    public GameObject world;
    public ObjectSelection scriptRefSelectL, scriptRefSelectR;
    public InputActionReference thumbStick;
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
            StartCoroutine(nameof(SpawnChallengeMode));
        }
    }
    private void Update() {
        Debug.Log(thumbStick.action.ReadValue<Vector2>());
    }
    IEnumerator SpawnChallengeMode()
    {
        GameObject dupWorld = Instantiate(world, world.transform.position, world.transform.rotation);
        scripRefMovement.setCloneWorld(dupWorld);
        dupWorld.SetActive(false);
        while (counter > 0)
        {
            timerText.text = $"Time until swap: {counter}";
            counter--;
            yield return new WaitForSeconds(1);
        }


        world.SetActive(false);
        dupWorld.SetActive(true);

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
        Destroy(obj);
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
        livesText.text = $"Lives: {lives}";
        if (lives == 0)
        {
            // Game over
            return;
        }
    }
}
