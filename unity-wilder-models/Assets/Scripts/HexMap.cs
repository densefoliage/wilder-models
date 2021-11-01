using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    /*
    Set the width (numColumns) and height (numRows) of the map
    in terms of hex tile units.
    */
    public int numColumns = 10;
    public int numRows = 10;

    public GameObject HexPrefab;

    public Material[] HexMaterials;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                /*
                Place a Hex object using Instantiate:
                Instantiate(GameObject, Position, Rotation, Parent)
                */
                Hex h = new Hex( column, row );

                GameObject hexGO =  Instantiate(
                    HexPrefab, 
                    h.Position(),
                    Quaternion.identity,
                    this.transform
                );
                
                /*
                Name the hex something sensible.
                */
                hexGO.name = "Hex_" + h.Q + "_" + h.R + "_" + h.S;

                /*
                Let the HexBehaviour component know to reference the Hex data
                component from this loop.
                */
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().HexMap = this;

                /*
                Update the position of the hex based on camera position (to allow
                for globe-like scrolling) if required.
                */
                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", column, row);

                /*
                Update the coordinate overlay text based on the hex's position
                */
                hexGO.GetComponent<HexComponent>().UpdatePosition();

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[ Random.Range(0, HexMaterials.Length) ];
            }
        }

        /*
        StaticBatchingUtility.Combine( this.gameObject ) can reduce computation 
        batches (and subseqently increase framerate) if the tiles are never going
        to move.
        For some reason this isn't working for me!
        */
        // StaticBatchingUtility.Combine( this.gameObject );

    }
}
