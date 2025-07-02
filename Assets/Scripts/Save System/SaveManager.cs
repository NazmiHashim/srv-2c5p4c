using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFolderPath => Path.Combine(Application.persistentDataPath, "Saves");
    private List<bool> destinationMaterialsChanged;
    private List<bool> destinationLockedStates;
    private Dictionary<string, int> prefabCounts;
    private int totalCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SaveManager Instance initialized.");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }

    public void InitializeDestinationMaterialsChangedList(int totalDestinations)
    {
        destinationMaterialsChanged = new List<bool>(new bool[totalDestinations]);
        destinationLockedStates = new List<bool>(new bool[totalDestinations]);
        Debug.Log($"Initialized destinationMaterialsChanged list with {totalDestinations} entries.");
    }

    public void InitializePrefabCounts(Dictionary<string, int> counts)
    {
        prefabCounts = new Dictionary<string, int>(counts);
        CalculateTotalCount();
        Debug.Log("Initialized prefab counts: " + string.Join(", ", prefabCounts.Select(kvp => kvp.Key + ": " + kvp.Value)));
    }

    public void SaveGame(string saveName, Vector3 playerPosition, Quaternion playerRotation, List<Vector3> objectPositions)
    {
        prefabCounts = DragNDropConfig.Instance.prefabCounts;
        totalCount = DragNDropConfig.Instance.CalculateTotalCount();

        SaveData saveData = new SaveData
        {
            SaveName = saveName,
            PlayerPosition = playerPosition,
            PlayerRotation = playerRotation,
            ObjectPositions = objectPositions,
            DestinationMaterialsChanged = destinationMaterialsChanged,
            DestinationLockedStates = destinationLockedStates,
            PrefabCounts = prefabCounts.Select(kvp => new PrefabCount(kvp.Key, kvp.Value)).ToList(),
            TotalCount = totalCount
        };

        saveData.SetSaveDate(DateTime.Now);

        string json = JsonUtility.ToJson(saveData, true);
        string saveFilePath = Path.Combine(saveFolderPath, saveName + ".json");
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game Saved: {saveName}");
        Debug.Log($"Saved Prefab Counts: {string.Join(", ", prefabCounts.Select(kvp => kvp.Key + ": " + kvp.Value))}");
    }

    public SaveData LoadGame(string saveName)
    {
        string saveFilePath = Path.Combine(saveFolderPath, saveName + ".json");
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Game Loaded: {saveName}");
            destinationMaterialsChanged = saveData.DestinationMaterialsChanged;
            destinationLockedStates = saveData.DestinationLockedStates;
            prefabCounts = saveData.PrefabCounts.ToDictionary(pc => pc.PrefabName, pc => pc.Count);
            totalCount = saveData.TotalCount;
            Debug.Log($"Loaded Prefab Counts: {string.Join(", ", prefabCounts.Select(kvp => kvp.Key + ": " + kvp.Value))}");
            return saveData;
        }
        else
        {
            Debug.LogError($"Save file not found: {saveName}");
            return null;
        }
    }

    public List<SaveData> GetAllSaves()
    {
        List<SaveData> saveFiles = new List<SaveData>();
        string[] files = Directory.GetFiles(saveFolderPath, "*.json");
        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            saveFiles.Add(saveData);
        }
        return saveFiles;
    }

    public string GenerateSaveFileName()
    {
        string[] files = Directory.GetFiles(saveFolderPath, "Save_*.json");
        int maxNumber = 0;

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName.StartsWith("Save_"))
            {
                string numberStr = fileName.Substring(5);
                if (int.TryParse(numberStr, out int number))
                {
                    if (number > maxNumber)
                    {
                        maxNumber = number;
                    }
                }
            }
        }

        int newSaveNumber = maxNumber + 1;
        return $"Save_{newSaveNumber:00}";
    }

    public void MarkDestinationChanged(Transform destination)
    {
        if (DragNDropConfig.Instance == null)
        {
            Debug.LogError("DragNDropConfig.Instance is not initialized.");
            return;
        }

        int index = DragNDropConfig.Instance.DragNDrops.SelectMany(d => d.Destinations).ToList().IndexOf(destination);
        if (index != -1 && index < destinationMaterialsChanged.Count)
        {
            destinationMaterialsChanged[index] = true;
        }
        else
        {
            Debug.LogError("Destination not found or index out of range");
        }
    }

    public void MarkDestinationLocked(Transform destination)
    {
        if (DragNDropConfig.Instance == null)
        {
            Debug.LogError("DragNDropConfig.Instance is not initialized.");
            return;
        }

        int index = DragNDropConfig.Instance.DragNDrops.SelectMany(d => d.Destinations).ToList().IndexOf(destination);
        if (index != -1 && index < destinationLockedStates.Count)
        {
            destinationLockedStates[index] = true;
        }
        else
        {
            Debug.LogError("Destination not found or index out of range");
        }
    }

    public List<bool> GetDestinationMaterialsChanged()
    {
        return destinationMaterialsChanged;
    }

    public void SetDestinationMaterialsChanged(List<bool> materialsChanged)
    {
        destinationMaterialsChanged = materialsChanged;
    }

    public List<bool> GetDestinationLockedStates()
    {
        return destinationLockedStates;
    }

    public void SetDestinationLockedStates(List<bool> lockedStates)
    {
        destinationLockedStates = lockedStates;
    }

    public Dictionary<string, int> GetPrefabCounts()
    {
        return prefabCounts;
    }

    public void SetPrefabCounts(Dictionary<string, int> counts)
    {
        prefabCounts = counts;
        CalculateTotalCount();
    }

    public int GetTotalCount()
    {
        return totalCount;
    }

    private void CalculateTotalCount()
    {
        totalCount = prefabCounts.Values.Sum();
    }

    public void ClearAllData()
    {
        string[] files = Directory.GetFiles(saveFolderPath, "*.json");
        foreach (var file in files)
        {
            File.Delete(file);
        }
        Debug.Log("All save data has been cleared.");
    }
}
