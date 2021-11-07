using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

    Canvas gridCanvas;
    HexMesh hexMesh;

    HexCell[] cells;

    void Awake() 
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position = new Vector3(
            x * HexMetrics.OUTER_RADIUS * 1.5f,
            0f,
            (z + x * 0.5f - x / 2) * (HexMetrics.INNER_RADIUS * 2f) * -1
        );

        HexCell cell = cells[i] = Instantiate<HexCell>(
            cellPrefab,
            position,
            Quaternion.identity,
            this.transform
            );
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        cell.name = "Hex" + cell.coordinates.ToString();

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(
            position.x,
            position.z
        );
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

    // Start is called before the first frame update
    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
			HandleInput();
		}
    }

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit.point);
		}
	}
	
	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        // int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        int index = coordinates.Z - coordinates.X * width + coordinates.X / 2;
		HexCell cell = cells[index];
		cell.color = touchedColor;
		hexMesh.Triangulate(cells);
		// Debug.Log("touched at " + position + "->" + coordinates.ToString());
	}
}
