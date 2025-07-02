using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrefabCountUI : MonoBehaviour
{
    public Transform textContainer;
    public TextMeshProUGUI totalCountText;
    private Dictionary<string, TextMeshProUGUI> prefabTextElements = new Dictionary<string, TextMeshProUGUI>();

    private void Start()
    {
        InitializeTextElements();
    }

    private void InitializeTextElements()
    {
        foreach (Transform child in textContainer)
        {
            TextMeshProUGUI textElement = child.GetComponent<TextMeshProUGUI>();
            if (textElement != null)
            {
                prefabTextElements[child.name] = textElement;
                Debug.Log("Initialized text element for " + child.name);
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found on " + child.name);
            }
        }
    }

    public void UpdateCounts(Dictionary<string, int> prefabCounts, int totalCount)
    {
        foreach (var prefabCount in prefabCounts)
        {
            if (prefabTextElements.ContainsKey(prefabCount.Key))
            {
                prefabTextElements[prefabCount.Key].text = $"{prefabCount.Key}: {prefabCount.Value}";
                Debug.Log("Updated UI for " + prefabCount.Key + ": " + prefabCount.Value);
            }
            else
            {
                Debug.LogWarning("No UI element found for prefab: " + prefabCount.Key);
            }
        }
        totalCountText.text = $"Total Count: {totalCount}";
        Debug.Log("Updated total count: " + totalCount);
    }
}
