using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
	public Color[] colors;
	public HexGrid hexGrid;
	Color activeColor;
	float activeElevation;
	bool applyColor;
	bool applyElevation = true;
	int brushSize;

	void Awake () {
		SelectColor(0);
	}

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
	}

	void HandleInput () 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			EditCells(hexGrid.GetCellByPosition(hit.point));
		}
	}
	public void SelectColor (int index) 
    {
		applyColor = index >= 0;
		if (applyColor) {
			activeColor = colors[index];	
		}
	}
	public void SetElevation (float elevation) 
	{
		activeElevation = elevation;
	}
	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
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
			if (applyColor) {
				cell.Color = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
		}
	}
}
