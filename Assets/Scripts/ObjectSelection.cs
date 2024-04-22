using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSelection : MonoBehaviour
{
    public Transform posThumb, posPointer;
    public InputActionReference triggerVal;
    public InputActionReference selectItemButton;
    public Material selectHighlight;
    public GameObject selectionSphere;
    List<GameObject> changedObjects = new List<GameObject>();
    Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    HashSet<GameObject> currentObjects = new HashSet<GameObject>();
    void Start() {
        if (selectionSphere != null) {
            selectionSphere.SetActive(false); // Initially disable the sphere
        }
    }
    private void Awake()
    {
        selectItemButton.action.performed += OnSelection;
    }

    private void OnEnable()
    {
        selectItemButton.action.Enable();
    }

    void OnSelection(InputAction.CallbackContext context)
    {
        float triggerValue = triggerVal.action.ReadValue<float>();
        float distance = Vector3.Distance(posThumb.position, posPointer.position);
        float minDistance = .005f;

        if (distance < minDistance && currentObjects.Count == 1)
        {
            GameObject found = changedObjects.FirstOrDefault(x => x == currentObjects.ElementAt(0));
            WorldManager worldManager = GameObject.Find("XR Origin (XR Rig)").GetComponent<WorldManager>();
            if (found != null)
            {
                Debug.Log(found.name);
                worldManager.RemoveChangedObject(found);
            }
            else
            {
                worldManager.RemoveLife();
            }
        }
    }

    void Update()
    {
        float triggerValue = triggerVal.action.ReadValue<float>();
        float distance = Vector3.Distance(posThumb.position, posPointer.position);
        
        Vector3 middlePoint = (posThumb.position + posPointer.position) / 2;
        selectionSphere.transform.position = middlePoint;
        
        float minDistance = .005f;
        float maxDistance = .065f;

        if (distance < maxDistance && triggerValue > .1) {
            selectionSphere.SetActive(true);
            float scale = Mathf.Lerp(.03f, .1f, (distance - minDistance) / (maxDistance - minDistance));
            selectionSphere.transform.localScale = new Vector3(scale, scale, scale);

            HighlightObjects();
        } else {
            selectionSphere.SetActive(false);
            ResetMaterials();
        }
    }

    private void HighlightObjects() {
        currentObjects.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(selectionSphere.transform.position, selectionSphere.transform.localScale.x / 2);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.tag == "prop") {
                GameObject obj = hitCollider.gameObject;
                currentObjects.Add(obj);
                if (!originalMaterials.ContainsKey(obj)) {
                    if (obj.TryGetComponent<MeshRenderer>(out var meshRenderer)) {
                        originalMaterials[obj] = meshRenderer.material;
                        meshRenderer.material = selectHighlight;
                    } else {
                        meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
                        if (meshRenderer != null) {
                            originalMaterials[obj] = meshRenderer.material;
                            meshRenderer.material = selectHighlight;
                        }
                    }
                }
            }
        }

        foreach (var obj in originalMaterials.Keys) {
            if (!currentObjects.Contains(obj)) {
                if (obj.TryGetComponent<MeshRenderer>(out var meshRenderer)) {
                    meshRenderer.material = originalMaterials[obj];
                } else {
                    meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
                    if (meshRenderer != null) {
                        meshRenderer.material = originalMaterials[obj];
                    }
                }
                originalMaterials.Remove(obj);
            }
        }
    }

    private void ResetMaterials() {
        foreach (var obj in originalMaterials.Keys) {
            if (obj.TryGetComponent<MeshRenderer>(out var meshRenderer)) {
                meshRenderer.material = originalMaterials[obj];
            }
        }
        originalMaterials.Clear();
    }

    public void SetChangedObjectList(List<GameObject> newObjects) {
        changedObjects = newObjects;
    }
}