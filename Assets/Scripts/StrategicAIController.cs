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

    private void OnSettlementLowFood(string id)
    {
        Debug.Log("Settlement " + id + " Reports low on food!");

        // Work out 

        StartCoroutine(testWaiter(id));
    }

    // For testing
    IEnumerator testWaiter(string id)
    {
        SettlementDispatchFood(id, 1000);

        yield return new WaitForSeconds(10);

        SettlementRecieveFood(id, 1000);
    }

    private void SettlementDispatchFood(string id, int food)
    {
        GameEvents.current.SettlementDispatchFood(id, 1000);
    }
    private void SettlementRecieveFood(string id, int food)
    {
        GameEvents.current.SettlementRecieveFood(id, 1000);
    }

    // Farm
    private void OnFarmReadyToDispatchFood(FarmController farm, int food)
    {
        GameEvents.current.FarmDispatchFood(farm, food);

        // TODO Spawn a truck with 1000 food with distination of nearest settlement

    }
}
