using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IconUIController : MonoBehaviour
{
    public GameObject[] searchIcons, foundIcons, livesIcon;
    private int foundIndex = 0;
    private int numChanged;
    private int livesIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSearchIcons(int numObjs) {
        int index = 0;
        numChanged = numObjs;
        foreach (GameObject obj in searchIcons) {
            if (index < numObjs) {
                obj.SetActive(true);
                index++;
            }
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
