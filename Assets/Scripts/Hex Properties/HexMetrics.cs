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
}
