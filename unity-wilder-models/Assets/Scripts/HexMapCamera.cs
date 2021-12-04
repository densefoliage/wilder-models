using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
	/*
	This is not a good solution for when we have multiple cameras
	but it works okay for now!
	*/
	static HexMapCamera instance;
	
	public float yOffset = 2f;
	public float stickMinZoom = -10f;
    public float stickMaxZoom = -25f;
    public float swivelMinZoom = 45f;
    public float swivelMaxZoom = 80f;
    public float moveSpeedMinZoom = 10f;
    public float moveSpeedMaxZoom = 40f;
    public float rotationSpeed = 180f;
    public HexGrid grid;
	public static bool Locked {
		set {
			instance.enabled = !value;
		}
	}
    Transform stick, swivel;

	public Camera Camera {
		get {
			return this.GetComponentInChildren<Camera>();
		}
	}
    float zoom = 0f;
    float rotationAngle;
	void OnEnable () {
		instance = this;
	}
	void Awake () {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);

		SetYOffset(yOffset);
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
			if ( Input.GetKey(KeyCode.LeftShift) ) {
				AdjustYOffset(zoomDelta);
			} else {
				AdjustZoom(zoomDelta);
			}
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

		setStickPosition(zoom);
		setSwivelRotation(zoom);
	}
	void setStickPosition(float zoom) {
		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);
	}
	void setSwivelRotation(float zoom) {
		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}
	void AdjustRotation (float delta) {
		float angle = rotationAngle + (delta * rotationSpeed * Time.deltaTime);
		SetRotationAngle(angle);
	}

	void SetRotationAngle (float angle) {
		if (angle < 0f) {
			angle += 360f;
		}
		else if (angle >= 360f) {
			angle -= 360f;
		}
		rotationAngle = angle;
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

	void AdjustYOffset (float yDelta) {
		yOffset = yOffset + Mathf.Clamp(yDelta, -0.5f, 0.5f);
		SetYOffset(yOffset);
	}
	void SetYOffset (float yOffset) {
		Vector3 position = transform.localPosition;
		Vector3 newPosition = new Vector3(position.x, yOffset, position.z);
        transform.localPosition = newPosition;
	}
	Vector3 ClampPosition (Vector3 position) {
		float xMax =
			(grid.cellCountX - 1f) *
			(1.5f * HexMetrics.OUTER_RADIUS);
		position.x = Mathf.Clamp(position.x, 0f, xMax);
		float zMin =
			(grid.cellCountZ - 0.5f) *
			(2f * HexMetrics.INNER_RADIUS * -1f);
		position.z = Mathf.Clamp(position.z, zMin, 0f);
		return position;
	}
	public static void ValidatePosition () {
		instance.AdjustPosition(0f, 0f);
	}

	public static Transform GetTransform() {
		return instance.transform;
	}
	public void Save (BinaryWriter writer) {
		/*
		Rig Position
		*/
		writer.Write(transform.localPosition.x);
		writer.Write(transform.localPosition.y);
		writer.Write(transform.localPosition.z);

		/*
		Rig Rotation
		*/
		writer.Write(transform.localRotation.eulerAngles.y);

		/*
		Min Max
		*/
		writer.Write(stickMinZoom);
		writer.Write(stickMaxZoom);
		writer.Write(swivelMinZoom);
		writer.Write(swivelMaxZoom);

		/*
		Zoom Level
		*/
		writer.Write(zoom);

	}

	public void Load (BinaryReader reader) {
		/*
		Rig Position
		*/
		float xPos = reader.ReadSingle();
		yOffset = reader.ReadSingle();
		float zPos = reader.ReadSingle();
		transform.localPosition = new Vector3(xPos, yOffset, zPos);

		/*
		Rig Rotation
		*/
		float yRotation = reader.ReadSingle();
		SetRotationAngle(yRotation);

		/*
		Min Max
		*/
		stickMinZoom = reader.ReadSingle();
		stickMaxZoom = reader.ReadSingle();
		swivelMinZoom = reader.ReadSingle();
		swivelMaxZoom = reader.ReadSingle();

		/*
		Zoom Level
		*/
		zoom = reader.ReadSingle();
		setStickPosition(zoom);
		setSwivelRotation(zoom);
	}
}
