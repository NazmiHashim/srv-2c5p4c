using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DragNDrop
{
    public Transform Source;
    public List<Transform> Destinations;
    public GameObject MovingPrefab;
}
