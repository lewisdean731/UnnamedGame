using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    private Mesh hexMesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Color> colors;

    private MeshCollider meshCollider;

    private void Awake()
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

    private void TriangulateCell(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            TriangulateCellPart(d, cell);
        }
    }

    private void TriangulateCellPart(HexDirection direction, HexCell cell)
    {
        // Create solid edge vertices
        Vector3 center = cell.transform.localPosition;
        EdgeVertices hev1 = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );

        // Subdivisions
        Vector3[] subdivisons = new Vector3[HexMetrics.cellSubdivisons];
        if (HexMetrics.cellSubdivisons > 0)
        {
            AddTriangle(center, hev1.v1, (HexMetrics.cellSubdivisons > 0 ? hev1.vx[0] : hev1.v2));
            AddTriangleColor(cell.color);

            for (int i = 0; i < HexMetrics.cellSubdivisons; i++)
            {
                if (i + 1 < HexMetrics.cellSubdivisons)
                {
                    AddTriangle(center, hev1.vx[i], hev1.vx[i + 1]);
                    AddTriangleColor(cell.color);
                }
                else
                {
                    AddTriangle(center, hev1.vx[i], hev1.v2);
                    AddTriangleColor(cell.color);
                }
            }
        }
        else
        {
            AddTriangle(center, hev1.v1, hev1.v2);
            AddTriangleColor(cell.color);
        }

        // Triangulate cell connections on NE, E, SE directions
        if (direction <= HexDirection.SE)
        {
            TriangulateCellConnection(direction, cell, hev1);
        }
    }

    private void TriangulateCellConnection(
        HexDirection direction, HexCell cell, EdgeVertices hev1
    )
    {
        // Don't make connections to non-existent neighbors
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.Position.y - cell.Position.y;
        EdgeVertices hev2 = new EdgeVertices(
            hev1.v1 + bridge,
            hev1.v2 + bridge
        );

        TriangulateCellBridge(direction, cell, hev1, neighbor, hev2);
        TriangulateCellCorner(direction, cell, hev1, neighbor, hev2);
    }

    private void TriangulateCellBridge(
        HexDirection direction, 
        HexCell cell,
        EdgeVertices hev1,
        HexCell neighbor,
        EdgeVertices hev2
    )
    {
        if (cell.GetEdgeType(direction) == HexEdgeType.SLOPE_TERRACE)
        {
            TriangulateBridgeTerraces(hev1.v1, hev1.v2, cell, hev2.v1, hev2.v2, neighbor);
        }
        else
        {
            // Subdivisions
            Vector3[,] subdivisonQuads = new Vector3[HexMetrics.cellSubdivisons, 2];

            if (HexMetrics.cellSubdivisons > 0)
            {
                subdivisonQuads[0, 0] = Vector3.Lerp(hev2.v1, hev2.v2, 1f / (HexMetrics.cellSubdivisons + 1f));
                subdivisonQuads[0, 1] = Vector3.Lerp(hev2.v1, hev2.v2, 2f / (HexMetrics.cellSubdivisons + 1f));

                AddQuad(hev1.v1, hev1.vx[0], hev2.v1, subdivisonQuads[0, 0]);
                AddQuadColorTwoWayBlend(cell.color, neighbor.color);

                for (int i = 1; i < HexMetrics.cellSubdivisons; i++)
                {
                    subdivisonQuads[i, 0] = Vector3.Lerp(hev2.v1, hev2.v2, (i + 1f) / (HexMetrics.cellSubdivisons + 1f));
                    subdivisonQuads[i, 1] = Vector3.Lerp(hev2.v1, hev2.v2, (i + 1f) / (HexMetrics.cellSubdivisons + 1f));
                }
                for (int i = 0; i < HexMetrics.cellSubdivisons; i++)
                {
                    if (i + 1 < HexMetrics.cellSubdivisons)
                    {
                        AddQuad(hev1.vx[i], hev1.vx[i + 1], subdivisonQuads[i, 0], subdivisonQuads[i + 1, 1]);
                        AddQuadColorTwoWayBlend(cell.color, neighbor.color);
                    }
                    else if (i + 1 == HexMetrics.cellSubdivisons && HexMetrics.cellSubdivisons == 1) // Edge case for subdivs = 1
                    {
                        AddQuad(hev1.vx[i], hev1.v2, subdivisonQuads[i, 0], subdivisonQuads[i, 1]);
                        AddQuadColorTwoWayBlend(cell.color, neighbor.color);
                    }
                    else
                    {
                        AddQuad(hev1.vx[i], hev1.v2, subdivisonQuads[i, 1], hev2.v2);
                        AddQuadColorTwoWayBlend(cell.color, neighbor.color);
                    }
                }
            }
            else
            {
                AddQuad(hev1.v1, hev1.v2, hev2.v1, hev2.v2);
                AddQuadColorTwoWayBlend(cell.color, neighbor.color);
            }
        }
    }

    private void TriangulateCellCorner(HexDirection direction, HexCell cell, EdgeVertices hev1, HexCell neighbor, EdgeVertices hev2)
    {
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = hev1.v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Position.y;

            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(hev1.v2, cell, hev2.v2, neighbor, v5, nextNeighbor);
                }
                else
                {
                    // Calculate counterclockwise to maintain orientation
                    TriangulateCorner(v5, nextNeighbor, hev1.v2, cell, hev2.v2, neighbor);
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(hev2.v2, neighbor, v5, nextNeighbor, hev1.v2, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, hev1.v2, cell, hev2.v2, neighbor);
            }
        }
    }

    private void TriangulateBridgeTerraces(
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

    private void TriangulateCorner(
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

    private void TriangulateCornerTerraces(
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

    private void TriangulateCornerTerracesCliff(
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

    private void TriangulateCornerCliffTerraces(
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

    private void TriangulateBoundaryTriangle(
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

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    private void AddTriangleColorPerVertex(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
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

    private void AddQuadColorTwoWayBlend(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

    private void AddQuadColorPerVertex(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    private Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        // We di *2f-1f to allow for positive or negative variations
        position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
        //position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
        return position;
    }
}