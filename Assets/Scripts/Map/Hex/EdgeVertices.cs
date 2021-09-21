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

    public static EdgeVertices TerraceLerp(
    EdgeVertices a, EdgeVertices b, int step)
    {
        EdgeVertices result;
        result.v1 = HexMetrics.TerraceLerpBetweenPoints(a.v1, b.v1, step);
        result.v2 = HexMetrics.TerraceLerpBetweenPoints(a.v2, b.v2, step);
        result.vx = new Vector3[HexMetrics.cellSubdivisons];

        for (int i = 0; i < HexMetrics.cellSubdivisons; i++)
        {
            result.vx[i] = HexMetrics.TerraceLerpBetweenPoints(a.vx[i], b.vx[i], step);
        }
        return result;
    }
}