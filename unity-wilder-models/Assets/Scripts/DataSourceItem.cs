using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataSourceItem : MonoBehaviour
{
	public NewMapFromDataMenu menu;
	public string DataName {
		get {
			return dataName;
		}
		set {
			dataName = value;
			transform.GetChild(0).GetComponent<Text>().text = value;
		}
	}
	string dataName;
	public void Select () {
		menu.SelectItem(dataName);
	}
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
