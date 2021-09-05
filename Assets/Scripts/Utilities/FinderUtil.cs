using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinderUtil : MonoBehaviour
{
    public static Transform FindNearestControlledSettlement(GameObject g, Faction faction)
    {
        Transform closest = null;
        SettlementController[] settlements = FindObjectsOfType<SettlementController>();
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = g.transform.position;
        foreach (SettlementController settlement in settlements)
        {
            if(settlement.district.controllingFaction == faction)
            {
                Vector3 directionToTarget = settlement.gameObject.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closest = settlement.gameObject.transform;
                }
            }
        }
        Debug.Log("Closest Settlement to " + g + " is " + closest);
        return closest;

    }
}
