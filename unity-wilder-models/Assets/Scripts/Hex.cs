using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
To do: data for hexes is organised in offset coordinates, some
translation will need to occur (at some point).
*/

/// <summary>
/// The Hex class defines the grid position, world space position, size,
/// neighbours, etc... of a hex tile. However, it does not interact with
/// unity directly in any way.
/// </summary>
public class Hex
{
    /*
    We are using cubic coordinates to store the position of each hex.
    There are 3 axis that define the coordinates:
        Q = column (x)
        R = row (y)
        S = sum value (z)
    The sum of the coordinate components always equals 0:
        Q + R + S = 0
    Hence, we can calculate S, rather than setting it directly:
        S = -(Q + R)

        this.Q = x;
        this.R = y - (x - (x&1)) / 2;
        this.S = -(this.Q + this.R);
    */

    public Hex(int x, int y)
    {
        this.X = x;
        this.Y = y;

        this.Q = x;
        this.R = Y - (x - (x&1)) / 2;
        this.S = -(this.Q + this.R);
    }

    public readonly int X;
    public readonly int Y;
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    static readonly float HEIGHT_MULTIPLIER = Mathf.Sqrt(3);
    float radius = 1f;

    bool allowWrapEastWest = false;
    bool allowWrapNorthSouth = false;

    /// <summary>
    /// Returns the world-space position of this hex.
    /// </summary>
    public Vector3 Position()
    {
        return new Vector3(
            HexHorizontalSpacing() * (this.Q),
            0,
            HexVerticalSpacing() * (this.R + this.Q/2f)
        );
    }

    public float HexWidth()
    {
        return radius * 2;
    }

    public float HexHeight() 
    {
        return radius * HEIGHT_MULTIPLIER;
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * -1;
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth() * 0.75f;
    }

    public Vector3 PositionFromCamera( Vector3 cameraPosition, int numColumns, int numRows )
    {
        float mapWidth = numColumns * HexHorizontalSpacing();
        float mapHeight = numRows * HexVerticalSpacing();

        Vector3 position = Position();

        if(allowWrapEastWest)
        {
            float howManyWidthsFromCamera = (position.x - cameraPosition.x) / mapWidth;
            
            /*
            If howManyWidthsFromCamera is between -0.5 and 0.5 then the tile is 
            visible to the camera.
            */
            if(howManyWidthsFromCamera > 0) {
                howManyWidthsFromCamera += 0.5f;
            } else {
                howManyWidthsFromCamera -= 0.5f;
            }

            int howManyWidthsToFix = (int)howManyWidthsFromCamera;
            position.x -= (howManyWidthsToFix * mapWidth);
        }

        if(allowWrapNorthSouth)
        {
            float howManyHeightsFromCamera = (position.z - cameraPosition.z) / mapHeight;
            
            /*
            If howManyWidthsFromCamera is between -0.5 and 0.5 then the tile is 
            visible to the camera.
            */
            if(howManyHeightsFromCamera > 0) {
                howManyHeightsFromCamera += 0.5f;
            } else {
                howManyHeightsFromCamera -= 0.5f;
            }

            int howManyHeightsToFix = (int)howManyHeightsFromCamera;
            position.z -= (howManyHeightsToFix * mapHeight);
        }

        return position;

    }

}
