using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSelection : MonoBehaviour
{
    private List<GameObject> changedObjects = new List<GameObject>();
    public Transform rightThumb, rightPointer;
    public InputActionReference triggerVal;
    public Material selectHighlight;
    public GameObject selectionSphere; // Sphere to visualize the selection area
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    void Start() {
        if (selectionSphere != null) {
            selectionSphere.SetActive(false); // Initially disable the sphere
        }
    }

    void Update()
    {
        float triggerValue = triggerVal.action.ReadValue<float>();
        float distance = Vector3.Distance(rightThumb.position, rightPointer.position);
        Vector3 middlePoint = (rightThumb.position + rightPointer.position) / 2;
        selectionSphere.transform.position = middlePoint;
        float minDistance = .005f;
        float maxDistance = .065f;

        if (distance < maxDistance && triggerValue > .1) {
            if (selectionSphere != null) {
                selectionSphere.SetActive(true);
                float scale = Mathf.Lerp(.02f, .06f, (distance - minDistance) / (maxDistance - minDistance));
                selectionSphere.transform.localScale = new Vector3(scale, scale, scale);
            }
            HighlightObjects();
        } else {
            if (selectionSphere != null) {
                selectionSphere.SetActive(false);
            }
            ResetMaterials();
        }
    }

    private void HighlightObjects() {
        Collider[] hitColliders = Physics.OverlapSphere(selectionSphere.transform.position, selectionSphere.transform.localScale.x / 2);
        HashSet<GameObject> currentObjects = new HashSet<GameObject>();

        // Add new objects and change their materials
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.tag == "prop") { // Make sure it is a prop
                GameObject obj = hitCollider.gameObject;
                currentObjects.Add(obj); // Store the objects inside of sphere collider
                if (!originalMaterials.ContainsKey(obj)) {
                    var meshRenderer = obj.GetComponent<MeshRenderer>();
                    if (meshRenderer != null) {
                        originalMaterials[obj] = meshRenderer.material; // Store the original material
                        meshRenderer.material = selectHighlight;
                    } else {
                        meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
                        if (meshRenderer != null) {
                            originalMaterials[obj] = meshRenderer.material; // Store the original material
                            meshRenderer.material = selectHighlight;
                        }
                    }
                }
            }
        }

        // Reset materials of objects that have left the sphere
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (var obj in originalMaterials.Keys) {
            if (!currentObjects.Contains(obj)) {
                var meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null) {
                    meshRenderer.material = originalMaterials[obj]; // Reset to the original material
                } else {
                    meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
                    if (meshRenderer != null) {
                        meshRenderer.material = originalMaterials[obj]; // Reset to the original material
                    }
                }
                objectsToRemove.Add(obj);
            }
        }

        // Clean up the dictionary
        foreach (var obj in objectsToRemove) {
            originalMaterials.Remove(obj);
        }
    }

    private void ResetMaterials() {
        foreach (var obj in originalMaterials.Keys) {
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshRenderer.material = originalMaterials[obj]; // Reset to the original material
            }
        }
        originalMaterials.Clear(); // Clear the dictionary after resetting
    }

    public void SetChangedObjectList(List<GameObject> newObjects) {
        changedObjects = newObjects;
    }
}