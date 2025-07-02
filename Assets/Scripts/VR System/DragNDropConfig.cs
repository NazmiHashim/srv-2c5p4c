using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragNDropConfig : MonoBehaviour
{
    public static DragNDropConfig Instance;

    [Range(0, 10)]
    public float GoodDistance = 1f;
    public Material assemblyMaterial;
    public Material greenTransparentMaterial;
    public Material completelyTransparentMaterial;
    public List<DragNDrop> DragNDrops;
    private Dictionary<Transform, bool> lockedDestinations = new Dictionary<Transform, bool>();
    public Dictionary<string, int> prefabCounts { get; private set; } = new Dictionary<string, int>();

    private void Awake()
    {

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        InitializePrefabCounts();
    }

    private void InitializePrefabCounts()
    {
        foreach (var dragNDrop in DragNDrops)
        {
            if (!prefabCounts.ContainsKey(dragNDrop.MovingPrefab.name))
            {
                prefabCounts[dragNDrop.MovingPrefab.name] = 0;
            }
        }
        Debug.Log("Initialized prefab counts: " + string.Join(", ", prefabCounts.Select(kvp => kvp.Key + ": " + kvp.Value)));
    }

    public void IncrementPrefabCount(string prefabName)
    {
        if (prefabCounts.ContainsKey(prefabName))
        {
            prefabCounts[prefabName]++;
        }
        else
        {
            prefabCounts[prefabName] = 1;
        }
        Debug.Log("Incremented count for " + prefabName + ": " + prefabCounts[prefabName]);
        UpdateUIPrefabCounts();
    }

    public Dictionary<string, int> GetPrefabCounts()
    {
        return prefabCounts;
    }

    private void UpdateUIPrefabCounts()
    {
        var prefabCountUI = FindObjectOfType<PrefabCountUI>(true);
        if (prefabCountUI != null)
        {
            prefabCountUI.UpdateCounts(prefabCounts,CalculateTotalCount());
            Debug.Log("PrefabCountUI found and updated.");
        }
        else
        {
            Debug.LogWarning("PrefabCountUI not found in the scene.");
        }
    }

    public void SetPrefabCounts(Dictionary<string, int> counts)
    {
        prefabCounts = counts;
        UpdateUIPrefabCounts();
        Debug.Log("Set prefab counts: " + string.Join(", ", prefabCounts.Select(kvp => kvp.Key + ": " + kvp.Value)));
    }

    public DragNDrop GetDragNDrop(Transform source)
    {
        return DragNDrops.Find(dragNDrop => (dragNDrop.Source == source));
    }

    public void LockDestination(Transform destination)
    {
        if (!lockedDestinations.ContainsKey(destination))
        {
            lockedDestinations[destination] = true;
        }
    }

    public bool IsDestinationLocked(Transform destination)
    {
        return lockedDestinations.ContainsKey(destination) && lockedDestinations[destination];
    }

    public int CalculateTotalCount()
    {
        return prefabCounts.Values.Sum();
    }
}
