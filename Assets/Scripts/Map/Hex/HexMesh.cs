using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	List<Color> colors;

	MeshCollider meshCollider;

	void Awake()
	{
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();

		meshCollider = gameObject.AddComponent<MeshCollider>();

	}

    private void Start()
    {
		gameObject.tag = "TERRAIN";
    }

    public void Triangulate(HexCell[] cells)
	{
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();
		for (int i = 0; i < cells.Length; i++)
		{
			TriangulateCell(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.RecalculateNormals();
		meshCollider.sharedMesh = hexMesh;
	}

	void TriangulateCell(HexCell cell)
	{
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			TriangulateCellPart(d, cell);
		}
	}

	void TriangulateCellPart(HexDirection direction, HexCell cell)
    {
		Vector3 center = cell.transform.localPosition;
		AddTriangle(
			center,
			center + HexMetrics.GetFirstCorner(direction),
			center + HexMetrics.GetSecondCorner(direction)
		);

		// Set Colours
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell; // Use ourselves if no neighbour in direction e.g. border cell
		Color edgeColor = (cell.color + neighbor.color) * 0.5f;
		AddTriangleColorPerVertex(cell.color, edgeColor, edgeColor);
		// AddTriangleColor(cell.color);
	}

	void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	void AddTriangleColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	void AddTriangleColorPerVertex(Color c1, Color c2, Color c3)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

}