using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    public int index;
    HexCell[] cells;
    public HexMesh terrain, streams, roads, water;
    Canvas gridCanvas;
    string activeLabelMode;
    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }
    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HexCell[HexMetrics.CHUNK_SIZE_X * HexMetrics.CHUNK_SIZE_Z];
    }
    public void Refresh()
    {
        enabled = true;
    }

    void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }

    public void SetLabelMode(string mode)
    {
        activeLabelMode = mode;
        UpdateAllLabelText();
    }

    void UpdateLabelText(int i)
    {
        HexCell cell = cells[i];
        Text label = cell.Label;
        if (activeLabelMode == "coordinates")
        {
            gridCanvas.gameObject.SetActive(true);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }
        else if (activeLabelMode == "index")
        {
            gridCanvas.gameObject.SetActive(true);
            label.text = cell.index.ToString();
        }
        else if (activeLabelMode == "chunk")
        {
            gridCanvas.gameObject.SetActive(true);
            label.text = cell.chunk.index.ToString();
        }
        else
        {
            /* Else default to no label */
            gridCanvas.gameObject.SetActive(false);
        }
    }
    void UpdateAllLabelText()
    {
        if (activeLabelMode != null)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                UpdateLabelText(i);
            }
        }
    }

    /**/
    public void Triangulate()
    {
        /*
		Triangulate the whole mesh...
		*/
		terrain.Clear();
		streams.Clear();
        roads.Clear();
        water.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        terrain.Apply();
		streams.Apply();
        roads.Apply();  
        water.Apply(); 
    }

    void Triangulate(HexCell cell)
    {
        /*
		Triangulate a single hex...
		*/
        for (HexDirection d = HexDirection.SE; d <= HexDirection.NE; d++)
        {
            Triangulate(d, cell);
        }
    }
    void Triangulate(HexDirection direction, HexCell cell)
    {
        /*
		Triangulate a slice of the hex...
		*/
        Vector3 center = cell.Position;
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );

        if (cell.HasStream)
        {
            if (cell.HasStreamThroughEdge(direction))
            {
                e.v3.y = cell.StreamBedY;
                if (cell.HasStreamBeginOrEnd)
                {
                    TriangulateWithStreamBeginOrEnd(direction, cell, center, e);
                }
                else
                {
                    TriangulateWithStream(direction, cell, center, e);
                }
            }
            else
            {
                TriangulateAdjacentToStream(direction, cell, center, e);
            }
        }
        else
        {
            TriangulateWithoutStream(direction, cell, center, e);
        }

        if (direction <= HexDirection.SW)
        {
            TriangulateConnection(direction, cell, e);
        }

		if (cell.IsUnderwater) {
			TriangulateWater(direction, cell, center);
		}
    }
	void TriangulateWithoutStream (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		TriangulateEdgeFan(center, e, cell.Color);
		
		if (cell.HasRoads) {
            Vector2 interpolators = GetRoadInterpolators(direction, cell);
			TriangulateRoad(
				center,
				Vector3.Lerp(center, e.v1, interpolators.x),
				Vector3.Lerp(center, e.v5, interpolators.y),
				e, cell.HasRoadThroughEdge(direction)
			);
		}
	}
    void TriangulateWithStreamBeginOrEnd(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(center, e.v1, 0.5f),
            Vector3.Lerp(center, e.v5, 0.5f)
        );
        m.v3.y = e.v3.y;
        TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
        TriangulateEdgeFan(center, m, cell.Color);

        if (!cell.IsUnderwater) {
            bool reversed = cell.HasIncomingStream;
            TriangulateStreamQuad(
                m.v2, m.v4, e.v2, e.v4, cell.StreamSurfaceY, 0.6f, reversed
            );

            center.y = m.v2.y = m.v4.y = cell.StreamSurfaceY;
            streams.AddTriangle(center, m.v2, m.v4);
            if (reversed) {
                streams.AddTriangleUV(
                    new Vector2(0.5f, 0.4f),
                    new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
                );
            }
            else {
                streams.AddTriangleUV(
                    new Vector2(0.5f, 0.4f),
                    new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
                );
            }
        }
    }
    void TriangulateWithStream(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
        Vector3 centerL, centerR;
        if (cell.HasStreamThroughEdge(direction.Opposite()))
        {
            centerL = center +
                HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
            centerR = center +
                HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
        }
        else if (cell.HasStreamThroughEdge(direction.Next()))
        {
            centerL = center;
            centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
        }
        else if (cell.HasStreamThroughEdge(direction.Previous()))
        {
            centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
            centerR = center;
        }
        else if (cell.HasStreamThroughEdge(direction.Next2()))
        {
            centerL = center;
            centerR = center +
                HexMetrics.GetSolidEdgeMiddle(direction.Next()) *
                (0.5f * HexMetrics.INNER_TO_OUTER);
        }
        else
        {
            centerL = center +
                HexMetrics.GetSolidEdgeMiddle(direction.Previous()) *
                (0.5f * HexMetrics.INNER_TO_OUTER);
            centerR = center;
        }
        center = Vector3.Lerp(centerL, centerR, 0.5f);

        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(centerL, e.v1, 0.5f),
            Vector3.Lerp(centerR, e.v5, 0.5f),
            1f / 6f
        );
        m.v3.y = center.y = e.v3.y;

        TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
        terrain.AddTriangle(centerL, m.v1, m.v2);
        terrain.AddTriangleColor(cell.Color);
        terrain.AddQuad(centerL, center, m.v2, m.v3);
        terrain.AddQuadColor(cell.Color);
        terrain.AddQuad(center, centerR, m.v3, m.v4);
        terrain.AddQuadColor(cell.Color);
        terrain.AddTriangle(centerR, m.v4, m.v5);
        terrain.AddTriangleColor(cell.Color);

        if (!cell.IsUnderwater) {
            bool reversed = cell.IncomingStream == direction;
            TriangulateStreamQuad(
                centerL, centerR, m.v2, m.v4, cell.StreamSurfaceY, 0.4f, reversed
            );
            TriangulateStreamQuad(
                m.v2, m.v4, e.v2, e.v4, cell.StreamSurfaceY, 0.6f, reversed
            );
        }
    }
    void TriangulateAdjacentToStream(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
		if (cell.HasRoads) {
			TriangulateRoadAdjacentToRiver(direction, cell, center, e);
		}

        if (cell.HasStreamThroughEdge(direction.Next()))
        {
            if (cell.HasStreamThroughEdge(direction.Previous()))
            {
                center += HexMetrics.GetSolidEdgeMiddle(direction) *
                    (HexMetrics.INNER_TO_OUTER * 0.5f);
            }
            else if (
                cell.HasStreamThroughEdge(direction.Previous2())
            )
            {
                center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
            }
        }
        else if (
            cell.HasStreamThroughEdge(direction.Previous()) &&
            cell.HasStreamThroughEdge(direction.Next2())
        )
        {
            center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
        }
        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(center, e.v1, 0.5f),
            Vector3.Lerp(center, e.v5, 0.5f)
        );

        TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
        TriangulateEdgeFan(center, m, cell.Color);
    }
    void TriangulateConnection(
        HexDirection direction, HexCell cell, EdgeVertices e1
    )
    {
        HexCell neighbour = cell.GetNeighbour(direction);
        if (neighbour == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbour.Position.y - cell.Position.y;
        EdgeVertices e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v5 + bridge
        );

        if (cell.HasStreamThroughEdge(direction))
        {
            e2.v3.y = neighbour.StreamBedY;

            if (!cell.IsUnderwater && !neighbour.IsUnderwater) {
                TriangulateStreamQuad(
                    e1.v2, e1.v4, e2.v2, e2.v4,
                    cell.StreamSurfaceY, neighbour.StreamSurfaceY, 0.8f,
                    cell.HasIncomingStream && cell.IncomingStream == direction
			);
            }
        }

        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbour, cell.HasRoadThroughEdge(direction));
        }
        else
        {
            TriangulateEdgeStrip(e1, cell.Color, e2, neighbour.Color,
				cell.HasRoadThroughEdge(direction)
            );
        }

        HexCell nextNeighbour = cell.GetNeighbour(direction.Next());
        if (direction <= HexDirection.S && nextNeighbour != null)
        {
            Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbour.Position.y;

            if (cell.Elevation <= neighbour.Elevation)
            {
                if (cell.Elevation <= nextNeighbour.Elevation)
                {
                    TriangulateCorner(e1.v5, cell, e2.v5, neighbour, v5, nextNeighbour);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbour, e1.v5, cell, e2.v5, neighbour);
                }
            }
            else if (neighbour.Elevation <= nextNeighbour.Elevation)
            {
                TriangulateCorner(e2.v5, neighbour, v5, nextNeighbour, e1.v5, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbour, e1.v5, cell, e2.v5, neighbour);
            }
        }
    }
    void TriangulateEdgeTerraces(
        EdgeVertices begin, HexCell beginCell,
        EdgeVertices end, HexCell endCell,
		bool hasRoad
    )
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

        TriangulateEdgeStrip(begin, beginCell.Color, e2, c2, hasRoad);

        for (int i = 2; i < HexMetrics.TERRACE_STEPS; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2, hasRoad);
        }

        TriangulateEdgeStrip(e2, c2, end, endCell.Color, hasRoad);
    }
    void TriangulateCorner(
        Vector3 bottom, HexCell bottomCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        /*
		TO DO: rename left -> top
		*/
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope)
        {
            if (rightEdgeType == HexEdgeType.Slope)
            {
                TriangulateCornerTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
            else if (rightEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope)
        {
            if (leftEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerCliffTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            if (leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
        }
        else
        {
            terrain.AddTriangle(bottom, left, right);
            terrain.AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
        }
    }
    void TriangulateCornerTerraces(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

        terrain.AddTriangle(begin, v3, v4);
        terrain.AddTriangleColor(beginCell.Color, c3, c4);

        for (int i = 2; i < HexMetrics.TERRACE_STEPS; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);
            terrain.AddQuad(v1, v2, v3, v4);
            terrain.AddQuadColor(c1, c2, c3, c4);
        }

        terrain.AddQuad(v3, v4, left, right);
        terrain.AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
    }
    void TriangulateCornerTerracesCliff(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        float b = Mathf.Abs(1f / (rightCell.Elevation - beginCell.Elevation));
        Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
        Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);

        /*
		Triangulate the steps on the lower half:
		*/
        TriangulateBoundaryTriangle(
            begin, beginCell, left, leftCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            /*
			If the upper half also has steps, triangulate those:
			*/
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            /*
			Otherwise fill the gap with a triangle:
			*/
            terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
            terrain.AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }
    }
    void TriangulateCornerCliffTerraces(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        /*
		This is a mirror of TriangulateCornerTerracesCliff...
		*/
        float b = Mathf.Abs(1f / (leftCell.Elevation - beginCell.Elevation));
        Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
        Color boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

        TriangulateBoundaryTriangle(
            right, rightCell, begin, beginCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
            terrain.AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }
    }

    void TriangulateBoundaryTriangle(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 boundary, Color boundaryColor
    )
    {

        Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        Color c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

        terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
        terrain.AddTriangleColor(beginCell.Color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.TERRACE_STEPS; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
            terrain.AddTriangleUnperturbed(v1, v2, boundary);
            terrain.AddTriangleColor(c1, c2, boundaryColor);
        }

        terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
        terrain.AddTriangleColor(c2, leftCell.Color, boundaryColor);
    }
    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        terrain.AddTriangle(center, edge.v1, edge.v2);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v2, edge.v3);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v3, edge.v4);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v4, edge.v5);
        terrain.AddTriangleColor(color);
    }
    void TriangulateEdgeStrip(
        EdgeVertices e1, Color c1,
        EdgeVertices e2, Color c2,
		bool hasRoad = false
    )
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);

		if (hasRoad) {
			TriangulateRoadSegment(e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4);
		}
    }
	void TriangulateStreamQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y1, float y2, float v, bool reversed
	) {
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		streams.AddQuad(v1, v2, v3, v4);
		if (reversed) {
			streams.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
		}
		else {
			streams.AddQuadUV(0f, 1f, v, v + 0.2f);
		}
	}
	void TriangulateStreamQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y, float v, bool reversed
	) {
		TriangulateStreamQuad(v1, v2, v3, v4, y, y, v, reversed);
	}
	void TriangulateRoadSegment (
		Vector3 v1, Vector3 v2, Vector3 v3,
		Vector3 v4, Vector3 v5, Vector3 v6
	) {
		roads.AddQuad(v1, v2, v4, v5);
		roads.AddQuad(v2, v3, v5, v6);
        roads.AddQuadUV(0f, 1f, 0f, 0f);
		roads.AddQuadUV(1f, 0f, 0f, 0f);
	}
    void TriangulateRoad (
		Vector3 center, Vector3 mL, Vector3 mR, 
        EdgeVertices e, bool hasRoadThroughCellEdge
	) {
		if (hasRoadThroughCellEdge) {
			Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
			TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
			roads.AddTriangle(center, mL, mC);
			roads.AddTriangle(center, mC, mR);
			roads.AddTriangleUV(
				new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
			);
			roads.AddTriangleUV(
				new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
			);
		}
		else {
			TriangulateRoadEdge(center, mL, mR);
		}
	}
	void TriangulateRoadEdge (Vector3 center, Vector3 mL, Vector3 mR) {
		roads.AddTriangle(center, mL, mR);
		roads.AddTriangleUV(
			new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
	}
	Vector2 GetRoadInterpolators (HexDirection direction, HexCell cell) {
		Vector2 interpolators;
		if (cell.HasRoadThroughEdge(direction)) {
			interpolators.x = interpolators.y = 0.5f;
		}
		else {
			interpolators.x =
				cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
			interpolators.y =
				cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
		}
		return interpolators;
	}
	void TriangulateRoadAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
		bool previousHasStream = cell.HasStreamThroughEdge(direction.Previous());
		bool nextHasStream = cell.HasStreamThroughEdge(direction.Next());
		Vector2 interpolators = GetRoadInterpolators(direction, cell);
		Vector3 roadCenter = center;

		if (cell.HasStreamBeginOrEnd) {
			roadCenter += HexMetrics.GetSolidEdgeMiddle(
				cell.StreamBeginOrEndDirection.Opposite()
			) * (1f / 3f);
		}
		else if (cell.IncomingStream == cell.OutgoingStream.Opposite()) {
			Vector3 corner;
			if (previousHasStream) {
				if (
					!hasRoadThroughEdge &&
					!cell.HasRoadThroughEdge(direction.Next())
				) {
					return;
				}
				corner = HexMetrics.GetSecondSolidCorner(direction);
			}
			else {
				if (
					!hasRoadThroughEdge &&
					!cell.HasRoadThroughEdge(direction.Previous())
				) {
					return;
				}
				corner = HexMetrics.GetFirstSolidCorner(direction);
			}
            roadCenter += corner * 0.5f;
			center += corner * 0.25f;
		}
		else if (cell.IncomingStream == cell.OutgoingStream.Previous()) {
			roadCenter -= HexMetrics.GetSecondCorner(cell.IncomingStream) * 0.2f;
		}
		else if (cell.IncomingStream == cell.OutgoingStream.Next()) {
			roadCenter -= HexMetrics.GetFirstCorner(cell.IncomingStream) * 0.2f;
		}
		else if (previousHasStream && nextHasStream) {
			if (!hasRoadThroughEdge) {
				return;
			}
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) *
				HexMetrics.INNER_TO_OUTER;
			roadCenter += offset * 0.7f;
			center += offset * 0.5f;
		}
		else {
			HexDirection middle;
			if (previousHasStream) {
				middle = direction.Next();
			}
			else if (nextHasStream) {
				middle = direction.Previous();
			}
			else {
				middle = direction;
			}
			if (
				!cell.HasRoadThroughEdge(middle) &&
				!cell.HasRoadThroughEdge(middle.Previous()) &&
				!cell.HasRoadThroughEdge(middle.Next())
			) {
				return;
			}
			roadCenter += HexMetrics.GetSolidEdgeMiddle(middle) * 0.25f;
		}

		Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
		Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
		if (previousHasStream) {
			TriangulateRoadEdge(roadCenter, center, mL);
		}
		if (nextHasStream) {
			TriangulateRoadEdge(roadCenter, mR, center);
		}
	}
	void TriangulateWater (
		HexDirection direction, HexCell cell, Vector3 center
	) {
		center.y = cell.WaterSurfaceY;
		Vector3 c1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondSolidCorner(direction);

		water.AddTriangle(center, c1, c2);

		if (direction <= HexDirection.SW) {
			HexCell neighbour = cell.GetNeighbour(direction);
			if (neighbour == null || !neighbour.IsUnderwater) {
				return;
			}

			Vector3 bridge = HexMetrics.GetBridge(direction);
			Vector3 e1 = c1 + bridge;
			Vector3 e2 = c2 + bridge;

			water.AddQuad(c1, c2, e1, e2);

			if (direction <= HexDirection.S) {
				HexCell nextNeighbour = cell.GetNeighbour(direction.Next());
				if (nextNeighbour == null || !nextNeighbour.IsUnderwater) {
					return;
				}
				water.AddTriangle(
					c2, e2, c2 + HexMetrics.GetBridge(direction.Next())
				);
			}
		}
	}
}