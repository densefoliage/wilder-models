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
    Text[] labels;
    string[] overlayModes = {
        "coordinates",
        "index",
        "hidden"
    };
    int previousOverlayMode;
    int activeOverlayMode = 0;

    void Awake() 
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];
        labels = new Text[height * width];

        for (int x = 0, i = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
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
        /* 
        Connecting neighbours... 
        */
        if (z > 0)
        {
            /* 
            We start by connecting north to south after row 0.
            */
            cell.SetNeighbour(HexDirection.N, cells[i-1]);
        } if (x > 0) 
        {
            if ((x & 1) == 0)
            {
                /* 
                Next we connect NE to SW on even columns (after column 0).
                */
                cell.SetNeighbour(HexDirection.SW, cells[i-height]);

                if (z > 0) 
                {
                    /* 
                    We can connect to the NW neighbours as well, except for the first 
                    cell in each column (because it doesn't have one).
                    */
                    cell.SetNeighbour(HexDirection.NW, cells[i-height-1]);
                }
            } else {
                /*
                The odd rows follow the same logic, but mirrored....
                */
                cell.SetNeighbour(HexDirection.NW, cells[i-height]);

                if (z < height - 1) 
                {
                    cell.SetNeighbour(HexDirection.SW, cells[i-height+1]);
                }
            } 
        }

        Text label = labels[i] = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(
            position.x,
            position.z
        );
        UpdateLabelText(i);
        
        cell.uiRect = label.rectTransform;
    }

    // Start is called before the first frame update
    void Start()
    {
        hexMesh.Triangulate(cells);
        previousOverlayMode = activeOverlayMode;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            // Debug.Log("Input.GetKeyDown(\"space\")");
            IncrementOverlayMode();
        }
        if (activeOverlayMode != previousOverlayMode) {
            /* the overlay mode has changed! */
            // Debug.Log("overlay mode: " + overlayModes[activeOverlayMode]);
            UpdateAllLabelText();
            previousOverlayMode = activeOverlayMode;
        }
    }
	
	public HexCell GetCellByPosition (Vector3 position) 
    {
		position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		return cells[CoordinatesToIndex(coordinates)];
		// Debug.Log("touched cell" + coordinates.ToString() + " at index: " + CoordinatesToIndex(coordinates));
	}
    int CoordinatesToIndex(HexCoordinates coordinates)
    {
        /*
        This is wrapping vertically!
        */
        int index = coordinates.Z + coordinates.X * width + coordinates.X / 2;
        return index;
    }
    void IncrementOverlayMode() 
    {
        activeOverlayMode = (activeOverlayMode+1)%overlayModes.Length;
        Debug.Log(activeOverlayMode);
    }
    void UpdateLabelText(int i)
    {
        Text label = labels[i];
        HexCell cell = cells[i];
        if ( overlayModes[activeOverlayMode] == "coordinates" ) {
            label.enabled = true;
            label.text = cell.coordinates.ToStringOnSeparateLines();
        } else if ( overlayModes[activeOverlayMode] == "index" ) {
            label.enabled = true;
            label.text = i.ToString();
        } else {
            /* Else default to no label */
            label.enabled = false;
        }
    }
    void UpdateAllLabelText()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            UpdateLabelText(i);
        }
    }
	public void Refresh () {
		hexMesh.Triangulate(cells);
	}
}
