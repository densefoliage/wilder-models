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
			EditCell(hexGrid.GetCellByPosition(hit.point));
		}
	}
	public void SelectColor (int index) 
    {
		activeColor = colors[index];
	}
	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}
	void EditCell (HexCell cell) {
		cell.color = activeColor;
		cell.elevation = activeElevation;
		hexGrid.Refresh();
	}
}
