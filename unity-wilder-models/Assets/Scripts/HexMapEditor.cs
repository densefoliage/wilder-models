using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
	public HexGrid hexGrid;
	int activeTerrainTypeIndex;
	float activeElevation;
	float activeWaterLevel;
	bool applyColor;
	bool applyElevation = true;
	bool applyWaterLevel = true;
	int brushSize;
	OptionalToggle streamMode, roadMode;
	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
		}
		else {
			previousCell = null;
		}
	}

	void HandleInput () 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCellByPosition(hit.point);
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}
	void ValidateDrag (HexCell currentCell) {
		for (
			dragDirection = HexDirection.SE;
			dragDirection <= HexDirection.NE;
			dragDirection++
		) {
			if (previousCell.GetNeighbour(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		/*
		TO DO: Prevent drag from jittering back and forth to by remembering
		drag direction and preventing it from immediately going in the opposite
		direction.
		*/
		isDrag = false;
	}
	public void SetTerrainTypeIndex (int index) {
		activeTerrainTypeIndex = index;
	}
	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
	}
	public void SetElevation (float elevation) 
	{
		activeElevation = elevation;
	}
	public void SetApplyWaterLevel (bool toggle) {
		applyWaterLevel = toggle;
	}
	
	public void SetWaterLevel (float level) {
		activeWaterLevel = (int)level;
	}
	public void SetStreamMode (int mode) {
		streamMode = (OptionalToggle)mode;
	}
	public void SetRoadMode (int mode) {
		roadMode = (OptionalToggle)mode;
	}
	public void SelectLabelMode (int index) 
    {
		hexGrid.SelectLabelMode(index);
	}
	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}
	void EditCells (HexCell center) 
	{
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, x = centerX - brushSize; x <= centerX; x++, r++) {
			for (int z = centerZ - r; z <= centerZ + brushSize; z++) {
				EditCell(hexGrid.GetCellByCoordinates(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, x = centerX + brushSize; x > centerX; x--, r++) {
			for (int z = centerZ - brushSize; z <= centerZ + r; z++) {
				EditCell(hexGrid.GetCellByCoordinates(new HexCoordinates(x, z)));
			}
		}

	}
	void EditCell (HexCell cell) 
	{
		if (cell) {
			if (activeTerrainTypeIndex >= 0) {
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel) {
				/*
				This looks crazy when adjacent submerged cells have different 
				water levels.
				TO DO: FIX THIS!
				*/
				cell.WaterLevel = activeWaterLevel;
			}
			if (streamMode == OptionalToggle.No) {
				cell.RemoveStream();
			}
			if (roadMode == OptionalToggle.No) {
				cell.RemoveRoads();
			}
			if (isDrag) {
				HexCell otherCell = cell.GetNeighbour(dragDirection.Opposite());
				if (otherCell) {
					if (streamMode == OptionalToggle.Yes) {
						otherCell.SetOutgoingStream(dragDirection);
					}
					if (roadMode == OptionalToggle.Yes) {
						otherCell.AddRoad(dragDirection);
					}
				}
			}
		}
	}
	public void Save () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		Debug.Log("SAVE: " + path);

		using (
			BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(1);
			hexGrid.Save(writer);
		}
	}

	public void Load () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		Debug.Log("LOAD: " + path);

		using (
			BinaryReader reader =
				new BinaryReader(File.OpenRead(path))
		) {
			int header = reader.ReadInt32();
			if (header <= 1) {
				/*
				This is save format 0
				*/
				hexGrid.Load(reader, header);
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
}
