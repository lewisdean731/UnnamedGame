using System.Collections.Generic;
using UnityEngine;

public class FactionTerritory : MonoBehaviour
{
    public List<GameObject> districts, contestedDistricts, regions, contestedRegions;

    private void Awake()
    {
        districts = new List<GameObject>();
        contestedDistricts = new List<GameObject>();
        regions = new List<GameObject>();
        contestedRegions = new List<GameObject>();
    }

    /*
	void addDistrict(GameObject district)
    {
		districts.Add(district);
	}

	void removeDistrict(GameObject district)
	{
		districts.Remove(district);
	}

	void addContestedDistrict(GameObject district)
	{
		contestedDistricts.Add(district);
	}

	void removeContestedDistrict(GameObject district)
	{
		contestedDistricts.Remove(district);
	}

	void addRegion(GameObject Region)
	{
		regions.Add(Region);
	}

	void removeRegion(GameObject Region)
	{
		regions.Remove(Region);
	}

	void addContestedRegion(GameObject Region)
	{
		contestedRegions.Add(Region);
	}

	void removeContestedRegion(GameObject Region)
	{
		contestedRegions.Remove(Region);
	}
	*/
}