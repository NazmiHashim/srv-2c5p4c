using System.Linq;
using BNG;
using UnityEngine;

public class DraggableVRObject : MonoBehaviour
{
    private DragNDrop dragNDrop;
    private GameObject movingObj;
    public OpenAssemblyUIManager openUIManager;

    protected void Start()
    {
        dragNDrop = DragNDropConfig.Instance.GetDragNDrop(transform);
    }

    public void SpawnGameObject()
    {
        if (dragNDrop != null)
        {
            movingObj = Instantiate(dragNDrop.MovingPrefab, transform.position, dragNDrop.MovingPrefab.transform.rotation);
            openUIManager.CloseAllMenu();
            Debug.Log("Spawned: " + movingObj.name);

            AssemblyPartGO assemblyPart = movingObj.AddComponent<AssemblyPartGO>();
            assemblyPart.Initialize(dragNDrop);

            GrabObject(movingObj);
        }
        else
        {
            Debug.LogError("DraggableVRObject: DragNDrop configuration is null.");
        }
    }

    private void GrabObject(GameObject obj)
    {
        Grabbable grabbable = obj.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            // Find the right hand Grabber
            Grabber rightHandGrabber = FindObjectsOfType<Grabber>().FirstOrDefault(g => g.HandSide == ControllerHand.Right);
            if (rightHandGrabber != null)
            {
                rightHandGrabber.GrabGrabbable(grabbable);
                Debug.Log("Object grabbed by right hand: " + obj.name);
            }
            else
            {
                Debug.LogError("No right hand Grabber component found in the scene.");
            }
        }
        else
        {
            Debug.LogError("No Grabbable component found on the object.");
        }
    }
}
