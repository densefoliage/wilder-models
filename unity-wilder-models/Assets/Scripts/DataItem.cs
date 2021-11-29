using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataItem {
    public int fid;	
    public float left, top, right, bottom;
    public float terrainEle, terrainSlo, terrainAsp;
    public float surfaceEle, surfaceSlo, surfaceAsp;
    public float terSurfDif;
    public int landCoverI;
    public bool isOnsite, isZoned, isForest, isRoad, isHedge, isWater, isSurfFeat;
    public string zoneName, roadType, waterType;

    public DataItem(DataItem d) {
        fid = d.fid;

        left = d.left;
        top = d.top;
        right = d.right;
        bottom = d.bottom;

        terrainEle = d.terrainEle;
        terrainSlo = d.terrainSlo;
        terrainAsp = d.terrainAsp;

        surfaceEle = d.surfaceEle;
        surfaceSlo = d.surfaceSlo;
        surfaceAsp = d.surfaceAsp;

        terSurfDif = d.terSurfDif;

        landCoverI = d.landCoverI;

        isOnsite = d.isOnsite; 
        isZoned = d.isZoned;
        isForest = d.isForest;
        isRoad = d.isRoad;
        isHedge = d.isHedge;
        isWater = d.isWater;
        isSurfFeat = d.isSurfFeat;

        zoneName = d.zoneName;
        roadType = d.roadType; 
        waterType = d.waterType;
        
    }
} 