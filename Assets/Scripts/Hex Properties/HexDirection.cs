using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection NeighborLeft(this HexDirection direction)
    {
        return (int)direction == 0 ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection NeighborRight(this HexDirection direction)
    {
        return (int)direction == 5 ? HexDirection.NE : (direction + 1);
    }

    public static HexDirection OppositeNeighborLeft(this HexDirection direction)
    {
        return Opposite(NeighborLeft(direction));
    }

    public static HexDirection OppositeNeighborRight(this HexDirection direction)
    {
        return Opposite(NeighborRight(direction));
    }
}