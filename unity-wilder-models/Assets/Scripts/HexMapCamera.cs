using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    public float swivelMinZoom = 45f;
    public float swivelMaxZoom = 80f;
	public float stickMinZoom = -10f;
    public float stickMaxZoom = -25f;
    public float moveSpeedMinZoom = 10f;
    public float moveSpeedMaxZoom = 40f;
    public float rotationSpeed = 180f;
    public HexGrid grid;
    Transform swivel, stick;
    float zoom = 0f;
    float rotationAngle;
	void Awake () {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);

        AdjustZoom(0);
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom(zoomDelta);
		}

		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f) {
			AdjustRotation(rotationDelta);
		}

        float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition(xDelta, zDelta);
		}
    }
	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

 		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}
	void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}
	void AdjustPosition (float xDelta, float zDelta) {
        Vector3 direction = transform.localRotation *
            new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance =
			Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
			damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = ClampPosition(position);
	}
	Vector3 ClampPosition (Vector3 position) {
		float xMax =
			(grid.chunkCountX * HexMetrics.CHUNK_SIZE_X - 1f) *
			(1.5f * HexMetrics.OUTER_RADIUS);
		position.x = Mathf.Clamp(position.x, 0f, xMax);
		float zMin =
			(grid.chunkCountZ * HexMetrics.CHUNK_SIZE_Z - 0.5f) *
			(2f * HexMetrics.INNER_RADIUS * -1f);
		position.z = Mathf.Clamp(position.z, zMin, 0f);
		return position;
	}
}