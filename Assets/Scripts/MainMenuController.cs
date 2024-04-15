using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public Transform playerCamera;
    public float thresholdAngle = 30.0f;
    public float distanceFromPlayer = 2.0f;

    public GameObject entry;
    public GameObject levelSelect;

    public int level = 0;
    public TMPro.TextMeshProUGUI levelText;

    public List<string> levelNames = new List<string>();
    public GameObject leftButton;
    public GameObject rightButton;

    void Update()
    {
        Vector3 forwardFlat = playerCamera.forward;
        forwardFlat.y = 0;

        if (Vector3.Angle(forwardFlat, playerCamera.forward) > thresholdAngle)
        {
            transform.position = playerCamera.position + playerCamera.forward * distanceFromPlayer;
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        }


        if (level == 0)
        {
            leftButton.SetActive(false);
        }
        else
        {
            leftButton.SetActive(true);
        }
    }

    public void PressPlay()
    {
        entry.SetActive(false);
        levelSelect.SetActive(true);
    }

    public void PressLevel(string buttonName)
    {
        if(buttonName == "left")
        {
            level--;
        }
        else if(buttonName == "right")
        {
            level++;
        }

        levelText.text = levelNames[level];
    }

    public void PressStart(string input)
    {
        StateNameController.difficulty = input;
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelNames[level]);
    }
}
