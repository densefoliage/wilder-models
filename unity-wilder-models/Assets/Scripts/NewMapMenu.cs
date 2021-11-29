using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    public HexGrid hexGrid;
	public void CreateSmallMap () {
		CreateMap(20, 15);
	}

	public void CreateMediumMap () {
		CreateMap(40, 30);
	}

	public void CreateLargeMap () {
		CreateMap(80, 60);
	}

	public void CreateMapFromData() {
		Debug.Log("CREATE MAP FROM DATA");
		Debug.Log(Application.dataPath);
		string path = Application.dataPath + "/Data/ewzClip.xlsx";
		if (File.Exists(path)) {
			Debug.Log("Loading file at: " + path);
			// Excel xls =  ExcelHelper.LoadExcel(path);

			return;
		}
	}
	public void Open () {
		gameObject.SetActive(true);
        HexMapCamera.Locked = true;
	}
	public void Close () {
		gameObject.SetActive(false);
        HexMapCamera.Locked = false;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	void CreateMap (int x, int z) {
		hexGrid.CreateMap(x, z);
        HexMapCamera.ValidatePosition();
		Close();
	}
}
