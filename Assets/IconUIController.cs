using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class IconUIController : MonoBehaviour
{
    public GameObject[] searchIcons, foundIcons, livesIcon;
    private int foundIndex = 0;
    private int numChanged;
    private int livesIndex = 2;

    public GameObject searchIconPrefab;
    public GameObject foundIconPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InstantiateSearchIcons(int numObjects)
    {
        numChanged = numObjects;
        GameObject numChangedContainer = GameObject.Find("NumChangedObjects");
        for(int i = 0; i < numObjects; i++)
        {
            GameObject searchIcon = Instantiate(searchIconPrefab, Vector3.zero, Quaternion.identity, numChangedContainer.transform);
            searchIcon.SetActive(true);
            searchIcon.transform.localPosition = new Vector3(-6.0f + (i * 2.5f), 0, 0);
            searchIcons.Append(searchIcon);
        }
    }

    public void InstantiateFoundIcons(int numObjects)
    {
        GameObject numFoundContainer = GameObject.Find("NumChangedObjects");
        for (int i = 0; i < numObjects; i++)
        {
            GameObject foundIcon = Instantiate(foundIconPrefab, Vector3.zero, Quaternion.identity, numFoundContainer.transform);
            foundIcon.SetActive(false);
            foundIcon.transform.localPosition = new Vector3(-6.0f + (i * 2.5f), 0, -0.001f);
            foundIcons.Append(foundIcon);
        }
    }

    public void SetFoundObj() {
        if (foundIndex < numChanged-1) { 
            foundIcons[foundIndex].SetActive(true);
            foundIndex++;
        } else {
            // YOU WIN!!
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }
    }
    public void RemoveLife() {
        livesIcon[livesIndex].SetActive(false);
        livesIndex--;
    }
    public void ShowLivesUI() {
        foreach (GameObject obj in livesIcon) {
            obj.SetActive(true);
        }
    }
}
