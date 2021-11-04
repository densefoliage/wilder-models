using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _HexComponent : MonoBehaviour
{
    public _Hex Hex;
    public _HexMap HexMap;
    public void UpdatePosition() {
        this.transform.position = Hex.PositionFromCamera(
            Camera.main.transform.position, 
            HexMap.NumColumns,
            HexMap.NumRows,
            HexMap.HexesWrapEastWest,
            HexMap.HexesWrapNorthSouth
        );
    }
}