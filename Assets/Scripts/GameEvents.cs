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

    public event Action<string> onSettlementLowFood;
    public void SettlementLowFood(string id)
    {
        if (onSettlementLowFood != null)
        {
            onSettlementLowFood(id);
        }
    }

    public event Action<string, int> onSettlementDispatchFood;
    public void SettlementDispatchFood(string id, int food)
    {
        if (onSettlementDispatchFood != null)
        {
            onSettlementDispatchFood(id, food);
        }
    }

    public event Action<string, int> onSettlementRecieveFood;
    public void SettlementRecieveFood(string id, int food)
    {
        if (onSettlementRecieveFood != null)
        {
            onSettlementRecieveFood(id, food);
        }
    }

    // Farm
    public event Action<string, int> onFarmDispatchFood;

    public void FarmDispatchFood(string id, int food)
    {
        if (onFarmDispatchFood != null)
        {
            onFarmDispatchFood(id, food);
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
