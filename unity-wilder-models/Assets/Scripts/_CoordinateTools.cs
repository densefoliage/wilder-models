using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class _CoordinateTools
{
    public static _OffsetCoordinate CubeToOffsetOddQ(_CubeCoordinate cube)
    {
        int x = cube.Q;
        int y = cube.R + (cube.Q - (cube.Q&1)) / 2;
        return new _OffsetCoordinate(x, y);
    }
    public static _CubeCoordinate OffsetOddQToCube(_OffsetCoordinate offset)
    {
        int q = offset.X;
        int r = offset.Y - (offset.X - (offset.X&1)) / 2;
        int s = -q-r;
        return new _CubeCoordinate(q,r,s);
    }

    public static _CubeCoordinate AddCoordinates(_CubeCoordinate a, _CubeCoordinate b)
    {
        return new _CubeCoordinate(
            a.Q + b.Q,
            a.R + b.R,
            a.S + b.S
        );
    }

    static _CubeCoordinate[] Directions = new _CubeCoordinate[]
    {
        new _CubeCoordinate(+1, -1, 0),
        new _CubeCoordinate(+1, 0, -1),
        new _CubeCoordinate(0, +1, -1),
        new _CubeCoordinate(-1, +1, 0),
        new _CubeCoordinate(-1, 0, +1),
        new _CubeCoordinate(0, -1, +1)
    };

    public static _CubeCoordinate GetDirectionCoordinate(int direction)
    {
        return Directions[direction];
    }

    public static _CubeCoordinate GetNeighbourCoordinate(_CubeCoordinate a, int direction)
    {
        return AddCoordinates(a, GetDirectionCoordinate(direction));
    }

    public static _CubeCoordinate[] GetAllNeighbourCoordinates(_CubeCoordinate a)
    {
        List<_CubeCoordinate> results = new List<_CubeCoordinate>();
        foreach (_CubeCoordinate direction in Directions)
        {
            results.Add(AddCoordinates(a, direction));
        }
        return results.ToArray();
    }

    static _CubeCoordinate[] DiagonalDirections = new _CubeCoordinate[]
    {
        new _CubeCoordinate(+2, -1, -1),
        new _CubeCoordinate(+1, +1, -2),
        new _CubeCoordinate(-1, +2, -1),
        new _CubeCoordinate(-2, +1, +1),
        new _CubeCoordinate(-1, -1, +2),
        new _CubeCoordinate(+1, -2, +1)
    };

    public static _CubeCoordinate GetDiagonalDirectionCoordinate(int direction)
    {
        return Directions[direction];
    }

    public static _CubeCoordinate GetDiagonalNeighbourCoordinate(_CubeCoordinate a, int direction)
    {
        return AddCoordinates(a, GetDiagonalDirectionCoordinate(direction));
    }

    public static _CubeCoordinate[] GetAllDiagonalNeighbourCoordinates(_CubeCoordinate a)
    {
        List<_CubeCoordinate> results = new List<_CubeCoordinate>();
        foreach (_CubeCoordinate direction in DiagonalDirections)
        {
            results.Add(AddCoordinates(a, direction));
        }
        return results.ToArray();
    }

    public static int DistanceBetweenCoordinates(_CubeCoordinate a, _CubeCoordinate b) {
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

    public static _CubeCoordinate[] GetCoordinatesWithinRangeOf(_CubeCoordinate a, int range)
    {
        List<_CubeCoordinate> results = new List<_CubeCoordinate>();
        for (int q = -range; q <= range; q++)
        {
            for (int s = Mathf.Max(-range, -q-range); s <= Mathf.Min(range, -q+range); s++)
            {
                    int r = -q-s;
                    results.Add(AddCoordinates(a, new _CubeCoordinate(q,r,s)));
            }
        }
        return results.ToArray();
    }

}