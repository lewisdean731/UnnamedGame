using UnityEngine;

public class SettlementController : MonoBehaviour
{
    public int updateTimeInSeconds = 5;
    public SettlementData data;
    public DistrictController district;

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

        if (data != null)
        {
            LoadSettlement(data);

            GameEvents.current.onSettlementDispatchFood += OnSettlementDispatchFood;
            GameEvents.current.onSettlementRecieveFood += OnSettlementRecieveFood;

            InvokeRepeating("UpdateInterval", GameManager.updateIntervalSettlement, GameManager.updateIntervalSettlement);
        }

    }

    private void OnDestroy()
    {
        GameEvents.current.onSettlementDispatchFood -= OnSettlementDispatchFood;
        GameEvents.current.onSettlementRecieveFood -= OnSettlementRecieveFood;
    }

    private void LoadSettlement(SettlementData data)
    {
        district = transform.parent.GetComponent<DistrictController>();

        population = data.population;
        morale = data.morale;
        playerControlled = data.playerControlled;

        // Supplies
        suppliesAmmo = data.suppliesAmmo;
        suppliesFood = data.suppliesFood;
        suppliesFuel = data.suppliesFuel;
        suppliesMaterials = data.suppliesMaterials;

        // Production
        productionMoney = data.productionMoney;

        // Special buildings?
        policePresenceLevel = data.policePresenceLevel;
        armyPresenceLevel = data.armyPresenceLevel;
        hospitalLevel = data.hospitalLevel;
        schoolLevel = data.schoolLevel;

        tags = data.tags;
    }

    // Invoked every updateTimeInSeconds seconds / Time.timeScale
    private void UpdateInterval()
    {

        // Consume food
        suppliesFood = (int)Mathf.Round(suppliesFood - (population / (GameManager.dayLengthInSeconds / updateTimeInSeconds)));
        if (suppliesFood < population * 3)
        {
            SettlementLowFood(data.id);
        }

        // Produce money
        productionMoney = (int)Mathf.Round((population * district.controllingFaction.policies.popTaxRate) / (GameManager.dayLengthInSeconds / updateTimeInSeconds));
        GameEvents.current.AlterFactionResourceVal(district.controllingFaction, "money", productionMoney);
    }


    // Update is called once per frame
    private void Update()
    {

    }

    private void SettlementLowFood(int id)
    {
        GameEvents.current.SettlementLowFood(id);
    }

    private void OnSettlementDispatchFood(int id, int food)
    {
        if (id == data.id)
        {
            suppliesFood -= food;
            Debug.Log("Settlement: " + data.name + " Dispatched " + food + " food supplies");

        }
    }

    private void OnSettlementRecieveFood(int id, int food)
    {
        if (id == data.id)
        {
            suppliesFood += food;
            Debug.Log("Settlement: " + data.name + " Recieved " + food + " food supplies");

        }
    }
}
