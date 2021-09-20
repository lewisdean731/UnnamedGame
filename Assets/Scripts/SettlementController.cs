using UnityEngine;

public class SettlementController : MonoBehaviour
{
    public int updateTimeInSeconds = 5;
    public DistrictController district;
    public UniqueID uniqueId;
    private string id;

    public int population;

    public int suppliesAmmo;
    public int suppliesFood;
    public int suppliesFuel;
    public int suppliesMaterials;

    public int productionMoney;

    public int morale;
    public bool playerControlled;

    // Special buildings?
    public int policePresenceLevel;

    public int armyPresenceLevel;
    public int hospitalLevel;
    public int schoolLevel;

    public string[] tags;

    // Start is called before the first frame update
    private void Start()
    {
        LoadSettlement();

        GameEvents.current.onSettlementDispatchFood += OnSettlementDispatchFood;
        GameEvents.current.onSettlementRecieveFood += OnSettlementRecieveFood;

        InvokeRepeating("UpdateInterval", GameManager.updateIntervalSettlement, GameManager.updateIntervalSettlement);
    }

    private void OnDestroy()
    {
        GameEvents.current.onSettlementDispatchFood -= OnSettlementDispatchFood;
        GameEvents.current.onSettlementRecieveFood -= OnSettlementRecieveFood;
    }

    private void LoadSettlement()
    {
        district = transform.parent.GetComponent<DistrictController>();

        this.gameObject.AddComponent<UniqueID>();
        uniqueId = this.gameObject.GetComponent<UniqueID>();
        id = uniqueId.guid;
    }

    // Invoked every updateTimeInSeconds seconds / Time.timeScale
    private void UpdateInterval()
    {
        // Consume food
        suppliesFood = (int)Mathf.Round(suppliesFood - (population / (GameManager.dayLengthInSeconds / updateTimeInSeconds)));
        if (suppliesFood < population * 3)
        {
            // SettlementLowFood(id);
        }

        // Produce money
        productionMoney = (int)Mathf.Round((population * district.controllingFaction.policies.popTaxRate) / (GameManager.dayLengthInSeconds / updateTimeInSeconds));
        GameEvents.current.AlterFactionResourceVal(district.controllingFaction, "money", productionMoney);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void SettlementLowFood(SettlementController id)
    {
        GameEvents.current.SettlementLowFood(id);
    }

    private void OnSettlementDispatchFood(SettlementController settlement, int food)
    {
        if (settlement == this)
        {
            suppliesFood -= food;
            Debug.Log("Settlement: " + name + " Dispatched " + food + " food supplies");
        }
    }

    private void OnSettlementRecieveFood(SettlementController settlement, int food)
    {
        if (settlement == this)
        {
            suppliesFood += food;
            Debug.Log("Settlement: " + name + " Recieved " + food + " food supplies");
        }
    }
}