using UnityEngine;

public class FactionIdentifier : MonoBehaviour
{
    public Faction faction;

    public bool isItPartOfMyFaction(Faction toCompare)
    {
        if (toCompare == null) { return false; }
        if (toCompare == faction) { return true; }
        else return false;
    }
}
