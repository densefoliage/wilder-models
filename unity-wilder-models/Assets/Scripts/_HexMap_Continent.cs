using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _HexMap_Continent : _HexMap
{
    /*
    Should a new map be generated when the mouse is clicked?
    */
    private bool generateMapOnClick = true;

    /*
    Should the debug log print random data?
    */
    private bool printDebugInformation = false;

    void Update()
    {
        if (generateMapOnClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RandomTerraforming();
                UpdateHexVisuals();
            }
        }
    }

    void RandomTerraforming()
    {
        Debug.Log("GENERATING RANDOM MAP");
        /*
        Reset the terrain elevation to a baseElevation (0 by default);
        */
        float baseElevation = Random.Range(0f, 1.5f);
        if (printDebugInformation) 
        {
            Debug.Log("Base Elevation: " + baseElevation);
        }
        for (int x = 0; x < NumColumns; x++)
        {
            for (int y = 0; y < NumRows; y++)
            {
                _Hex h = GetHexByOffsetCoordinates(new _OffsetCoordinate(x, y));
                h.Elevation = baseElevation;
            }
        }

        /*
        Change the elevation in the map...
        Starting with low points (ponds):
        */
        int numPonds = Random.Range(5, 9);
        if (printDebugInformation) 
        {
            Debug.Log("Num Ponds: " + numPonds);
        }
        for (int i = 0; i < numPonds; i++)
        {
            int x = Random.Range(0, NumColumns - 1);
            int y = Random.Range(0, NumRows - 1);
            int range = Random.Range(4, 12);
            float height = Random.Range(-1f, -5f);
            float falloff = Random.Range(0.75f, 2f);
            ElevateRange(x, y, range, height, falloff);
        }

        /*
        And now with high points (hills):
        */
        int numHills = Random.Range(7, 11);
        if (printDebugInformation) 
        {
        }
            Debug.Log("Num Hills: " + numPonds);
        for (int i = 0; i < numHills; i++)
        {
            int x = Random.Range(0, NumColumns - 1);
            int y = Random.Range(0, NumRows - 1);
            int range = Random.Range(3, 12);
            float height = Random.Range(1f, 5f);
            float falloff = Random.Range(1f, 2f);
            ElevateRange(x, y, range, height, falloff);
        }


        /*
        Add lumpiness - Perlin Noise maybe?
        */
        float noiseResolution = 0.3f;
        float noiseScale = Random.Range(1.5f,2.5f);
        float noiseXOffset = Random.Range(0f,1f);
        float noiseYOffset = Random.Range(0f,1f);
        if (printDebugInformation) 
        {
            Debug.Log("Noise Resol: " + noiseResolution);
            Debug.Log("Noise Scale: " + noiseScale);
            Debug.Log("Noise Offset: " + noiseXOffset + ", " + noiseYOffset);
        }
        for (int x = 0; x < NumColumns; x++)
        {
            for (int y = 0; y < NumRows; y++)
            {
                _Hex h = GetHexByOffsetCoordinates(new _OffsetCoordinate(x, y));
                float n = (Mathf.PerlinNoise( 
                    noiseXOffset + (float)x/Mathf.Max(NumColumns, NumRows)/noiseResolution, 
                    noiseYOffset + (float)y/Mathf.Max(NumColumns, NumRows)/noiseResolution 
                ) - 0.5f) * noiseScale * 2f;
                h.Elevation += n;
            }
        }

        /*
        Set mesh to mountain/hill/flat/water based on height
        */

        /*
        Simulate rainfall/moisture (probably just noise for now)
        and set materials
        */

        /*
        Now make sure all the hex visuals update to match the data.
        */
    }
    override public void GenerateMap()
    {
        base.GenerateMap();
        RandomTerraforming();
        UpdateHexVisuals();
    }

    void ElevateRange(int x, int y, int range, float elevationHeight)
    {
        ElevateRange(x, y, range, elevationHeight, 0f);
    }
    void ElevateRange(int x, int y, int range, float elevationHeight, float falloff)
    {
        _Hex centreHex = GetHexByOffsetCoordinates(new _OffsetCoordinate(x, y));
        if (centreHex != null)
        {
            _Hex[] targetHexes = GetHexesWithinRangeOf(centreHex, range);
            foreach (_Hex h in targetHexes)
            {
                /*
                This doesn't this work when wrapping is on.
                https://www.youtube.com/watch?v=XRP8BqdYHXU <- some help here
                */
                h.Elevation += elevationHeight * Mathf.Lerp(1f, 0f, falloff * (_Hex.Distance(centreHex, h) / (range + 1)));
            }
        }
    }

}
