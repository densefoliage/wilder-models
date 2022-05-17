using UnityEngine;
using UnityEngine.UI;

public class ColorSelectButton : MonoBehaviour {

	public ColorSelectMenu menu;
    public int index;
	
	public string ColorName {
		get {
			return colorName;
		}
		set {
			colorName = value;
			transform.GetChild(0).GetComponent<Text>().text = colorName;
		}
	}
	
	string colorName;
	
	public void Select () {
		Debug.Log(index);
		menu.SelectItem(index);
	}
}