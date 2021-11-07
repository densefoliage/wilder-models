using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    private const float ROOT_3_DIV_2 = 0.86602540378f; // sqrt(3)/2
    private const float PI = Mathf.PI; // sqrt(3)/2
    public const float INNER_RADIUS = 1f;
    public const float OUTER_RADIUS = INNER_RADIUS / ROOT_3_DIV_2;

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
}
