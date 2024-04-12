using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private List<GameObject> changedObjects = new();
    public GameObject world;
    public ObjectSelection scriptRefSelect;
    public GodMovement scripRefMovement;

    private void Start()
    {
        foreach (Transform child in world.transform)
        {
            if (child.name.Contains("Face"))
            {
                child.GetComponent<LowPolyTerrainGenerator>().GenerateWorld();
            }
        }
        GameObject dupWorld = Instantiate(world, world.transform.position, world.transform.rotation);
        scripRefMovement.setCloneWorld(dupWorld);

        world.SetActive(false);

        foreach (Transform child in dupWorld.transform)
        {
            if (child.name.Contains("Face"))
            {
                LowPolyTerrainGenerator dupWorldTerrainGen = child.GetComponent<LowPolyTerrainGenerator>();
                foreach(Transform transform in child)
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

    public int GetChangedObjectCount()
    {
        scriptRefSelect.SetChangedObjectList(changedObjects);
        return changedObjects.Count;
    }
}
