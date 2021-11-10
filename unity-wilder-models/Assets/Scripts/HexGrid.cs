using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4, chunkCountZ = 3;
    public HexGridChunk chunkPrefab;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
	public Color defaultColor = Color.white;
    public Texture2D noiseSource;

    int cellCountX, cellCountZ;
    HexGridChunk[] chunks;
    HexCell[] cells;
    Text[] labels;
    string[] overlayModes = {
        "coordinates",
        "index",
        "chunk",
        "hidden"
    };
    int activeOverlayMode = 2;

    void Awake() 
    {
        HexMetrics.noiseSource = noiseSource;

		cellCountX = chunkCountX * HexMetrics.CHUNK_SIZE_X;
		cellCountZ = chunkCountZ * HexMetrics.CHUNK_SIZE_Z;

        CreateChunks();
		CreateCells();
    }
    void OnEnable () 
    {
		HexMetrics.noiseSource = noiseSource;
	}
	void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int x = 0, i = 0; x < chunkCountX; x++) {
			for (int z = 0; z < chunkCountZ; z++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(this.transform);
                chunk.index = i;
			}
		}
	}
	void CreateCells () {
		cells = new HexCell[cellCountX * cellCountZ];
        labels = new Text[cellCountX * cellCountZ];

		for (int x = 0, i = 0; x < cellCountX; x++) {
			for (int z = 0; z < cellCountZ; z++) {
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
            Quaternion.identity
            );
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
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
                cell.SetNeighbour(HexDirection.SW, cells[i-cellCountZ]);

                if (z > 0) 
                {
                    /* 
                    We can connect to the NW neighbours as well, except for the first 
                    cell in each column (because it doesn't have one).
                    */
                    cell.SetNeighbour(HexDirection.NW, cells[i-cellCountZ-1]);
                }
            } else {
                /*
                The odd rows follow the same logic, but mirrored....
                */
                cell.SetNeighbour(HexDirection.NW, cells[i-cellCountZ]);

                if (z < cellCountZ - 1) 
                {
                    cell.SetNeighbour(HexDirection.SW, cells[i-cellCountZ+1]);
                }
            } 
        }

        Text label = labels[i] = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(
            position.x,
            position.z
        );
        cell.uiRect = label.rectTransform;
        /*
        Elevation must only be set after the label ui rect has been linked!
        */
        cell.Elevation = 0;
        AddCellToChunk(x, z, cell);
        UpdateLabelText(i);
    }
	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.CHUNK_SIZE_X;
		int chunkZ = z / HexMetrics.CHUNK_SIZE_Z;
        cell.Color = Color.Lerp(Color.white, Color.black, (float)(chunkZ + chunkX * chunkCountZ)/(chunkCountX*chunkCountZ));
		
        int chunkIndex = chunkZ + chunkX * chunkCountZ;
        HexGridChunk chunk = chunks[chunkIndex];

		int localX = x - chunkX * HexMetrics.CHUNK_SIZE_X;
		int localZ = z - chunkZ * HexMetrics.CHUNK_SIZE_Z;
		chunk.AddCell(localZ + localX * HexMetrics.CHUNK_SIZE_Z, cell);
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
        int index = coordinates.Z + coordinates.X * cellCountZ + coordinates.X / 2;
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
        } else if ( overlayModes[activeOverlayMode] == "chunk" ) {
            label.enabled = true;
            label.text = cell.chunk.index.ToString();
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
}
