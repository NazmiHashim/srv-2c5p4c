using UnityEngine;

public class OpenAndCloseLockPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject childPanel;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float uiDistance = 2f;
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);

    void Start()
    {
        childPanel.SetActive(false);
    }

    private void LateUpdate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        UpdatePosition();
#endif
    }

    private void UpdatePosition()
    {
        // position UI a certain distance in front of the player
        Vector3 uiPosition = playerTransform.position + playerTransform.forward * uiDistance + uiOffset;
        this.transform.position = uiPosition;

        // set UI to look at the player, but maintain its original 'up' direction
        Vector3 lookPosition = new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z);
        this.transform.LookAt(lookPosition, Vector3.up);

        // flip the UI 180 degrees around the Y axis and apply the rotation offset
        this.transform.Rotate(0, 180, 0);
        this.transform.Rotate(rotationOffset);
    }

    public void ShowPanel()
    {
        childPanel.SetActive(true);
        UpdatePosition();
    }

    public void HidePanel()
    {
        childPanel.SetActive(false);
    }
}
