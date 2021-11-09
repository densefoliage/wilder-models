using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color;
    public float Elevation 
    {
        get 
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            /* 
            The tutorial suggests using the above, but because I don't want to use
            integers as elevation, I will use the below.
            */
            // position.y = value * HexMetrics.elevationStep;
            position.y = value * HexMetrics.ELEVATION_FACTOR;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.ELEVATION_FACTOR;
			uiRect.localPosition = uiPosition;
        }
    }
    public RectTransform uiRect;

    [SerializeField]
    HexCell[] neighbours;
    float elevation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public HexCell GetNeighbour(HexDirection direction) 
    {
        return neighbours[(int)direction];
    }
    public void SetNeighbour(HexDirection direction, HexCell cell) 
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }
    public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, GetNeighbour(direction).elevation
		);
	}
    public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}
}
