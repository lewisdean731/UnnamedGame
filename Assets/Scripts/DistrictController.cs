using UnityEngine;

public class DistrictController : MonoBehaviour
{
    public new string name;
    public UniqueID uniqueId;
    private string id;
    public string description;
    public Faction controllingFaction;
    public Faction[] contestingFactions;
    public bool contested;
    public Vector3[] borders;
    public int moneyIncome = 0, ammunitionIncome = 0, fuelIncome = 0, materialsIncome = 0, foodIncome = 0;
    public string[] tags;

    // Start is called before the first frame update
    void Start()
    {
        id = uniqueId.guid;
        InvokeRepeating("UpdateInterval", GameManager.updateIntervalDistrict, GameManager.updateIntervalDistrict);
    }

    // Update is called once per frame
    void UpdateInterval()
    {

    }
}
