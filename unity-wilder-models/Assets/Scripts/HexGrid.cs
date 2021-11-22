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
	public Color[] colors;
    public Texture2D noiseSource;

    int cellCountX, cellCountZ;
    HexGridChunk[] chunks;
    HexCell[] cells;
    Text[] labels;
    string[] labelModes = {
        "none",
        "index",
        "coordinates",
        "chunk"
    };
    int activeLabelModeIndex = 0;

    void Awake() 
    {
        HexMetrics.noiseSource = noiseSource;
        // HexMetrics.InitializeHashGrid(seed);
        HexMetrics.colors = colors;

		cellCountX = chunkCountX * HexMetrics.CHUNK_SIZE_X;
		cellCountZ = chunkCountZ * HexMetrics.CHUNK_SIZE_Z;

        CreateChunks();
		CreateCells();
    }
	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			// HexMetrics.InitializeHashGrid(seed);
			HexMetrics.colors = colors;
		}
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
        cell.index = i;
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
        cell.Label = label;
        /*
        Elevation must only be set after the label ui rect has been linked!
        */
        cell.Elevation = 0;
        AddCellToChunk(x, z, cell);
    }
	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.CHUNK_SIZE_X;
		int chunkZ = z / HexMetrics.CHUNK_SIZE_Z;
		
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
    public HexCell GetCellByCoordinates(HexCoordinates coordinates)
    {
		int x = coordinates.X;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		int z = coordinates.Z + x / 2;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
        return cells[CoordinatesToIndex(coordinates)];
    }
    int CoordinatesToIndex(HexCoordinates coordinates)
    {
        /*
        This is wrapping vertically!
        */
        int x = coordinates.X;
        if (x > cellCountX) {
            x = cellCountX;
        } else if (x < 0) {
            x = 0;
        }

        int z = coordinates.Z + x / 2;
        if (z > cellCountZ) {
            z = cellCountZ;
        } else if (z < 0) {
            z = 0;
        }

        int index = z + x * cellCountZ;
        return index;
    }
	public void SelectLabelMode (int index) {
		activeLabelModeIndex = index;
        setChunkLabels ();
	}

    void setChunkLabels () {
        foreach (HexGridChunk chunk in chunks)
        {
            chunk.SetLabelMode(labelModes[activeLabelModeIndex]);
        }
    }
}
