using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconUIController : MonoBehaviour
{
    public GameObject[] searchIcons, foundIcons, livesIcon;
    private int foundIndex = 0;
    private int livesIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSearchIcons(int numObjs) {
        int index = 0;
        foreach (GameObject obj in searchIcons) {
            if (index < numObjs) {
                obj.SetActive(true);
                index++;
            }
        }
    }
    public void SetFoundObj() {
        foundIcons[foundIndex].SetActive(true);
        foundIndex++;
    }
    public void RemoveLife() {
        livesIcon[livesIndex].SetActive(false);
        livesIndex--;
    }
}
