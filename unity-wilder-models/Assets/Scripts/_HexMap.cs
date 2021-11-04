using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _HexMap : MonoBehaviour
{
    /*
    Set the width (NumColumns) and height (NumRows) of the map
    in terms of hex tile units.
    */
    public int NumColumns = 10;
    public int NumRows = 12;

    /*
    Information about the how tor render elevation data.
    */
    public float WaterLevel = 0;
    public float ElevationRenderCap_Max = 5;
    public float ElevationRenderCap_Min = -5;

    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh MeshTerrain;

    public Material MatWater_River;
    public Material MatWater_Pond;

    public Material MatTerrain_Field;
    public Material MatTerrain_Forest;
    public Material MatTerrain_Hedge;
    public Material MatTerrain_Scrub;

    /*
    TO DO:
    Link up to Hex class version of this.
    */
    public bool CoordinatesWrapEastWest = true;
    public bool CoordinatesWrapNorthSouth = true;
    public bool HexesWrapEastWest = true;
    public bool HexesWrapNorthSouth = true;

    private _Hex[,] hexes;
    private Dictionary< _Hex, GameObject > hexToGameObjectMap;

    public _Hex GetHexByOffsetCoordinates(int x, int y)
    {
        return GetHexByOffsetCoordinates(new _OffsetCoordinate(x,y));
    }

    public _Hex GetHexByOffsetCoordinates(_OffsetCoordinate offsetCoordinate)
    {
        if(hexes == null)
        {
            Debug.LogError("Hexes array not yet instantiated!");
            return null;
        }
        // Debug.Log(offsetCoordinate.X + "," + offsetCoordinate.Y + ": ");
        if ( 0 > offsetCoordinate.X || offsetCoordinate.X > NumColumns-1 ) {
            if(CoordinatesWrapEastWest)
            {
                offsetCoordinate.X = (offsetCoordinate.X + NumColumns)  % NumColumns;
            }
            if ( 0 > offsetCoordinate.X || offsetCoordinate.X > NumColumns-1 )
            {
                // Debug.LogError("No hex at " + offsetCoordinate.X + ", " + offsetCoordinate.Y + "!");
                return null;
            }
        }
        if ( 0 > offsetCoordinate.Y || offsetCoordinate.Y > NumRows-1 ) 
        {
            if(CoordinatesWrapNorthSouth)
            {
                offsetCoordinate.Y = (offsetCoordinate.Y + NumRows) % NumRows;
            }
            if ( 0 > offsetCoordinate.Y || offsetCoordinate.Y > NumRows-1 )
            {
                // Debug.LogError("No hex at " + offsetCoordinate.X + ", " + offsetCoordinate.Y + "!");
                return null;
            }
        }
        /*
        TO FIX: 
        Currently the coordinates can only wrap in the negative direction once.
        */
        // Debug.Log("Good Coord!");
        return hexes[offsetCoordinate.X, offsetCoordinate.Y];
    }

    public _Hex GetHexByCubeCoordinates(int q, int r, int s)
    {
        return GetHexByCubeCoordinates(new _CubeCoordinate(q,r,s));
    }
    public _Hex GetHexByCubeCoordinates(_CubeCoordinate cubeCoordinate)
    {
        if(hexes == null)
        {
            Debug.LogError("Hexes array not yet instantiated!");
            return null;
        }
        _OffsetCoordinate offsetCoordinate = _CoordinateTools.CubeToOffsetOddQ(cubeCoordinate); 
        return GetHexByOffsetCoordinates(offsetCoordinate);
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    virtual public void GenerateMap()
    {
        /*
        Generate a map totally filled with water.
        */

        /*
        Instantiate the 2 dimensional array to store the hexes and the
        Hex to Game Object map.
        */
        hexes = new _Hex[NumColumns, NumRows];
        hexToGameObjectMap = new Dictionary<_Hex, GameObject>();

        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                /*
                Place a Hex object using Instantiate:
                Instantiate(GameObject, Position, Rotation, Parent)
                */
                _Hex h = new _Hex( column, row );
                h.Elevation = WaterLevel;

                /*
                Add the Hex to the array of Hexes.
                */
                hexes[ column, row ] = h;

                /*
                Instantiate a HexPrefab gameobject, and link the Hex and the
                GameObject in the dictionary.
                */
                GameObject hexGO =  Instantiate(
                    HexPrefab, 
                    h.Position(),
                    Quaternion.identity,
                    this.transform
                );
                hexToGameObjectMap.Add(h, hexGO);
                
                /*
                Name the GameObject something sensible.
                */
                hexGO.name = "Hex_" + h.Q + "_" + h.R + "_" + h.S;

                /*
                Let the HexBehaviour component know to reference the Hex data
                component from this loop.
                */
                hexGO.GetComponent<_HexComponent>().Hex = h;
                hexGO.GetComponent<_HexComponent>().HexMap = this;

                /*
                Update the coordinate overlay text based on the hex's position
                */
                // );
                hexGO.transform.Find("CoordinateOverlay_XY").GetComponent<TextMesh>().text = string.Format(
                    "{0},{1}",
                    h.X,
                    h.Y
                );
                hexGO.transform.Find("CoordinateOverlay_Q").GetComponent<TextMesh>().text = string.Format(
                    "{0}",
                    h.Q
                );
                hexGO.transform.Find("CoordinateOverlay_R").GetComponent<TextMesh>().text = string.Format(
                    "{0}",
                    h.R
                );
                hexGO.transform.Find("CoordinateOverlay_S").GetComponent<TextMesh>().text = string.Format(
                    "{0}",
                    h.S
                );

                /*
                Update the position of the hex based on camera position (to allow
                for globe-like scrolling) if required.
                */
                hexGO.GetComponent<_HexComponent>().UpdatePosition();
            }
        }

        UpdateHexVisuals();

        /*
        StaticBatchingUtility.Combine( this.gameObject ) can reduce computation 
        batches (and subseqently increase framerate) if the tiles are never going
        to move.
        For some reason this isn't working for me!
        */
        StaticBatchingUtility.Combine( this.gameObject );

    }

    public void UpdateHexVisuals() {
        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                _Hex h = GetHexByOffsetCoordinates(new _OffsetCoordinate(column, row));
                GameObject hexGO = hexToGameObjectMap[h];

                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                mf.mesh = MeshWater;

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();

                if(h.Elevation >= WaterLevel)
                {
                    /*
                    The tile is above the water.
                    */
                    Color32 cMin = new Color32(24, 38, 24, 255);
                    Color32 cMax = new Color32(136, 143, 136, 255);
                    mr.material = MatTerrain_Field;
                    mr.material.color = Color32.Lerp(cMin, cMax, MapElevationForRender(-1, 1, h.Elevation-1));
                }
                else
                {
                    /*
                    The tile is below the water.
                    */
                    Color32 cMin = new Color32(89, 171, 201, 255);
                    Color32 cMax = new Color32(15, 19, 26, 255);
                    mr.material = MatWater_Pond;
                    mr.material.color = Color32.Lerp(cMin, cMax, MapElevationForRender(1, -1, h.Elevation));
                }
            }
        }
    }

    float MapElevationForRender01(float elevation)
    {
        return MapElevationForRender(0, 1, elevation);
    }
    float MapElevationForRender(float newRangeMin, float newRangeMax, float elevation)
    {
        float t = Mathf.InverseLerp(ElevationRenderCap_Min, ElevationRenderCap_Max, elevation);
        float mapped = Mathf.Lerp(newRangeMin, newRangeMax, t);
        return Mathf.Clamp01(mapped);
    }

    public _Hex[] GetHexNeighbours(_Hex centreHex)
    {
        List<_Hex> results = new List<_Hex>();
        foreach (_CubeCoordinate neighbourCoordinate in _CoordinateTools.GetAllNeighbourCoordinates(centreHex.CubeCoordinate))
        {
            _Hex h = GetHexByCubeCoordinates(
                new _CubeCoordinate(
                    neighbourCoordinate.Q, 
                    neighbourCoordinate.R, 
                    neighbourCoordinate.S
                )
            );
            if (h != null){
                results.Add(h);
            }
        }
        return results.ToArray();
    }

    public _Hex[] GetHexDiagonalNeighbours(_Hex centreHex)
    {
        List<_Hex> results = new List<_Hex>();
        foreach (_CubeCoordinate neighbourCoordinate in _CoordinateTools.GetAllDiagonalNeighbourCoordinates(centreHex.CubeCoordinate))
        {
            _Hex h = GetHexByCubeCoordinates(
                new _CubeCoordinate(
                    neighbourCoordinate.Q, 
                    neighbourCoordinate.R, 
                    neighbourCoordinate.S
                )
            );
            if (h != null){
                results.Add(h);
            }
        }
        return results.ToArray();
    }

    public _Hex[] GetHexesWithinRangeOf(_Hex centreHex, int range)
    {
        List<_Hex> results = new List<_Hex>();
        foreach (_CubeCoordinate neighbourCoordinate in _CoordinateTools.GetCoordinatesWithinRangeOf(centreHex.CubeCoordinate, range))
        {
            _Hex h = GetHexByCubeCoordinates(
                new _CubeCoordinate(
                    neighbourCoordinate.Q, 
                    neighbourCoordinate.R, 
                    neighbourCoordinate.S
                )
            );
            if (h != null){
                results.Add(h);
            }
        }
        return results.ToArray();
    }
}
