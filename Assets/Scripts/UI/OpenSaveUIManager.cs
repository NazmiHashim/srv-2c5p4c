using BNG;
using UnityEngine;

public class OpenSaveUIManager : MonoBehaviour
{
    public OpenAndCloseSaveUI saveUI;
    public bool isOpen;
    private bool previousBButtonState;

    // Start is called before the first frame update
    void Start()
    {
        previousBButtonState = InputBridge.Instance.BButton;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool currentBButtonState = InputBridge.Instance.BButton;

        if (currentBButtonState && !previousBButtonState)
        {
            isOpen = !isOpen;
            ToggleMenu(isOpen);
        }

        previousBButtonState = currentBButtonState;
    }

    public void SetIsOpenState(bool state)
    {
        isOpen = state;
    }

    private void ToggleMenu(bool value)
    {
        if (value)
        {
            OpenMenu();
        }
        else
        {
            CloseAllMenu();
        }
    }

    public void OpenMenu()
    {
        saveUI.OpenUI();
    }

    public void CloseAllMenu()
    {
        saveUI.CloseUI();
    }

    public void SimulateBButtonPress()
    {
        isOpen = !isOpen;
        ToggleMenu(isOpen);
    }
}
