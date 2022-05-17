using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectMenu : MonoBehaviour
{
    public RectTransform listContent;
    public ColorSelectButton buttonPrefab;
    public PaintOnTexture painter;
    string fileContents;
    string[] lines;
    const string FILE_NAME = "colorIndices";

    // Start is called before the first frame update
    void Start()
    {
        TextAsset file = Resources.Load<TextAsset>(FILE_NAME);
        fileContents = file.text;
        lines = fileContents.Split('\n');

        FillList(lines);

        // https://forum.unity.com/threads/ui-scroll-view-and-dynamic-buttons-tutorial.344946/

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectItem(int index) {
        Color color = index == 0 ? Color.black : new Color(index/255f, index/255f, index/255f, 1f);
        painter.selectColor(color);
    }

    void FillList(string[] colors) {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        foreach (string color in colors)
        {
            string[] colorInfo = color.Split(':');
            int index = int.Parse(colorInfo[0]);
            string colorName = colorInfo[1].TrimStart(' ');
            Debug.Log(index + "->" + colorName);

            ColorSelectButton button = Instantiate(buttonPrefab);
            button.menu = this;
            button.index = index;
            button.ColorName = colorName;
            button.transform.SetParent(listContent, false);
        }
    }
}
