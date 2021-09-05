using System.Collections.Generic;
using UnityEngine;

public class FactionUnits : MonoBehaviour
{
    // TODO: split by unit types
    // e.g. public List<GameObject> millitants, trucks, technicals, elites;
    public List<GameObject> units;

    private void Awake()
    {
        units = new List<GameObject>();
    }
}
