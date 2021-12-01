using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NewMapFromDataMenu : MonoBehaviour
{
    public HexGrid hexGrid;
    public InputField nameInput, cellCountXInput, cellCountZInput;
    public RectTransform listContent;
    public DataSourceItem itemPrefab;

    string fileName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Open () {
        FillList();

        cellCountXInput.text = "40";
        cellCountZInput.text = "50";

		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
    }
    public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
    }
	string GetSelectedPath () {
        /*
        InputField is locked to alphanumeric content to prevent entry of invalid chars
        */
        string fileName = nameInput.text;
		if (fileName.Length == 0) {
			return null;
		}
		return fileName + ".csv";
	}
	public void SelectItem (string name) {
		nameInput.text = name;
	}

	void FillList () {

		for (int i = 0; i < listContent.childCount; i++) {
			Destroy(listContent.GetChild(i).gameObject);
		}

        string folder = Application.dataPath + "/Resources/";

		string[] paths = Directory.GetFiles(
            folder, "*.csv"
        );

        Array.Sort(paths);

		for (int i = 0; i < paths.Length; i++) {
			DataSourceItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.DataName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
		}

	}

    public void ValidateCellCountX(string value) {
        if (value.Length == 0) {
            cellCountXInput.text = HexMetrics.CHUNK_SIZE_X.ToString();
            return;
        }
        int cellCount = ParseCellCount(value);
        cellCountXInput.text = FloorCellCount(cellCount, HexMetrics.CHUNK_SIZE_X).ToString();
    }
    public void ValidateCellCountZ(string value) {
        if (value.Length == 0) {
            cellCountZInput.text = HexMetrics.CHUNK_SIZE_Z.ToString();
            return;
        }
        int cellCount = ParseCellCount(value);
        cellCountZInput.text = FloorCellCount(cellCount, HexMetrics.CHUNK_SIZE_Z).ToString();
    }

    int ParseCellCount(string inputVal) {
        if (inputVal.Length == 0) {
            return -1;
        }
        return int.Parse(inputVal, System.Globalization.NumberStyles.Integer);
    }

    int FloorCellCount(int cellCount, int chunkSize) {
        return cellCount - (cellCount % chunkSize);
    }


    public void CreateMapFromData() {
		Debug.Log("CREATE MAP FROM DATA");

        string folder = Application.dataPath + "/Resources/";
        string path = GetSelectedPath();

		if (!File.Exists(folder+path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}

        int cellCountX = ParseCellCount(cellCountXInput.text);
        int cellCountZ = ParseCellCount(cellCountZInput.text);

		if (
            cellCountX < HexMetrics.CHUNK_SIZE_X ||
            cellCountZ < HexMetrics.CHUNK_SIZE_Z
        ) {
			Debug.LogError("Map too small!");
			return;
		} else if (
            (cellCountX / HexMetrics.CHUNK_SIZE_X) * 
                (cellCountZ / HexMetrics.CHUNK_SIZE_Z) > HexMetrics.MAX_CHUNKS
        ) {
			Debug.LogError("Map too large!");
			return;
		}

        if (cellCountX % HexMetrics.CHUNK_SIZE_X == 0 && cellCountZ % HexMetrics.CHUNK_SIZE_Z == 0) {
            hexGrid.CreateMap(cellCountX, cellCountZ, Path.GetFileNameWithoutExtension(path));
        }
	}

}
