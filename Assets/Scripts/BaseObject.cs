using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public List<GameObject> accessoryPrefabs;

    public void PlaceRandomAccessory()
    {
        if (accessoryPrefabs != null && accessoryPrefabs.Count > 0)
        {
            int index = Random.Range(0, accessoryPrefabs.Count);
            GameObject accessoryPrefab = accessoryPrefabs[index];

            GameObject accessory = Instantiate(accessoryPrefab, transform.position, Quaternion.identity, this.transform);

            float scaleFactor = Random.Range(0.25f, 1f);
            accessory.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            Collider baseCollider = GetComponent<Collider>();
            Collider accessoryCollider = accessory.GetComponent<Collider>();

            if (baseCollider != null && accessoryCollider != null)
            {
                float baseTop = baseCollider.bounds.max.y;
                float accessoryBottom = accessoryCollider.bounds.min.y;
                float offset = baseTop - accessoryBottom;
                accessory.transform.position += new Vector3(0f, offset + accessoryCollider.bounds.extents.y * (scaleFactor - 1f), 0f);
            }
        }
    }
}
