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
    public IconUIController iconUIController;

    public string levelDifficulty;
    public int lives = 3;
    private int counter = 45;
    public bool isChallengeMode = false;
    private bool firstTime = true;
    private GameObject dupWorld;

    public TMPro.TextMeshProUGUI timerText;

    private void Start()
    {
        levelDifficulty = StateNameController.difficulty;
        foreach (Transform child in world.transform)
        {
            if (child.name.Contains("Face"))
            {
                child.GetComponent<LowPolyTerrainGenerator>().GenerateWorld();
            }
        }
        if (levelDifficulty.ToLower() == "normal")
        {
            SpawnCasualMode();

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
            //Debug.Log("right swipe");
            world.SetActive(false);
            dupWorld.SetActive(true);
            scriptRefSelectL.StartSelection();
            scriptRefSelectR.StartSelection();
            if (isChallengeMode && firstTime) {
                iconUIController.SetSearchIcons(changedObjects.Count);
                iconUIController.ShowLivesUI();
                firstTime = false;
                StartCoroutine(nameof(StartChallengeTimer));
            } else if (firstTime) {
                iconUIController.SetSearchIcons(changedObjects.Count);
                firstTime = false;
                timerText.enabled = false;
            } 
        } else if (thumbstickPosition.x < -.9f && dupWorld != null && !isChallengeMode) {
            //Debug.Log("left swipe");
            world.SetActive(true);
            dupWorld.SetActive(false);
            scriptRefSelectL.StopSelection();
            scriptRefSelectR.StopSelection();
        }
        if (changedObjects.Count == 0)
        {
            StartCoroutine(nameof(WinGame));
        }
    }

    IEnumerator WinGame()
    {
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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
    }
    public void SpawnCasualMode()
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
    }

    public void AddChangedObject(GameObject obj)
    {
        changedObjects.Add(obj);
    }

    public void RemoveChangedObject(GameObject obj)
    {
        changedObjects.Remove(obj);
        GetChangedObjectCount();
    }
    public void updateFoundObjectUI() {
        iconUIController.SetFoundObj();
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
        iconUIController.RemoveLife();
        if (lives == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }
    }
}
