using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour {
    public int index;

	HexCell[] cells;

	HexMesh hexMesh;
	Canvas gridCanvas;
	string activeLabelMode;
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

	public void SetLabelMode (string mode) {
		activeLabelMode = mode;
		UpdateAllLabelText();
	}

    void UpdateLabelText(int i)
    {
			HexCell cell = cells[i];
			Text label = cell.Label;
			if ( activeLabelMode == "coordinates" ) {
				gridCanvas.gameObject.SetActive(true);
				label.text = cell.coordinates.ToStringOnSeparateLines();
			} else if ( activeLabelMode == "index" ) {
				gridCanvas.gameObject.SetActive(true);
				label.text = cell.index.ToString();
			} else if ( activeLabelMode == "chunk" ) {
				gridCanvas.gameObject.SetActive(true);
				label.text = cell.chunk.index.ToString();
			} else {
				/* Else default to no label */
				gridCanvas.gameObject.SetActive(false);
			}
    }
    void UpdateAllLabelText()
    {
		if (activeLabelMode != null) {
			for (int i = 0; i < cells.Length; i++)
			{
				UpdateLabelText(i);
			}
		}
    }
}