using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCSV : MonoBehaviour
{
    /*
    https://www.youtube.com/watch?v=C37C2yCUlCM
    */
    public DataItem blankItem;
    public List<DataItem> hexDatabase = new List<DataItem>();

    public void LoadHexData() {

        // Clear database
        hexDatabase.Clear();

        // Read CSV files
        List<Dictionary<string, object>> data = CSVReader.Read("ewzClip");
        for (int i = 0; i < data.Count; i++)
        {
            int fid = int.Parse(data[i]["fid"].ToString(), System.Globalization.NumberStyles.Integer);
        }

    }
}
