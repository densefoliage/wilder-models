using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

	[SerializeField] private int x, z;

	public int X 
	{
		get {
			return x;
		}
	}
	public int Z 
	{
		get {
			return z;
		}
	}
    public int Y 
	{
		get {
			return -X - Z;
		}
	}

	public HexCoordinates (int x, int z) 
	{
		this.x = x;
		this.z = z;
	}
	public static HexCoordinates FromOffsetCoordinates (int x, int z) 
	{
		return new HexCoordinates(x, z - x / 2);
	}
	public static HexCoordinates FromPosition (Vector3 position) 
	{
		float offset = position.x / (HexMetrics.OUTER_RADIUS * 3f);

		float z = -1 * position.z / (HexMetrics.INNER_RADIUS * 2f);
		float y = -z;
		float x = -z - y;

		z -= offset;
		y -= offset;

		int iZ = Mathf.RoundToInt(z);
		int iY = Mathf.RoundToInt(y);
		int iX = Mathf.RoundToInt(-z -y);

		if (iX + iY + iZ != 0) 
		{
			// Debug.LogWarning("rounding error!");
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

		return new HexCoordinates(iX, iZ);
	}

    public override string ToString () 
	{
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}
	public string ToStringOnSeparateLines () 
	{
		return "x: " + X.ToString() + "\ny: " + Y.ToString() + "\nz: " + Z.ToString();
	}


}