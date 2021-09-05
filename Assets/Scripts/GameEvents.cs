using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{

    public static GameEvents current;


    public void Awake()
    {
        current = this;
    }

    // Input

    public event Action onMouse0Down;
    public void Mouse0Down()
    {
        if (onMouse0Down != null)
        {
            onMouse0Down();
        }
    }

    public event Action onMouse1Down;
    public void Mouse1Down()
    {
        if (onMouse1Down != null)
        {
            onMouse1Down();
        }
    }

    // Settlement

    public event Action<SettlementController> onSettlementLowFood;
    public void SettlementLowFood(SettlementController settlement)
    {
        if (onSettlementLowFood != null)
        {
            onSettlementLowFood(settlement);
        }
    }

    public event Action<SettlementController, int> onSettlementDispatchFood;
    public void SettlementDispatchFood(SettlementController settlement, int food)
    {
        if (onSettlementDispatchFood != null)
        {
            onSettlementDispatchFood(settlement, food);
        }
    }

    public event Action<SettlementController, int> onSettlementRecieveFood;
    public void SettlementRecieveFood(SettlementController settlement, int food)
    {
        if (onSettlementRecieveFood != null)
        {
            onSettlementRecieveFood(settlement, food);
        }
    }

    // Farm
    public event Action<FarmController, int> onFarmReadyToDispatchFood;

    public void FarmReadyToDispatchFood(FarmController farm, int food)
    {
        if (onFarmReadyToDispatchFood != null)
        {
            onFarmReadyToDispatchFood(farm, food);
        }
    }

    public event Action<FarmController, int> onFarmDispatchFood;

    public void FarmDispatchFood(FarmController farm, int food)
    {
        if (onFarmDispatchFood != null)
        {
            onFarmDispatchFood(farm, food);
        }
    }

    // Resources

    public event Action<Faction, string, int> onAlterFactionResourceVal;
    public void AlterFactionResourceVal(Faction faction, string resource, int amount)
    {
        if (onAlterFactionResourceVal != null)
        {
            onAlterFactionResourceVal(faction, resource, amount);
        }
    }


}
