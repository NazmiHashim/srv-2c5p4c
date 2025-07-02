using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public Transform player;
    public List<Transform> objects;
    public GameObject savePromptPanel;
    public Button saveButton;
    public Button cancelButton;
    private string currentSaveName;

    [Header("Scene Name")]
    public string homeSceneName;

    private void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGame);
        }
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelSave);
        }
        InitializeDestinationMaterialsChangedList();
        LoadGameIfAvailable();
    }

    private void InitializeDestinationMaterialsChangedList()
    {
        if (DragNDropConfig.Instance == null)
        {
            Debug.LogError("DragNDropConfig.Instance is null");
            return;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance is null");
            return;
        }

        int totalDestinations = DragNDropConfig.Instance.DragNDrops.SelectMany(d => d.Destinations).Count();
        SaveManager.Instance.InitializeDestinationMaterialsChangedList(totalDestinations);
    }

    private void SaveGame()
    {
        List<Vector3> objectPositions = new List<Vector3>();
        foreach (var obj in objects)
        {
            objectPositions.Add(obj.localPosition);
        }

        if (string.IsNullOrEmpty(currentSaveName))
        {
            currentSaveName = SaveManager.Instance.GenerateSaveFileName();
        }

        SaveManager.Instance.SaveGame(currentSaveName, player.localPosition, player.localRotation, objectPositions);
        savePromptPanel.SetActive(false);
    }

    private void CancelSave()
    {
        savePromptPanel.SetActive(false);
    }

    private void LoadGameIfAvailable()
    {
        currentSaveName = PlayerPrefs.GetString("SaveToLoad", null);
        if (!string.IsNullOrEmpty(currentSaveName))
        {
            SaveData saveData = SaveManager.Instance.LoadGame(currentSaveName);
            if (saveData != null)
            {
                player.localPosition = saveData.PlayerPosition;
                player.localRotation = saveData.PlayerRotation;
                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i].localPosition = saveData.ObjectPositions[i];
                }
                SaveManager.Instance.SetDestinationMaterialsChanged(saveData.DestinationMaterialsChanged);
                SaveManager.Instance.SetDestinationLockedStates(saveData.DestinationLockedStates);
                DragNDropConfig.Instance.SetPrefabCounts(saveData.PrefabCounts.ToDictionary(pc => pc.PrefabName, pc => pc.Count));
                ApplyMaterialChanges(SaveManager.Instance.GetDestinationMaterialsChanged());
                ApplyLockedStates(SaveManager.Instance.GetDestinationLockedStates());
                ApplyPrefabCounts(SaveManager.Instance.GetPrefabCounts());
            }
            // Do not clear PlayerPrefs key here as it is used to track current save file
        }
        else
        {
            currentSaveName = string.Empty;
        }
    }

    private void ApplyMaterialChanges(List<bool> destinationMaterialsChanged)
    {
        if (DragNDropConfig.Instance == null)
        {
            Debug.LogError("DragNDropConfig.Instance is null");
            return;
        }

        List<Transform> allDestinations = DragNDropConfig.Instance.DragNDrops.SelectMany(d => d.Destinations).ToList();
        for (int i = 0; i < destinationMaterialsChanged.Count; i++)
        {
            if (destinationMaterialsChanged[i])
            {
                Transform destination = allDestinations[i];
                MeshRenderer meshRenderer = destination.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.material = DragNDropConfig.Instance.assemblyMaterial;
                }
            }
        }
    }

    private void ApplyLockedStates(List<bool> destinationLockedStates)
    {
        if (DragNDropConfig.Instance == null)
        {
            Debug.LogError("DragNDropConfig.Instance is null");
            return;
        }

        List<Transform> allDestinations = DragNDropConfig.Instance.DragNDrops.SelectMany(d => d.Destinations).ToList();
        for (int i = 0; i < destinationLockedStates.Count; i++)
        {
            if (destinationLockedStates[i])
            {
                Transform destination = allDestinations[i];
                DragNDropConfig.Instance.LockDestination(destination);
            }
        }
    }

    private void ApplyPrefabCounts(Dictionary<string, int> prefabCounts)
    {
        if (DragNDropConfig.Instance == null)
        {
            Debug.LogError("DragNDropConfig.Instance is null");
            return;
        }

        DragNDropConfig.Instance.SetPrefabCounts(prefabCounts);
    }
}
