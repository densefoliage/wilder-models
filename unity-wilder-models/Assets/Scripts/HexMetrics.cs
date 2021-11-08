using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    private const float ROOT_3_DIV_2 = 0.86602540378f; // sqrt(3)/2
    private const float PI = Mathf.PI; // sqrt(3)/2
    public const float INNER_RADIUS = 1f;
    public const float OUTER_RADIUS = INNER_RADIUS / ROOT_3_DIV_2;
    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    /*
    A function to find each vertex of the hexagon, with the vertex on the
    centre right being vertex 0 and each subsequent vertex being 60degrees
    away anticlockwise.
    Each vertex is OUTER_RADIUS units away from the center.
    */
    private static Vector3 FlatHexCorner(float size, int i)
    {
        int deg = -60 * i;
        float rad = PI / 180 * deg;
        return new Vector3(
            size * Mathf.Cos(rad),
            0,
            size * Mathf.Sin(rad)
        );
    }
    public static Vector3[] corners = {
        FlatHexCorner( OUTER_RADIUS, 0 ),
        FlatHexCorner( OUTER_RADIUS, 1 ),
        FlatHexCorner( OUTER_RADIUS, 2 ),
        FlatHexCorner( OUTER_RADIUS, 3 ),
        FlatHexCorner( OUTER_RADIUS, 4 ),
        FlatHexCorner( OUTER_RADIUS, 5 )
    };

    public static Vector3 GetCorner (int i) {
        /* 
        If we ever minus from a direction, we need to +6 before the modulo
        to account for negative looping.
        */
        return corners[
            i % 6
        ];
    }

    public static Vector3 GetFirstSolidCorner (HexDirection direction) 
    {
		return GetCorner((int)direction) * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner (HexDirection direction) 
    {
		return GetCorner((int)direction + 1) * solidFactor;
	}
    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return GetCorner((int)direction);
    }
    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return GetCorner((int)direction+1);
    }
	public static Vector3 GetBridge (HexDirection direction) {
		return (GetCorner((int)direction) + GetCorner((int)direction+1)) 
            * blendFactor;
	}
}
