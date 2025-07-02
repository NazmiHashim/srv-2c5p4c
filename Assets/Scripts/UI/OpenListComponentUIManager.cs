using BNG;
using UnityEngine;

public class OpenListComponentUIManager : MonoBehaviour
{
     public OpenAndCloseListComponentUI listComponentUI;
    public bool isOpen;
    private bool previousAButtonState;

    // Start is called before the first frame update
    void Start()
    {
        previousAButtonState = InputBridge.Instance.AButton;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool currentAButtonState = InputBridge.Instance.AButton;

        if (currentAButtonState && !previousAButtonState)
        {
            isOpen = !isOpen;
            ToggleMenu(isOpen);
        }

        previousAButtonState = currentAButtonState;
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
        listComponentUI.OpenUI();
    }

    public void CloseAllMenu()
    {
        listComponentUI.CloseUI();
    }

    public void SimulateAButtonPress()
    {
        isOpen = !isOpen;
        ToggleMenu(isOpen);
    }
}
