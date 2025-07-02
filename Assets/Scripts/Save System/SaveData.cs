using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string SaveName;
    public string SaveDate;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public List<Vector3> ObjectPositions;
    public List<bool> DestinationMaterialsChanged;
    public List<bool> DestinationLockedStates;
    public List<PrefabCount> PrefabCounts;
    public int TotalCount;

    public DateTime GetSaveDate()
    {
        return DateTime.Parse(SaveDate);
    }

    public void SetSaveDate(DateTime date)
    {
        SaveDate = date.ToString("o"); // ISO 8601 format
    }
}
