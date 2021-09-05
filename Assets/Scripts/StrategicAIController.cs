using System.Collections;
using System.Linq;
using UnityEngine;

public class StrategicAIController : MonoBehaviour
{

    string[] controlledSettlements = { };

    // Start is called before the first frame update
    private void Start()
    {
        GameEvents.current.onSettlementLowFood += OnSettlementLowFood;
        GameEvents.current.onFarmReadyToDispatchFood += OnFarmReadyToDispatchFood;
    }

    private void OnDestroy()
    {
        GameEvents.current.onSettlementLowFood -= OnSettlementLowFood;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnSettlementLowFood(SettlementController settlement)
    {
        Debug.Log("Settlement " + settlement + " Reports low on food!");

        // Work out 

        StartCoroutine(testWaiter(settlement));
    }

    // For testing
    IEnumerator testWaiter(SettlementController settlement)
    {
        SettlementDispatchFood(settlement, 1000);

        yield return new WaitForSeconds(10);

        SettlementRecieveFood(settlement, 1000);
    }

    private void SettlementDispatchFood(SettlementController settlement, int food)
    {
        GameEvents.current.SettlementDispatchFood(settlement, 1000);
    }
    private void SettlementRecieveFood(SettlementController settlement, int food)
    {
        GameEvents.current.SettlementRecieveFood(settlement, 1000);
    }

    // Farm
    private void OnFarmReadyToDispatchFood(FarmController farm, int food)
    {
        GameEvents.current.FarmDispatchFood(farm, food);

        // TODO Spawn a truck with 1000 food with distination of nearest settlement
        Transform nearestSettlement = FinderUtil.FindNearestControlledSettlement(farm.gameObject, farm.district.controllingFaction);

        // Until resource transporting is a thing
        SettlementController sc = nearestSettlement.gameObject.GetComponent<SettlementController>();
        GameEvents.current.SettlementRecieveFood(sc, food);
    }
}
