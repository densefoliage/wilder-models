using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexDatum {
    [SerializeField]
    int id;	

    public float Left {
        get {
            return left;
        }
        set {
            left = value;
        }
    }
    public float Top {
        get {
            return top;
        }
        set {
            top = value;
        }
    }
    public float Right {
        get {
            return right;
        }
        set {
            right = value;
        }
    }
    public float Bottom {
        get {
            return bottom;
        }
        set {
            bottom = value;
        }
    }
    float left, top, right, bottom;

    public float TerrainElevation {
        get {
            return terrainElevation;
        }
        set {
            terrainElevation = value;
        }
    }
    public float TerrainSlope {
        get {
            return terrainSlope;
        }
        set {
            if (value < 0f) {
                terrainSlope = value + 90f;
            } else {
                terrainSlope = value % 90f;
            }
        }
    }
    public float TerrainAspect {
        get {
            return terrainAspect;
        }
        set {
            if (value < 0f) {
                terrainAspect = value + 360f;
            } else {
                terrainAspect = value % 360f;
            }
        }
    }
    [SerializeField]
    float terrainElevation, terrainSlope, terrainAspect;

    public float SurfaceElevation {
        get {
            return surfaceElevation;
        }
        set {
            surfaceElevation = value;
        }
    }
    public float SurfaceSlope {
        get {
            return surfaceSlope;
        }
        set {
            if (value < 0f) {
                surfaceSlope = value + 90f;
            } else {
                surfaceSlope = value % 90f;
            }
        }
    }
    public float SurfaceAspect {
        get {
            return surfaceAspect;
        }
        set {
            if (value < 0f) {
                surfaceAspect = value + 360f;
            } else {
                surfaceAspect = value % 360f;
            }
        }
    }
    [SerializeField]
    float surfaceElevation, surfaceSlope, surfaceAspect;

    public float TerrainSurfaceDifference {
        get {
            return terrainSurfaceDifference;
        }
        set {
            terrainSurfaceDifference = value;
        }
    }
    float terrainSurfaceDifference;
    public bool HasSurfaceFeature {
        get {
            return hasSurfaceFeature;
        }
        set {
            hasSurfaceFeature = value;
        }
    }
    [SerializeField]
    bool hasSurfaceFeature;

    public int LandCoverIndex {
        get {
            return landCoverIndex;
        }
        set {
            if (value >=1 && value <= 21) {
                landCoverIndex = value;
                landCover = LAND_COVER_TYPES[ value-1 ];
            }
        }
    }
    public string LandCover {
        get {
            return landCover;
        }
    }
    [SerializeField]
    int landCoverIndex;
    [SerializeField]
    string landCover;

    public bool IsOnsite {
        get {
            return isOnsite;
        }
        set {
            isOnsite = value;
        }
    }
    public bool IsZoned {
        get {
            return isZoned;
        }
        set {
            isZoned = value;
        }
    }
    public bool IsForest {
        get {
            return isForest;
        }
        set {
            isForest = value;
        }
    }
    public bool IsRoad {
        get {
            return isRoad;
        }
        set {
            isRoad = value;
        }
    }
    public bool IsHedge {
        get {
            return isHedge;
        }
        set {
            isHedge = value;
        }
    }
    public bool IsWater {
        get {
            return isWater;
        }
        set {
            isWater = value;
        }
    }
    [SerializeField]
    bool isOnsite, isZoned, isForest, isRoad, isHedge, isWater;

    public string ZoneName {
        get {
            return zoneName;
        }
        set {
            zoneName = value;
        }
    }
    public string RoadType {
        get {
            return roadType;
        }
        set {
            // TO DO: Validate road type
            roadType = value;
        }
    }
    public string WaterType {
        get {
            return waterType;
        }
        set {
            // TO DO: Validate water type
            waterType = value;
        }
    }
    [SerializeField]
    string zoneName, roadType, waterType;

    static string[] LAND_COVER_TYPES = {
        "deciduous woodland",
        "coniferous woodland",
        "arable",
        "improved grassland",
        "neutral grassland",
        "calcareous grassland",
        "acid grassland",
        "fen",
        "heather",
        "heather grassland",
        "bog",
        "inland rock",
        "saltwater",
        "freshwater",
        "supralittoral rock",
        "supralittoral sediment",
        "littoral rock",
        "littoral sediment",
        "saltmarsh",
        "urban",
        "suburban"
    };
    static string[] VALID_ROAD_TYPES = {
        "footpath",
        "track",
        "road"
    };
    static string[] VALID_WATER_TYPES = {
        "footpath",
        "track",
        "road"
    };

    public HexDatum( int _id ) {
        id = _id;
    }
    public HexDatum(HexDatum d) {
        id = d.id;

        left = d.left;
        top = d.top;
        right = d.right;
        bottom = d.bottom;

        terrainElevation = d.terrainElevation;
        terrainSlope = d.terrainSlope;
        terrainAspect = d.terrainAspect;

        surfaceElevation = d.surfaceElevation;
        surfaceSlope = d.surfaceSlope;
        surfaceAspect = d.surfaceAspect;

        terrainSurfaceDifference = d.terrainSurfaceDifference;
        hasSurfaceFeature = d.hasSurfaceFeature;

        landCoverIndex = d.landCoverIndex;

        isOnsite = d.isOnsite; 
        isZoned = d.isZoned;
        isForest = d.isForest;
        isRoad = d.isRoad;
        isHedge = d.isHedge;
        isWater = d.isWater;

        zoneName = d.zoneName;
        roadType = d.roadType; 
        waterType = d.waterType;
    }
} 