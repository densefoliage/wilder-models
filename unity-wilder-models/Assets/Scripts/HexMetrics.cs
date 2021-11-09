using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    private const float ROOT_3_DIV_2 = 0.86602540378f; // sqrt(3)/2
    private const float PI = Mathf.PI;
    public const float INNER_RADIUS = 1f;
    public const float OUTER_RADIUS = INNER_RADIUS / ROOT_3_DIV_2;
    public const float SOLID_FACTOR = 0.75f;
    public const float BLEND_FACTOR = 1f - SOLID_FACTOR;
    public const float ELEVATION_FACTOR = 0.25f;
    public const int TERRACES_PER_SLOPE = 2;
    public const int TERRACE_STEPS = TERRACES_PER_SLOPE * 2 + 1;
    public const float HORIZONTAL_TERRACE_STEP_SIZE = 1f / TERRACE_STEPS;
    public const float VERTICAL_TERRACE_STEP_SIZE = 1f / (TERRACES_PER_SLOPE+1);
    public const float STEEP_THRESHOLD = 2f;

    // public const float elevationStep = 5f;

    /*
    A function to find each vertex of the hexagon, with the vertex on the
    centre right being vertex 0 and each subsequent vertex being 60degrees
    away anticlockwise.
    Each vertex is OUTER_RADIUS units away from the center.
    */
    static Vector3 FlatHexCorner(float size, int i)
    {
        int deg = -60 * i;
        float rad = PI / 180 * deg;
        return new Vector3(
            size * Mathf.Cos(rad),
            0,
            size * Mathf.Sin(rad)
        );
    }
    public static Vector3[] corners = 
    {
        FlatHexCorner( OUTER_RADIUS, 0 ),
        FlatHexCorner( OUTER_RADIUS, 1 ),
        FlatHexCorner( OUTER_RADIUS, 2 ),
        FlatHexCorner( OUTER_RADIUS, 3 ),
        FlatHexCorner( OUTER_RADIUS, 4 ),
        FlatHexCorner( OUTER_RADIUS, 5 )
    };
    public static Vector3 GetCorner (int i) 
    {
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
		return GetCorner((int)direction) * SOLID_FACTOR;
	}

	public static Vector3 GetSecondSolidCorner (HexDirection direction) 
    {
		return GetCorner((int)direction + 1) * SOLID_FACTOR;
	}
    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return GetCorner((int)direction);
    }
    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return GetCorner((int)direction+1);
    }
	public static Vector3 GetBridge (HexDirection direction) 
    {
		return (GetCorner((int)direction) + GetCorner((int)direction+1)) 
            * BLEND_FACTOR;
	}
	public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) 
    {
        float h = step * HORIZONTAL_TERRACE_STEP_SIZE;
        a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * VERTICAL_TERRACE_STEP_SIZE;
		a.y += (b.y - a.y) * v;
		return a;
	}
    public static Color TerraceLerp (Color a, Color b, int step) {
		float h = step * HORIZONTAL_TERRACE_STEP_SIZE;
		return Color.Lerp(a, b, h);
	}
    public static HexEdgeType GetEdgeType (float elevation1, float elevation2) {
		if (elevation1 == elevation2) {
			return HexEdgeType.Flat;
		}
		float delta = elevation2 - elevation1;
		if (Mathf.Abs(delta) < STEEP_THRESHOLD) {
			return HexEdgeType.Slope;
		}
		return HexEdgeType.Cliff;
	}
}
