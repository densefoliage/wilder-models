using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class CoordinateTools
{
    public static OffsetCoordinate CubeToOffsetOddQ(CubeCoordinate cube)
    {
        int x = cube.Q;
        int y = cube.R + (cube.Q - (cube.Q&1)) / 2;
        return new OffsetCoordinate(x, y);
    }
    public static CubeCoordinate OffsetOddQToCube(OffsetCoordinate offset)
    {
        int q = offset.X;
        int r = offset.Y - (offset.X - (offset.X&1)) / 2;
        int s = -q-r;
        return new CubeCoordinate(q,r,s);
    }

    public static CubeCoordinate AddCoordinates(CubeCoordinate a, CubeCoordinate b)
    {
        return new CubeCoordinate(
            a.Q + b.Q,
            a.R + b.R,
            a.S + b.S
        );
    }

    static CubeCoordinate[] Directions = new CubeCoordinate[]
    {
        new CubeCoordinate(+1, -1, 0),
        new CubeCoordinate(+1, 0, -1),
        new CubeCoordinate(0, +1, -1),
        new CubeCoordinate(-1, +1, 0),
        new CubeCoordinate(-1, 0, +1),
        new CubeCoordinate(0, -1, +1)
    };

    public static CubeCoordinate GetDirectionCoordinate(int direction)
    {
        return Directions[direction];
    }

    public static CubeCoordinate GetNeighbourCoordinate(CubeCoordinate a, int direction)
    {
        return AddCoordinates(a, GetDirectionCoordinate(direction));
    }

    public static CubeCoordinate[] GetAllNeighbourCoordinates(CubeCoordinate a)
    {
        List<CubeCoordinate> results = new List<CubeCoordinate>();
        foreach (CubeCoordinate direction in Directions)
        {
            results.Add(AddCoordinates(a, direction));
        }
        return results.ToArray();
    }

    static CubeCoordinate[] DiagonalDirections = new CubeCoordinate[]
    {
        new CubeCoordinate(+2, -1, -1),
        new CubeCoordinate(+1, +1, -2),
        new CubeCoordinate(-1, +2, -1),
        new CubeCoordinate(-2, +1, +1),
        new CubeCoordinate(-1, -1, +2),
        new CubeCoordinate(+1, -2, +1)
    };

    public static CubeCoordinate GetDiagonalDirectionCoordinate(int direction)
    {
        return Directions[direction];
    }

    public static CubeCoordinate GetDiagonalNeighbourCoordinate(CubeCoordinate a, int direction)
    {
        return AddCoordinates(a, GetDiagonalDirectionCoordinate(direction));
    }

    public static CubeCoordinate[] GetAllDiagonalNeighbourCoordinates(CubeCoordinate a)
    {
        List<CubeCoordinate> results = new List<CubeCoordinate>();
        foreach (CubeCoordinate direction in DiagonalDirections)
        {
            results.Add(AddCoordinates(a, direction));
        }
        return results.ToArray();
    }

    public static int DistanceBetweenCoordinates(CubeCoordinate a, CubeCoordinate b) {
        return (Mathf.Abs(a.Q - b.Q) + Mathf.Abs(a.R - b.R)+ Mathf.Abs(a.S - b.S)) / 2;
    }

    // public static int DirectionBetweenCoordinates(CubeCoordinate a, CubeCoordinate b) {
    //     /*
    //     This doesn't seem to be working yet, and I'm not sure why...
    //     can also use the max of abs(dx-dy), abs(dy-dz), abs(dz-dx) to figure out
    //     which of the 6 “wedges” a hex is in...
    //     https://www.redblobgames.com/grids/hexagons/#distances-cube 
    //     */
    //     int dq = Mathf.Abs(a.Q - b.Q);
    //     int dr = Mathf.Abs(a.R - b.R);
    //     int ds = Mathf.Abs(a.S - b.S);
    //     return Mathf.Max(Mathf.Abs(dq-ds), Mathf.Abs(ds-dr), Mathf.Abs(dr-dq));
    // }

    public static CubeCoordinate[] GetCoordinatesWithinRangeOf(CubeCoordinate a, int range)
    {
        List<CubeCoordinate> results = new List<CubeCoordinate>();
        for (int q = -range; q <= range; q++)
        {
            for (int s = Mathf.Max(-range, -q-range); s <= Mathf.Min(range, -q+range); s++)
            {
                    int r = -q-s;
                    results.Add(AddCoordinates(a, new CubeCoordinate(q,r,s)));
            }
        }
        return results.ToArray();
    }

}