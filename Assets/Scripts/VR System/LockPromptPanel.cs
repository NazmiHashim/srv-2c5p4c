using System;
using UnityEngine;

public class LockPromptPanel : MonoBehaviour
{
    public UnityEngine.UI.Button yesButton;
    public UnityEngine.UI.Button noButton;
    private OpenAndCloseLockPromptUI uiController;

    private void Awake()
    {
        uiController = GetComponent<OpenAndCloseLockPromptUI>();
        if (uiController == null)
        {
            Debug.LogError("OpenAndCloseLockPromptUI component not found on the same GameObject.");
        }
    }

    public void ShowPrompt(Action onYes, Action onNo)
    {
        if (uiController != null)
        {
            uiController.ShowPanel();
        }

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() => { onYes?.Invoke(); HidePrompt(); });
        noButton.onClick.AddListener(() => { onNo?.Invoke(); HidePrompt(); });
    }

    private void HidePrompt()
    {
        if (uiController != null)
        {
            uiController.HidePanel();
        }
    }
}
