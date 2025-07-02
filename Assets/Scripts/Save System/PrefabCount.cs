using System;

[Serializable]
public class PrefabCount
{
    public string PrefabName;
    public int Count;

    public PrefabCount(string prefabName, int count)
    {
        PrefabName = prefabName;
        Count = count;
    }
}
