using UnityEngine;

public class Faction : MonoBehaviour
{
    public FactionIdentifier factionId;
    public FactionTerritory territory;
    public FactionResources resources;
    public FactionBuildings buildings;
    public FactionUnits units;
    public FactionPolicies policies;

    public new string name;

    public void Initialise()
    {
        this.gameObject.AddComponent<FactionIdentifier>();
        this.gameObject.AddComponent<FactionTerritory>();
        this.gameObject.AddComponent<FactionResources>();
        this.gameObject.AddComponent<FactionBuildings>();
        this.gameObject.AddComponent<FactionUnits>();
        this.gameObject.AddComponent<FactionPolicies>();

        factionId = this.gameObject.GetComponent<FactionIdentifier>();
        factionId.faction = this;

        territory = this.gameObject.GetComponent<FactionTerritory>();
        resources = this.gameObject.GetComponent<FactionResources>();
        buildings = this.gameObject.GetComponent<FactionBuildings>();
        units = this.gameObject.GetComponent<FactionUnits>();
        policies = this.gameObject.GetComponent<FactionPolicies>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
