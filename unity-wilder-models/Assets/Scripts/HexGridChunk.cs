using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour {
    public int index;

	HexCell[] cells;

	HexMesh hexMesh;
	Canvas gridCanvas;
    public void AddCell (int index, HexCell cell) {
		cells[index] = cell;
        cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}
	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.CHUNK_SIZE_X * HexMetrics.CHUNK_SIZE_Z];
	}
	public void Refresh () {
		enabled = true;
	}

	void LateUpdate () {
		hexMesh.Triangulate(cells);
		enabled = false;
	}
}