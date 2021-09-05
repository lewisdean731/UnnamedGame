using UnityEngine;

public class FactionResources : MonoBehaviour
{
    public Faction faction;
    public int money = 0, ammunition = 0, fuel = 0, materials = 0, food = 0;
    public int moneyCap = -1, ammunitionCap = 0, fuelCap = 0, materialsCap = 0, foodCap = 0;
    public int moneyIncome = 0, ammunitionIncome = 0, fuelIncome = 0, materialsIncome = 0, foodIncome = 0;

    // Start is called before the first frame update
    private void Start()
    {
        GameEvents.current.onAlterFactionResourceVal += OnAlterFactionResourceVal;
    }

    private void OnDestroy()
    {
        GameEvents.current.onAlterFactionResourceVal -= OnAlterFactionResourceVal;
    }

    void OnAlterFactionResourceVal(Faction eventFaction, string resource, int amount)
    {
        if (eventFaction == faction)
        {
            alterResourceVal(resource, amount);
        }
    }

    void alterResourceVal(string resource, int amount) //have one method to alter the value, just have to multiply decreases by -1 when passing it through
    {
        switch (resource)
        {
            case "money":
                money += amount;
                break;
            case "ammunition":
                ammunition += amount;
                break;
            case "fuel":
                fuel += amount;
                break;
            case "materials":
                materials += amount;
                break;
            case "food":
                food += amount;
                break;
            default:
                break;
        }
    }

}
