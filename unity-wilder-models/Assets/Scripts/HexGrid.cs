using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int cellCountX = 20, cellCountZ = 15;
    public HexGridChunk chunkPrefab;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public Texture2D noiseSource;
    public LoadCSV loadCSV;
    public HexMapCamera cameraRig;
    int chunkCountX, chunkCountZ;
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
		CreateMap(cellCountX, cellCountZ);
	}

    public bool CreateMap (int x, int z, string fileName) {

		if (
			x <= 0 || x % HexMetrics.CHUNK_SIZE_X != 0 ||
			z <= 0 || z % HexMetrics.CHUNK_SIZE_Z != 0
		) {
			Debug.LogError("Unsupported map size.");
			return false;
		}

        List<HexDatum> data = loadCSV.LoadHexData(fileName);

        if (
            x * z != data.Count
        ) {
            Debug.LogError("Map size doesn't match data! m: " + (x * z) + " d: " + data.Count);
            loadCSV.ClearHexData();

			return false;
        }

		if (chunks != null) {
			for (int i = 0; i < chunks.Length; i++) {
				Destroy(chunks[i].gameObject);
			}
		}

        cellCountX = x;
		cellCountZ = z;

		chunkCountX = cellCountX / HexMetrics.CHUNK_SIZE_X;
		chunkCountZ = cellCountZ / HexMetrics.CHUNK_SIZE_Z;

        CreateChunks();
		CreateCells();

        int numWaterCells = 0;
        float waterLevel = 0f;

        for (int i = 0; i < cells.Length; i++)
        {   
            HexCell cell = cells[i];
            HexDatum datum = data[i];

            if ( datum.IsWater || datum.IsRoad ) {
                cell.Elevation = datum.TerrainElevation;
            } else {
                cell.Elevation = datum.SurfaceElevation;
            }

            if ( datum.IsRoad ) {
                Debug.Log(datum.RoadType);
                if ( datum.IsWater ) {
                    // BRIDGE
                    cell.TerrainTypeIndex = 5; // CONCRETE
                } else if ( datum.RoadType == "road" ) {
                    cell.TerrainTypeIndex = 4; // TARMAC PATH
                } else if ( datum.RoadType == "track" ) {
                    cell.TerrainTypeIndex = 5; // CONCRETE
                } else if ( datum.IsForest || datum.IsHedge ) {
                    cell.TerrainTypeIndex = 3; // MUD PATH
                } else {
                    cell.TerrainTypeIndex = 2; // GRASS PATH
                }

            } else if ( datum.IsWater ) {
                numWaterCells++;
                waterLevel += datum.TerrainElevation;
                cell.TerrainTypeIndex = 1; // MUD
                cell.Elevation -= 1f ;

            } else if ( datum.IsForest ) {
                Debug.Log(datum.LandCover);
                if ( datum.LandCover == "deciduous woodland" ) {
                    cell.TerrainTypeIndex = 8; // LIGHT FOREST
                } else if ( datum.LandCover == "coniferous woodland" ) {
                    cell.TerrainTypeIndex = 8; // DARK FOREST
                } else {
                    cell.TerrainTypeIndex = 8; // LIGHT FOREST
                }
                if ( !datum.IsWater ) {
                    cell.Elevation += 2;
                }
            } else if ( datum.IsHedge ) {
                cell.TerrainTypeIndex = 7; // HEDGE
                cell.Elevation += 2;
            } else if ( datum.LandCover == "urban" || datum.LandCover == "suburban" ) {
                if ( datum.HasSurfaceFeature ) {
                    cell.TerrainTypeIndex = 6; // BRICK
                } else { 
                    cell.TerrainTypeIndex = 5; // CONCRETE
                }
            } else {
                if ( datum.HasSurfaceFeature ) {
                    cell.TerrainTypeIndex = 7; // HEDGE
                }
                else if ( datum.TerrainSlope > 20 ) {
                    cell.TerrainTypeIndex = 1; // MUD
                }
                 else {
                    cell.TerrainTypeIndex = 0; // GRASS
                }
            }
        }

        waterLevel = waterLevel / numWaterCells;
        Debug.Log(waterLevel);

        for (int i = 0; i < cells.Length; i++) {
            HexCell cell = cells[i];

            cell.WaterLevel = waterLevel;
        }

        return true;
    }

	public bool CreateMap (int x, int z) {

		if (
			x <= 0 || x % HexMetrics.CHUNK_SIZE_X != 0 ||
			z <= 0 || z % HexMetrics.CHUNK_SIZE_Z != 0
		) {
			Debug.LogError("Unsupported map size.");
			return false;
		}

		if (chunks != null) {
			for (int i = 0; i < chunks.Length; i++) {
				Destroy(chunks[i].gameObject);
			}
		}

        cellCountX = x;
		cellCountZ = z;

		chunkCountX = cellCountX / HexMetrics.CHUNK_SIZE_X;
		chunkCountZ = cellCountZ / HexMetrics.CHUNK_SIZE_Z;

        CreateChunks();
		CreateCells();

        return true;
    }
	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			// HexMetrics.InitializeHashGrid(seed);
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

	/*
	SAVE AND LOAD
	*/
	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}

        cameraRig.Save(writer);
	}

	public void Load (BinaryReader reader, int header) {
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}

		if (x != cellCountX || z != cellCountZ) {
            /*
            If map to load is the same size as current map
            we can skip creating a new map.
            */
			if (!CreateMap(x, z)) {
                /*
                If map creation fails, abort loading
                */
				return;
			}
		}

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader);
		}
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh();
		}

        if (header >= 2) {
            // Load camera position
            cameraRig.Load(reader);
        }
	}
}
