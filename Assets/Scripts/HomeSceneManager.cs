using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeSceneManager : MonoBehaviour
{
    public Button createNewButton;
    public Button loadButton;
    public Button clearDataButton;
    public GameObject initPanel;
    public GameObject loadPanel;
    public GameObject deleteDataPanel;
    public Transform saveListParent;
    public GameObject saveItemPrefab;

    [Header("Scene Name")]
    public string newGameScene;
    public string loadSaveGameScene;

    private void Start()
    {
        createNewButton.onClick.AddListener(CreateNewGame);
        loadButton.onClick.AddListener(ShowLoadPanel);
        clearDataButton.onClick.AddListener(ClearData);
        initPanel.SetActive(true);
        loadPanel.SetActive(false);
        PopulateSaveList();
    }

    private void CreateNewGame()
    {
        PlayerPrefs.SetString("SaveToLoad", string.Empty);
        SceneManager.LoadScene(newGameScene);
    }

    private void ShowLoadPanel()
    {
        initPanel.SetActive(false);
        deleteDataPanel.SetActive(false);
        loadPanel.SetActive(true);
    }

    private void ClearData()
    {
        SaveManager.Instance.ClearAllData();
        deleteDataPanel.SetActive(false);
        loadPanel.SetActive(false);
        initPanel.SetActive(true);
    }

    private void PopulateSaveList()
    {
        foreach (Transform child in saveListParent)
        {
            Destroy(child.gameObject);
        }

        List<SaveData> saveFiles = SaveManager.Instance.GetAllSaves();
        foreach (var saveData in saveFiles)
        {
            GameObject saveItem = Instantiate(saveItemPrefab, saveListParent);
            saveItem.transform.SetAsFirstSibling();
            DateTime saveDate = saveData.GetSaveDate();
            string formattedDate = saveDate.ToString("yyyy/MM/dd hh:mm tt");
            saveItem.GetComponentInChildren<TextMeshProUGUI>().text = $"{saveData.SaveName} - {formattedDate}";
            saveItem.GetComponent<Button>().onClick.AddListener(() => LoadGame(saveData.SaveName));
        }
    }

    private void LoadGame(string saveName)
    {
        PlayerPrefs.SetString("SaveToLoad", saveName);
        SceneManager.LoadScene(loadSaveGameScene);
    }
}
