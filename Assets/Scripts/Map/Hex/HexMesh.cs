using UnityEngine;
using System.Collections.Generic;
using System;

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
		// Create solid centre
		Vector3 center = cell.transform.localPosition;
		Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

		// Subdivisions
		Vector3[] subdivisons = new Vector3[HexMetrics.cellSubdivisons];
		if (HexMetrics.cellSubdivisons > 0)
		{
			subdivisons[0] = Vector3.Lerp(v1, v2, 1f / (HexMetrics.cellSubdivisons + 1f));

			AddTriangle(center, v1, (HexMetrics.cellSubdivisons > 0 ? subdivisons[0] : v2));
			AddTriangleColor(cell.color);

			for (int i = 1; i < HexMetrics.cellSubdivisons; i++)
			{
				subdivisons[i] = Vector3.Lerp(v1, v2, (i + 1f) / (HexMetrics.cellSubdivisons + 1f));
			}
			for (int i = 0; i < HexMetrics.cellSubdivisons; i++)
			{
				if (i + 1 < HexMetrics.cellSubdivisons)
				{
					AddTriangle(center, subdivisons[i], subdivisons[i + 1]);
					AddTriangleColor(cell.color);
				}
				else
				{
					AddTriangle(center, subdivisons[i], v2);
					AddTriangleColor(cell.color);
				}
			}
		}
		else
		{
			AddTriangle(center, v1, v2);
			AddTriangleColor(cell.color);
		}

		if (direction <= HexDirection.SE)
		{
			TriangulateCellPartConnection(direction, cell, v1, subdivisons, v2);
		}
	}

	void TriangulateCellPartConnection(
		HexDirection direction, HexCell cell, Vector3 v1, Vector3[] subdivisions, Vector3 v2
	)
	{
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null)
		{
			return;
		}

		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;
		v3.y = v4.y = neighbor.Position.y;

		// Subdivisions
		Vector3[,] subdivisonQuads = new Vector3[HexMetrics.cellSubdivisons , 2];

		Vector3 e3 = Vector3.Lerp(v3, v4, 1f / 3f);
		Vector3 e4 = Vector3.Lerp(v3, v4, 2f / 3f);


		if (HexMetrics.cellSubdivisons > 0)
		{
			subdivisonQuads[0, 0] = Vector3.Lerp(v3, v4, 1f / (HexMetrics.cellSubdivisons + 1f));
			subdivisonQuads[0, 1] = Vector3.Lerp(v3, v4, 2f / (HexMetrics.cellSubdivisons + 1f));

			AddQuad(v1, subdivisions[0], v3, subdivisonQuads[0, 0]);
			AddQuadColorTwoWayBlend(cell.color, neighbor.color);

			for (int i = 1; i < HexMetrics.cellSubdivisons; i++)
			{
				subdivisonQuads[i, 0] = Vector3.Lerp(v3, v4, (i + 1f) / (HexMetrics.cellSubdivisons + 1f));
				subdivisonQuads[i, 1] = Vector3.Lerp(v3, v4, (i + 1f) / (HexMetrics.cellSubdivisons + 1f));
			}
			for (int i = 0; i < HexMetrics.cellSubdivisons; i++)
			{
				if (i + 1 < HexMetrics.cellSubdivisons)
				{
					AddQuad(subdivisions[i], subdivisions[i+1], subdivisonQuads[i, 0], subdivisonQuads[i+1, 1]);
					AddQuadColorTwoWayBlend(cell.color, neighbor.color);
				}
				else
				{
					AddQuad(subdivisions[i], v2, subdivisonQuads[0, 1], v4);
					AddQuadColorTwoWayBlend(cell.color, neighbor.color);
				}
			}
		}
		else
		{
			AddQuad(v1, v2, v3, v4);
			AddQuadColorTwoWayBlend(cell.color, neighbor.color);
		}





		if (cell.GetEdgeType(direction) == HexEdgeType.SLOPE_TERRACE)
        {
			TriangulateBridgeTerraces(v1, v2, cell, v3, v4, neighbor);
        } 
		else
        {
			//AddQuad(v1, v2, v3, v4);
			//AddQuadColorTwoWayBlend(cell.color, neighbor.color);
        }

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null)
		{
			Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Position.y;

			if (cell.Elevation <= neighbor.Elevation)
			{
				if (cell.Elevation <= nextNeighbor.Elevation)
				{
					TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
				}
				else
				{
					// Calculate counterclockwise to maintain orientation
					TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation)
			{
				TriangulateCorner(v4, neighbor, v5, nextNeighbor, v2, cell);
			}
			else
			{
				TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
			}

		}
	}

	void TriangulateBridgeTerraces(
		Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
		Vector3 endLeft, Vector3 endRight, HexCell endCell
	)
	{
		Vector3 v3 = HexMetrics.TerraceLerpBetweenPoints(beginLeft, endLeft, 1);
		Vector3 v4 = HexMetrics.TerraceLerpBetweenPoints(beginRight, endRight, 1);
		Color c2 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, endCell.color, 1);

		AddQuad(beginLeft, beginRight, v3, v4);
		AddQuadColorTwoWayBlend(beginCell.color, c2);

		for (int i = 2; i < HexMetrics.terraceSteps; i++)
		{
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c2;
			v3 = HexMetrics.TerraceLerpBetweenPoints(beginLeft, endLeft, i);
			v4 = HexMetrics.TerraceLerpBetweenPoints(beginRight, endRight, i);
			c2 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, endCell.color, i);
			AddQuad(v1, v2, v3, v4);
			AddQuadColorTwoWayBlend(c1, c2);
		}

		AddQuad(v3, v4, endLeft, endRight);
		AddQuadColorTwoWayBlend(c2, endCell.color);
	}

	void TriangulateCorner(
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	)
	{
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.SLOPE_TERRACE)
		{
			if (rightEdgeType == HexEdgeType.SLOPE_TERRACE)
			{
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
			else if (rightEdgeType == HexEdgeType.FLAT)
			{
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
			else
			{
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (rightEdgeType == HexEdgeType.SLOPE_TERRACE)
		{
			if (leftEdgeType == HexEdgeType.FLAT)
			{
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else
			{
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.SLOPE_TERRACE)
		{
			if (leftCell.Elevation < rightCell.Elevation)
			{
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else
			{
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
		}
		else
		{
			AddTriangle(bottom, left, right);
			AddTriangleColorPerVertex(bottomCell.color, leftCell.color, rightCell.color);
		}
	}

	void TriangulateCornerTerraces(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	)
	{
		Vector3 v3 = HexMetrics.TerraceLerpBetweenPoints(begin, left, 1);
		Vector3 v4 = HexMetrics.TerraceLerpBetweenPoints(begin, right, 1);
		Color c3 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, leftCell.color, 1);
		Color c4 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, rightCell.color, 1);

		AddTriangle(begin, v3, v4);
		AddTriangleColorPerVertex(beginCell.color, c3, c4);

		for (int i = 2; i < HexMetrics.terraceSteps; i++)
		{
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c3;
			Color c2 = c4;
			v3 = HexMetrics.TerraceLerpBetweenPoints(begin, left, i);
			v4 = HexMetrics.TerraceLerpBetweenPoints(begin, right, i);
			c3 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, leftCell.color, i);
			c4 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, rightCell.color, i);
			AddQuad(v1, v2, v3, v4);
			AddQuadColorPerVertex(c1, c2, c3, c4);
		}

		AddQuad(v3, v4, left, right);
		AddQuadColorPerVertex(c3, c4, leftCell.color, rightCell.color);
	}

	void TriangulateCornerTerracesCliff(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
)
	{
		float b = Math.Abs(1f / (rightCell.Elevation - beginCell.Elevation));
		Vector3 boundary = Vector3.Lerp(begin, right, b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		TriangulateBoundaryTriangle(
			begin, beginCell, left, leftCell, boundary, boundaryColor
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.SLOPE_TERRACE)
		{
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else
		{
			AddTriangle(left, right, boundary);
			AddTriangleColorPerVertex(leftCell.color, rightCell.color, boundaryColor);
		}
	}

	void TriangulateCornerCliffTerraces(
	Vector3 begin, HexCell beginCell,
	Vector3 left, HexCell leftCell,
	Vector3 right, HexCell rightCell
)
	{
		float b = Math.Abs(1f / (leftCell.Elevation - beginCell.Elevation));

		Vector3 boundary = Vector3.Lerp(begin, left, b);
		Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

		TriangulateBoundaryTriangle(
			right, rightCell, begin, beginCell, boundary, boundaryColor
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.SLOPE_TERRACE)
		{
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else
		{
			AddTriangle(left, right, boundary);
			AddTriangleColorPerVertex(leftCell.color, rightCell.color, boundaryColor);
		}
	}

	void TriangulateBoundaryTriangle(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor
	)
	{
		Vector3 v2 = HexMetrics.TerraceLerpBetweenPoints(begin, left, 1);
		Color c2 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, leftCell.color, 1);

		AddTriangle(begin, v2, boundary);
		AddTriangleColorPerVertex(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++)
		{
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexMetrics.TerraceLerpBetweenPoints(begin, left, i);
			c2 = HexMetrics.TerraceLerpBetweenColors(beginCell.color, leftCell.color, i);
			AddTriangle(v1, v2, boundary);
			AddTriangleColorPerVertex(c1, c2, boundaryColor);
		}

		AddTriangle(v2, left, boundary);
		AddTriangleColorPerVertex(c2, leftCell.color, boundaryColor);
	}

	void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
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


	void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		vertices.Add(Perturb(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	void AddQuadColorTwoWayBlend(Color c1, Color c2)
	{
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}

	void AddQuadColorPerVertex(Color c1, Color c2, Color c3, Color c4)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}

	Vector3 Perturb(Vector3 position)
	{
		Vector4 sample = HexMetrics.SampleNoise(position);
		// We di *2f-1f to allow for positive or negative variations
		position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
		//position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
		return position;
	}

}