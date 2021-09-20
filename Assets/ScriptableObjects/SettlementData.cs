using UnityEngine;

[CreateAssetMenu(fileName = "New Settlement", menuName = "Game/Settlements/New Settlement")]
public class SettlementData : ScriptableObject
{
    public new string name;
    public int id;
    public District district;
    public string description;

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
}