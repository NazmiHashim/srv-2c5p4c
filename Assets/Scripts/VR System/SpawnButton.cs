using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    public DraggableVRObject draggableVRObject;

    public void OnButtonClick()
    {
        Debug.Log("Clicked.");
        if (draggableVRObject != null)
        {
            Debug.Log("Ready to spawn.");
            draggableVRObject.SpawnGameObject();
        }
    }
}
