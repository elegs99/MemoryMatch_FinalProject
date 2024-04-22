using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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

    private float swipeThreshold = 0.5f;
    private bool isChallengeMode = false;
    private bool firstTime = true;

    public TMPro.TextMeshProUGUI timerText;
    public TMPro.TextMeshProUGUI ObjChangeText;
    public TMPro.TextMeshProUGUI livesText;
    private GameObject dupWorld;

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
            isChallengeMode = true;
            SpawnChallengeMode();
        }
    }

    private void OnEnable()
    {
        thumbStick.action.Enable();
        thumbStick.action.performed += OnThumbstickMoved;
    }

    private void OnDisable()
    {
        thumbStick.action.performed -= OnThumbstickMoved;
        thumbStick.action.Disable();
    }


    private void OnThumbstickMoved(InputAction.CallbackContext context)
    {
        Vector2 thumbstickPosition = context.ReadValue<Vector2>();

        if (thumbstickPosition.x > .9f && dupWorld != null)
        {
            Debug.Log("right swipe");
            world.SetActive(false);
            dupWorld.SetActive(true);
            if (isChallengeMode && firstTime) {
                firstTime = false;
                StartCoroutine(nameof(StartChallengeTimer));
                timerText.enabled = true;
                ObjChangeText.enabled = true;
                livesText.enabled = true;
            }
        }

        else if (thumbstickPosition.x < -.9f && dupWorld != null && !isChallengeMode)
        {
            Debug.Log("left swipe");
            world.SetActive(true);
            dupWorld.SetActive(false);
        }
    }

    private void Update() {
        // Debug.Log(thumbStick.action.ReadValue<Vector2>());
        if(lives == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }
    }
    IEnumerator StartChallengeTimer() {
        while (counter >= 0)
        {
            timerText.text = $"Time Left: {counter}";
            counter--;
            yield return new WaitForSeconds(1);
        }
    }
    public void SpawnChallengeMode()
    {
        dupWorld = Instantiate(world, world.transform.position, world.transform.rotation);
        scripRefMovement.setCloneWorld(dupWorld);
        dupWorld.SetActive(false);

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

        ObjChangeText.text = $"# of Objects Changed: {changedObjects.Count}";
    }

    public void AddChangedObject(GameObject obj)
    {
        changedObjects.Add(obj);
    }

    public void RemoveChangedObject(GameObject obj)
    {
        changedObjects.Remove(obj);
        Destroy(obj);
        GetChangedObjectCount();
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
