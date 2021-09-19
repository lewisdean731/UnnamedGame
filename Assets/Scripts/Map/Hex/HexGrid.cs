using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
	HexCell[] cells;

	public Text cellLabelPrefab;

	Canvas gridCanvas;

	HexMesh hexMesh;

	public Color defaultColor = Color.white;
	public Color selectedColor = Color.green;


	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();


		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	void Start()
	{
		GameEvents.current.onSelectCell += OnSelectCell;
		hexMesh.Triangulate(cells);
	}

	private void OnDestroy()
	{
		GameEvents.current.onSelectCell -= OnSelectCell;
	}

	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate(cellPrefab);
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;

		cell.color = defaultColor;

		// Set Neighbour Directions
		if (x > 0)
		{
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0)
		{
			if ((z & 1) == 0)
			{
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else
			{
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1)
				{
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
			}
		}

		// Set Cell Text
		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();

		cell.uiRect = label.rectTransform;
	}

	public HexCell GetCell(Vector3 position)
    {
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		return cells[index];
	}

	void OnSelectCell(Vector3 position)
	{
		HexCell cell = GetCell(position);
		if(cell != null)
        {
			GameEvents.current.CellSelected(cell);
        }
	}

	public void ColorCell(HexCell cell, Color color)
	{
		cell.color = color;
	}

	public void Refresh()
	{
		hexMesh.Triangulate(cells);
	}
}
