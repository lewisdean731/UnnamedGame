using UnityEngine;

public class FarmController : MonoBehaviour
{
    public DistrictController district;

    public UniqueID uniqueId;
    private string id;
    [Range(1, 5)]
    public int level = 1;
    [Range(0f, 1f)]
    public float health = 1f;

    public int suppliesAmmo, suppliesFood, suppliesFuel, suppliesMaterial = 0;
    public int suppliesAmmoCap, suppliesFoodCap, suppliesFuelCap, suppliesMaterialCap = 0;

    // Start is called before the first frame update
    void Start()
    {
        LoadFarm();

        GameEvents.current.onFarmDispatchFood += OnFarmDispatchFood;

        InvokeRepeating("UpdateInterval", GameManager.updateIntervalFarm, GameManager.updateIntervalFarm);

    }

    private void OnDestroy()
    {
        GameEvents.current.onFarmDispatchFood -= OnFarmDispatchFood;
    }

    void LoadFarm()
    {
        district = transform.parent.GetComponent<DistrictController>();

        this.gameObject.AddComponent<UniqueID>();
        uniqueId = this.gameObject.GetComponent<UniqueID>();
        id = uniqueId.guid;

        suppliesFoodCap = 1000;
    }

    // Invoked every updateTimeInSeconds seconds / Time.timeScale
    void UpdateInterval()
    {
        // Produce food
        suppliesFood += (int)Mathf.Round((level * 100 * health) / (GameManager.dayLengthInSeconds / GameManager.updateIntervalFarm));
        if(suppliesFood > suppliesFoodCap)
        {
            suppliesFood = suppliesFoodCap;
        }

        // When farm >= 1000 food dispatch it to nearest settlement
        if( suppliesFood >= 1000)
        {
            GameEvents.current.FarmReadyToDispatchFood(this, suppliesFood);
        }
    }

    void OnFarmDispatchFood(FarmController farm, int food)
    {
        if(farm == this)
        {
            suppliesFood -= food;
        }
    }
}
