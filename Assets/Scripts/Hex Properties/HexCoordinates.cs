using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This structure handles hexagonal coordinates.
/// Default coordinates for a hexagon are cube coordinates,
/// This struct also contains methods for converting other
/// coordinate types into cube coordinates
/// </summary>
[System.Serializable]  
public struct HexCoordinates
{

    public int X { get; private set; }
    public int Z { get; private set; }
    public int Y { get { return -X - Z; } }

    /// <summary>
    /// Primary constructor.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public HexCoordinates(int x, int z)
    {
        X = x;
        Z = z;
    }

    /// <summary>
    /// Creates cube coordinates set from offset coordinates
    /// </summary>
    public static HexCoordinates fromOffsetCoordinates(int x, int z)
	{
        return new HexCoordinates(x - z / 2, z);
    }

    /// <summary>
    /// Creates cube coordinates set from worldspace position
    /// </summary>
    public static HexCoordinates fromPosition(Vector3 position)
    {
        //Dividing position's X coordinate by the width of a single hexagon
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x; // Y coordinate is always -X if Z = 0
        
        //Applying Z coordinate offset
        //e.g, x=1.8 y=-1.8 offset=0.8
        //then x=1.0 y=-2.4
        //making x=1 y=-2 z=1
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        //Making sure the rounding was done correct
        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }

    /// <summary>
    /// Returns X,Y and Z coordinates separated by space
    /// </summary>
    /// <returns></returns>
    override public string ToString()
    {
        return X.ToString() + " " + Y.ToString() + " " + Z.ToString();
    }

    /// <summary>
    /// Returns X,Y and Z coordinates each on a new line
    /// </summary>
    /// <returns></returns>
    public string ToStringOnMultipleLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }
}
