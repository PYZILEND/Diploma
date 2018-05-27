using UnityEngine;

/// <summary>
/// This metrics are used to compute distances between hexagons
/// </summary>
public static class HexMetrics
{
    /// <summary>
    /// Distance from cell's center to cell's vertex
    /// </summary>
    public const float outerRadius = 10f;

    /// <summary>
    /// Distance from cell's center to cell's edge
    /// </summary>
    public const float innerRadius = outerRadius * 0.866025404f;


    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    /// <summary>
    /// Positions of cell's vertices relative to it's center.
    /// One extra position to be used in cycles
    /// </summary>
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
            0.5f * blendFactor;
    }

    public static Vector3 GetRoadVertice(HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.NE: return new Vector3(2.12832f, 0f, GetFirstSolidCorner(HexDirection.NE).z/2); 
            case HexDirection.E: return new Vector3(4.31416f, 0f, 0); 
            case HexDirection.SE:  return new Vector3(2.12832f, 0f, GetFirstSolidCorner(HexDirection.SW).z / 2); 
            case HexDirection.SW: return new Vector3(-2.12832f, 0f, GetFirstSolidCorner(HexDirection.SW).z / 2); 
            case HexDirection.W: return new Vector3(-4.31416f, 0f, 0); 
            case HexDirection.NW: return new Vector3(-2.12832f, 0f, GetFirstSolidCorner(HexDirection.NE).z / 2); 
            default: return new Vector3(-1, -1, -1); 
        }
    }

}
