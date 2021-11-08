using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color;
    public float elevation;
    public const float elevationFactor = 5f;

    [SerializeField]
    HexCell[] neighbours;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HexCell GetNeighbour(HexDirection direction) 
    {
        return neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, HexCell cell) 
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }
}
