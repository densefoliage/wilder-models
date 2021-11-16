using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public int index;
	public Color Color {
		get {
			return color;
		}
		set {
			if (color == value) {
				return;
			}
			color = value;
			Refresh();
		}
	}
    public float Elevation 
    {
        get 
        {
            return elevation;
        }
        set
        {
            if (elevation == value) {
				return;
			}
            elevation = value;
            Vector3 position = transform.localPosition;
            /*
            Should this happen here, or when the mesh is constructed? Otherwise, heights
            don't change when the ELEVATION_PERTURB_FACTOR changes...
            */
            position.y = value * HexMetrics.ELEVATION_FACTOR;
            position.y +=
				(HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ELEVATION_PERTURB_FACTOR;
            transform.localPosition = position;
            Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = -position.y;
			uiRect.localPosition = uiPosition;
            Refresh();
        }
    }
    public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}
    public Text Label {
        get {
            return label;
        }
        set {
            label = value;
            uiRect = label.rectTransform;
        }
    }
	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			waterLevel = value;
			Refresh();
		}
	}
	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}
	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.WATER_ELEVATION_OFFSET) *
				HexMetrics.ELEVATION_FACTOR;
		}
	}
    public RectTransform uiRect;
    public HexGridChunk chunk;

    [SerializeField]
    HexCell[] neighbours;
    Color color;
    float elevation = int.MinValue;
    Text label;
    int waterLevel;

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
	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbours.Length; i++) {
				HexCell neighbour = neighbours[i];
				if (neighbour != null && neighbour.chunk != chunk) {
					neighbour.chunk.Refresh();
				}
			}
		}
	}
}
