using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
   [SerializeField] private GameObject childPanel;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float uiDistance = 2f;
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);

    void Start()
    {
        if (childPanel != null)
        {
            childPanel.SetActive(true); // Always show the UI
        }
    }

    private void LateUpdate()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        UpdatePosition(); // Always follow the player's head
#endif
    }

    private void UpdatePosition()
    {
        if (playerTransform == null || childPanel == null) return;

        // position UI a certain distance in front of the player
        Vector3 uiPosition = playerTransform.position + playerTransform.forward * uiDistance + uiOffset;
        transform.position = uiPosition;

        // set UI to look at the player, but maintain its original 'up' direction
        Vector3 lookPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookPosition, Vector3.up);

        // flip the UI 180 degrees around the Y axis and apply the rotation offset
        transform.Rotate(0, 180, 0);
        transform.Rotate(rotationOffset);
    }
}
