using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCSV : MonoBehaviour
{
    /*
    https://www.youtube.com/watch?v=C37C2yCUlCM
    */
    public List<HexDatum> hexDatabase = new List<HexDatum>();

    public List<HexDatum> LoadHexData(string path) {

        // Clear database
        hexDatabase.Clear();

        // Read CSV files
        List<Dictionary<string, object>> data = CSVReader.Read(path);

        Debug.Log("LOAD HEX DATA");

        for (int i = 0; i < data.Count; i++)
        {
            Dictionary<string, object> datum = data[i];

            int id = int.Parse(datum["fid"].ToString(), 
                System.Globalization.NumberStyles.Integer);

            float left = float.Parse(datum["left"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float top = float.Parse(datum["top"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float right = float.Parse(datum["right"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float bottom = float.Parse(datum["bottom"].ToString(), 
                System.Globalization.NumberStyles.Float);
            
            float terrainElevation = float.Parse(datum["terrainEle"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float terrainSlope = float.Parse(datum["terrainSlo"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float terrainAspect = float.Parse(datum["terrainAsp"].ToString(), 
                System.Globalization.NumberStyles.Float);

            float surfaceElevation = float.Parse(datum["surfaceEle"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float surfaceSlope = float.Parse(datum["surfaceSlo"].ToString(), 
                System.Globalization.NumberStyles.Float);
            float surfaceAspect = float.Parse(datum["surfaceAsp"].ToString(), 
                System.Globalization.NumberStyles.Float);

            float terrainSurfaceDifference = float.Parse(datum["terSurfDif"].ToString(), 
                System.Globalization.NumberStyles.Float);
            bool hasSurfaceFeature = bool.Parse(datum["hasSurfFea"].ToString());

            int landCoverIndex = int.Parse(datum["landCoverI"].ToString(), 
                System.Globalization.NumberStyles.Integer);
            
            bool isOnsite = bool.Parse(datum["isOnsite"].ToString());
            bool isZoned = bool.Parse(datum["isZoned"].ToString());
            bool isForest = bool.Parse(datum["isForest"].ToString());
            bool isRoad = bool.Parse(datum["isRoad"].ToString());
            bool isHedge = bool.Parse(datum["isHedge"].ToString());
            bool isWater = bool.Parse(datum["isWater"].ToString());

            string zoneName = datum["zoneName"].ToString();
            string roadType = datum["roadType"].ToString();
            string waterType = datum["waterType"].ToString();

            HexDatum hexDatum = new HexDatum(id);
            hexDatum.Left = left;
            hexDatum.Top = top;
            hexDatum.Right = right;
            hexDatum.Bottom = bottom;
            hexDatum.TerrainElevation = terrainElevation;
            hexDatum.TerrainSlope = terrainSlope;
            hexDatum.TerrainAspect = terrainAspect;
            hexDatum.SurfaceElevation = surfaceElevation;
            hexDatum.SurfaceSlope = surfaceSlope;
            hexDatum.SurfaceAspect = surfaceAspect;
            hexDatum.TerrainSurfaceDifference = terrainSurfaceDifference;
            hexDatum.HasSurfaceFeature = hasSurfaceFeature;
            hexDatum.LandCoverIndex = landCoverIndex;
            hexDatum.IsOnsite = isOnsite;
            hexDatum.IsZoned = isZoned;
            hexDatum.IsForest = isForest;
            hexDatum.IsRoad = isRoad;
            hexDatum.IsHedge = isHedge;
            hexDatum.IsWater = isWater;
            hexDatum.ZoneName = zoneName;
            hexDatum.RoadType = roadType;
            hexDatum.WaterType = waterType;

            hexDatabase.Add(hexDatum);
        }

        return hexDatabase;
    }

    public void ClearHexData() {

        // Clear database
        hexDatabase.Clear();
    }
}
