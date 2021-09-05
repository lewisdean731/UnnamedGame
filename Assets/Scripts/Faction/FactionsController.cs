using System.Collections.Generic;
using UnityEngine;

public class FactionsController : MonoBehaviour
{
    public List<Faction> factions;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

#nullable enable
    public static Faction? GetFaction(FactionIdentifier factionId)
    {
        Faction[] factions = FindObjectsOfType<Faction>();

        foreach (Faction f in factions)
        {
            if (f.factionId == factionId.faction)
            {
                return f;
            }
        }

        return null;
    }
}
