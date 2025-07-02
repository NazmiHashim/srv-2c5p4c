using System.Collections;
using System.Collections.Generic;
using BNG;
using UnityEngine;

public class OpenAssemblyUIManager : MonoBehaviour
{
    public OpenAndCloseAssemblyUI componentAssemblyUI;
    public bool isOpen;
    private bool previousYButtonState;

    // Start is called before the first frame update
    void Start()
    {
        previousYButtonState = InputBridge.Instance.YButton;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool currentYButtonState = InputBridge.Instance.YButton;

        if (currentYButtonState && !previousYButtonState)
        {
            isOpen = !isOpen;
            ToggleMenu(isOpen);
        }

        previousYButtonState = currentYButtonState;
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
        componentAssemblyUI.OpenUI();
    }

    public void CloseAllMenu()
    {
        componentAssemblyUI.CloseUI();
    }

    public void SimulateYButtonPress()
    {
        isOpen = !isOpen;
        ToggleMenu(isOpen);
    }
}
