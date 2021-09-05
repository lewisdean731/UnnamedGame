using UnityEngine;

[CreateAssetMenu(fileName = "New District", menuName = "Game/Districts/New District")]
public class District : ScriptableObject
{
    public new string name;
    public int id;
    public string description;
    public Faction controllingFaction;
    public Faction[] contestingFactions;
    public bool contested;
    public Vector3[] borders;
    public int moneyIncome = 0, ammunitionIncome = 0, fuelIncome = 0, materialsIncome = 0, foodIncome = 0;
    public string[] tags;

}