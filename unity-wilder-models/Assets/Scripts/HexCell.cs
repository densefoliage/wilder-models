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
			return HexMetrics.colors[terrainTypeIndex];
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

			ValidateStreams();

			for (int i = 0; i < roads.Length; i++) {
				if (roads[i] && GetElevationDifference((HexDirection)i) > 1) {
					SetRoad(i, false);
				}
			}

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
	public float WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			waterLevel = value;
			ValidateStreams();
			Refresh();
		}
	}
	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}
	public float StreamBedY {
		get {
			return
				(elevation + HexMetrics.STREAM_BED_ELEVATION_OFFSET) *
				HexMetrics.ELEVATION_FACTOR;
		}
	}
	public float StreamSurfaceY {
		get {
			return
				(elevation + HexMetrics.WATER_ELEVATION_OFFSET) *
				HexMetrics.ELEVATION_FACTOR;
		}
	}
	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.WATER_ELEVATION_OFFSET) *
				HexMetrics.ELEVATION_FACTOR;
		}
	}
	public int TerrainTypeIndex {
		get {
			return terrainTypeIndex;
		}
		set {
			if (terrainTypeIndex != value) {
				terrainTypeIndex = value;
				Refresh();
			}
		}
	}
    public RectTransform uiRect;
    public HexGridChunk chunk;

    [SerializeField]
    HexCell[] neighbours;

	[SerializeField]
	bool[] roads;

    int terrainTypeIndex;
    float elevation = int.MinValue;
    Text label;
    float waterLevel;
    bool hasIncomingStream, hasOutgoingStream;
    HexDirection incomingStream, outgoingStream;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	/*
	CORE
	*/
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
	public float GetElevationDifference (HexDirection direction) {
		float difference = elevation - GetNeighbour(direction).elevation;
		return difference >= 0 ? difference : -difference;
	}

	/*
	STREAMS
	*/
	public bool HasIncomingStream {
		get {
			return hasIncomingStream;
		}
	}
	public bool HasOutgoingStream {
		get {
			return hasOutgoingStream;
		}
	}
	public HexDirection IncomingStream {
		get {
			return incomingStream;
		}
	}
	public HexDirection OutgoingStream {
		get {
			return outgoingStream;
		}
	}
    public bool HasStream {
		get {
			return hasIncomingStream || hasOutgoingStream;
		}
	}
	public bool HasStreamBeginOrEnd {
		get {
			return hasIncomingStream != hasOutgoingStream;
		}
	}
	public bool HasStreamThroughEdge (HexDirection direction) {
		return
			hasIncomingStream && incomingStream == direction ||
			hasOutgoingStream && outgoingStream == direction;
	}
	public void RemoveOutgoingStream () {
		if (!hasOutgoingStream) {
			return;
		}
		hasOutgoingStream = false;
		RefreshSelfOnly();
        
        HexCell neighbour = GetNeighbour(outgoingStream);
		neighbour.hasIncomingStream = false;
		neighbour.RefreshSelfOnly();
	}
	public void RemoveIncomingStream () {
		if (!hasIncomingStream) {
			return;
		}
		hasIncomingStream = false;
		RefreshSelfOnly();

		HexCell neighbour = GetNeighbour(incomingStream);
		neighbour.hasOutgoingStream = false;
		neighbour.RefreshSelfOnly();
	}
	public void RemoveStream () {
		RemoveOutgoingStream();
		RemoveIncomingStream();
	}
	public void SetOutgoingStream (HexDirection direction) {
		if (hasOutgoingStream && outgoingStream == direction) {
			return;
		}

		HexCell neighbour = GetNeighbour(direction);
		if (!IsValidStreamDestination(neighbour)) {
			return;
		}

		RemoveOutgoingStream();
		if (hasIncomingStream && incomingStream == direction) {
			RemoveIncomingStream();
		}

		hasOutgoingStream = true;
		outgoingStream = direction;
		
		neighbour.RemoveIncomingStream();
		neighbour.hasIncomingStream = true;
		neighbour.incomingStream = direction.Opposite();

		SetRoad((int)direction, false);
	}
	public HexDirection StreamBeginOrEndDirection {
		get {
			return hasIncomingStream ? incomingStream : outgoingStream;
		}
	}
	bool IsValidStreamDestination (HexCell neighbour) {
		return neighbour && (
			elevation >= neighbour.elevation || waterLevel == neighbour.elevation
		);
	}
	void ValidateStreams () {
		if (
			hasOutgoingStream &&
			!IsValidStreamDestination(GetNeighbour(outgoingStream))
		) {
			RemoveOutgoingStream();
		}
		if (
			hasIncomingStream &&
			!GetNeighbour(incomingStream).IsValidStreamDestination(this)
		) {
			RemoveIncomingStream();
		}
	}

	/*
	ROADS
	*/
	public bool HasRoadThroughEdge (HexDirection direction) {
		return roads[(int)direction];
	}
	public bool HasRoads {
		get {
			for (int i = 0; i < roads.Length; i++) {
				if (roads[i]) {
					return true;
				}
			}
			return false;
		}
	}
	public void AddRoad (HexDirection direction) {
		if (!roads[(int)direction] && !HasStreamThroughEdge(direction) &&
			GetElevationDifference(direction) <= HexMetrics.ROAD_ELEVATION_DIFFERENCE_THRESHOLD
		) {
			SetRoad((int)direction, true);
		}
	}
	public void RemoveRoads () {
		for (int i = 0; i < neighbours.Length; i++) {
			if (roads[i]) {
				SetRoad(i, false);
			}
		}
	}
	void SetRoad (int index, bool state) {
		roads[index] = state;
		neighbours[index].roads[(int)((HexDirection)index).Opposite()] = state;
		neighbours[index].RefreshSelfOnly();
		RefreshSelfOnly();
	}

	/*
	REFRESH
	*/
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
	void RefreshSelfOnly () {
		chunk.Refresh();
	}
}
