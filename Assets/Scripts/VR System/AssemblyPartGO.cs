using System.Collections;
using System.Collections.Generic;
using BNG;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyPartGO : MonoBehaviour
{
    [SerializeField] private Grabbable handleGrabbable;
    [SerializeField] private TextMeshProUGUI distanceText;
    private DragNDrop dragNDrop;
    private Vector3 startPos;
    private bool isNearDestination;
    private Transform currentDestination;
    private bool shouldUpdateDistanceText = true;

    [Header("Grab Events")]
    public UnityEvent onGrab = new UnityEvent();
    public UnityEvent onRelease = new UnityEvent();

    public Grabbable HandleGrabbable { get => handleGrabbable; set => handleGrabbable = value; }

    public void Initialize(DragNDrop dragNDrop)
    {
        this.dragNDrop = dragNDrop;
        startPos = transform.position;

        handleGrabbable = GetComponent<Grabbable>();
        if (handleGrabbable == null)
        {
            Debug.LogError("AssemblyPartGO: Grabbable component not found.");
            return;
        }

        onRelease.AddListener(DropItem);

        GameObject distanceTextObject = GameObject.FindWithTag("DistanceText");
        if (distanceTextObject != null)
        {
            distanceText = distanceTextObject.GetComponent<TextMeshProUGUI>();
            Debug.Log("TextMeshProUGUI component found and assigned.");
        }
        else
        {
            Debug.LogError("DistanceText GameObject with the specified tag not found.");
        }
    }

    void Update()
    {
        if (dragNDrop == null)
        {
            Debug.LogWarning("AssemblyPartGO: DragNDrop is not initialized.");
            return;
        }

        bool previouslyNearDestination = isNearDestination;
        currentDestination = GetClosestDestination();
        isNearDestination = currentDestination != null && IsInGoodPosition(transform.position, currentDestination);

        if (isNearDestination != previouslyNearDestination)
        {
            ChangeDestinationMaterial(isNearDestination);
        }
        Debug.Log($"Previously Near Destination: {previouslyNearDestination}");
        Debug.Log($"AssemblyPartGO: Update method called. IsNearDestination: {isNearDestination}");

        if (handleGrabbable.BeingHeld)
        {
            onGrab?.Invoke();
        }
        else
        {
            onRelease?.Invoke();
        }

        if (shouldUpdateDistanceText)
        {
            UpdateDistanceText(transform.position, currentDestination?.position);
        }
    }

    public void DropItem()
    {
        if (isNearDestination && currentDestination != null)
        {
            MeshRenderer meshRenderer = currentDestination.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = DragNDropConfig.Instance.assemblyMaterial;
                Debug.Log("AssemblyPartGO: Object dropped near destination. Material changed to assemblyMaterial.");
                shouldUpdateDistanceText = false;
                ResetDistanceText();
                SaveManager.Instance.MarkDestinationChanged(currentDestination);
                DragNDropConfig.Instance.IncrementPrefabCount(dragNDrop.MovingPrefab.name);
            }
             else
            {
                Debug.LogError("AssemblyPartGO: MeshRenderer not found on dragNDrop.Source.");
            }
            Destroy(gameObject);
        }
        ShowLockPrompt(currentDestination);
    }

    private Transform GetClosestDestination()
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (var destination in dragNDrop.Destinations)
        {
            if (destination != null)
            {
                float distance = Vector3.Distance(transform.position, destination.position);
                if (distance < closestDistance)
                {
                    closest = destination;
                    closestDistance = distance;
                }
            }
        }

        return closest;
    }

    private bool IsInGoodPosition(Vector3 pos, Transform destination)
    {
        bool inGoodPosition = Vector3.Distance(pos, destination.position) <= DragNDropConfig.Instance.GoodDistance;
        Debug.Log($"AssemblyPartGO: Checking if in good position. Distance: {Vector3.Distance(pos, destination.position)}, InGoodPosition: {inGoodPosition}");
        return inGoodPosition;
    }

    private void ChangeDestinationMaterial(bool isNear)
    {
        if (currentDestination != null && !DragNDropConfig.Instance.IsDestinationLocked(currentDestination))
        {
            MeshRenderer meshRenderer = currentDestination.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Material material = isNear ? DragNDropConfig.Instance.greenTransparentMaterial : DragNDropConfig.Instance.completelyTransparentMaterial;
                meshRenderer.material = material;
                Debug.Log($"AssemblyPartGO: Changing destination material to {(isNear ? "green" : "transparent")}. IsNear: {isNear}");
            }
            else
            {
                Debug.LogError("AssemblyPartGO: MeshRenderer not found on currentDestination.");
            }
        }
    }

    private void UpdateDistanceText(Vector3 pos, Vector3? destinationPos)
    {
        if (distanceText != null && destinationPos.HasValue)
        {
            float distance = Vector3.Distance(pos, destinationPos.Value);
            distanceText.text = $"Distance: {distance:F2} m";
        }
    }

    private void ResetDistanceText()
    {
        if (distanceText != null)
        {
            distanceText.text = "Distance: 0.00 m";
            Debug.Log("Distance text has been reset.");
        }
        else
        {
            Debug.LogError("distanceText is null when trying to reset it.");
        }
    }

    private void ShowLockPrompt(Transform destination)
    {
        var lockPromptPanel = FindObjectOfType<LockPromptPanel>();
        if (lockPromptPanel != null)
        {
            lockPromptPanel.ShowPrompt(
                onYes: () =>
                {
                    DragNDropConfig.Instance.LockDestination(destination);
                    SaveManager.Instance.MarkDestinationLocked(destination); // Mark destination as locked in save manager
                    Debug.Log("Material change is locked.");
                },
                onNo: () => { Debug.Log("Material change is not locked."); }
            );
        }
        else
        {
            Debug.LogError("LockPromptPanel not found in the scene.");
        }
    }
}
