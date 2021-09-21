using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{

	public HexCell[] cells;

	HexMesh hexMesh;
	Canvas gridCanvas;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}

	void Start()
	{
		hexMesh.Triangulate(cells);
	}

	void LateUpdate()
	{
		hexMesh.Triangulate(cells);
		enabled = false;
	}

	public void AddCell(int index, HexCell cell)
	{
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}

	public void Refresh()
	{
		// Because HGM doesn't do anything else can use its enabled state to signal that an update is needed
		enabled = true;
	}
}