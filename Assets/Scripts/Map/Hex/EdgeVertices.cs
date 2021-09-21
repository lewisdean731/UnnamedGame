using UnityEngine;

public struct EdgeVertices
{
    public Vector3 v1, v2;
    public Vector3[] vx;

    public EdgeVertices(Vector3 corner1, Vector3 corner2)
    {
        v1 = corner1;
        v2 = corner2;

        vx = new Vector3[HexMetrics.cellSubdivisons];

        for (int i = 0; i < HexMetrics.cellSubdivisons; i++)
        {
            vx[i] = Vector3.Lerp(v1, v2, (i + 1f) / (HexMetrics.cellSubdivisons + 1f));
        }
    }
}