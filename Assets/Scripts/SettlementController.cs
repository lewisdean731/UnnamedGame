using UnityEngine;

public class SettlementController : MonoBehaviour
{
    public int updateTimeInSeconds = 5;
    public SettlementData data;
    public DistrictController district;

    private int population;

    private int suppliesAmmo;
    private int suppliesFood;
    private int suppliesFuel;
    private int suppliesMaterials;

    private int productionMoney;

    private int morale;
    private bool playerControlled;

    // Special buildings?
    private int policePresenceLevel;
    private int armyPresenceLevel;
    private int hospitalLevel;
    private int schoolLevel;

    private string[] tags;

    // Start is called before the first frame update
    void Start()
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

    void LoadSettlement(SettlementData data)
    {
        Debug.Log("Loading Settlement: " + data.name);

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
    void UpdateInterval()
    {

        Debug.Log("SETTDISTCONTROLLER " + district.controllingFaction);


        // Consume food
        suppliesFood = (int)Mathf.Round(suppliesFood - (population / (GameManager.dayLengthInSeconds / updateTimeInSeconds)));
        if (suppliesFood < population * 3)
        {
            SettlementLowFood(data.id);
        }
        Debug.Log("Updated " + data.name + " food supplies: " + suppliesFood);

        // Produce money
        productionMoney = (int)Mathf.Round((population * district.controllingFaction.policies.popTaxRate) / (GameManager.dayLengthInSeconds / updateTimeInSeconds));
        GameEvents.current.AlterFactionResourceVal(district.controllingFaction, "money", productionMoney);
    }


    // Update is called once per frame
    void Update()
    {

    }

    void SettlementLowFood(int id)
    {
        GameEvents.current.SettlementLowFood(id);
    }

    void OnSettlementDispatchFood(int id, int food)
    {
        if (id == data.id)
        {
            suppliesFood -= food;
            Debug.Log("Settlement: " + data.name + " Dispatched " + food + " food supplies");

        }
    }

    void OnSettlementRecieveFood(int id, int food)
    {
        if (id == data.id)
        {
            suppliesFood += food;
            Debug.Log("Settlement: " + data.name + " Recieved " + food + " food supplies");

        }
    }
}
