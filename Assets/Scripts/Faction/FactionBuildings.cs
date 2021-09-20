using System.Collections.Generic;
using UnityEngine;

public class FactionBuildings : MonoBehaviour
{
    public List<GameObject> buildings;

    private void Awake()
    {
        buildings = new List<GameObject>();
    }
}