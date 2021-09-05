using System.Collections;
using System.Linq;
using UnityEngine;

public class StrategicAIController : MonoBehaviour
{

    int[] controlledSettlements = { 0 };

    // Start is called before the first frame update
    private void Start()
    {
        GameEvents.current.onSettlementLowFood += OnSettlementLowFood;
    }

    private void OnDestroy()
    {
        GameEvents.current.onSettlementLowFood -= OnSettlementLowFood;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnSettlementLowFood(int id)
    {
        if (controlledSettlements.Contains(id))
        {
            Debug.Log("Settlement " + id + " Reports low on food!");

            // Work out 

            StartCoroutine(testWaiter());

        }
    }

    // For testing
    IEnumerator testWaiter()
    {
        SettlementDispatchFood(0, 1000);

        yield return new WaitForSeconds(10);

        SettlementRecieveFood(0, 1000);
    }

    private void SettlementDispatchFood(int id, int food)
    {
        GameEvents.current.SettlementDispatchFood(id, 1000);
    }
    private void SettlementRecieveFood(int id, int food)
    {
        GameEvents.current.SettlementRecieveFood(id, 1000);
    }
}
