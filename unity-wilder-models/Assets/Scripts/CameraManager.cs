using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public WorldCamera worldCameraPrefab;
    public HexMapCamera hexMapCamera;

    List<WorldCamera> worldCameras;
    List<Camera> cameras;
    Camera mainCam;
    int activeCameraIndex;

    void Awake() 
    {
        mainCam = hexMapCamera.Camera;

        worldCameras = new List<WorldCamera>();
        cameras = new List<Camera>();

        cameras.Add(mainCam);
	}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddWorldCamera() {
        /* 
        This function drops a new camera into the world at the current location
        of the main camera.
        */
        Transform camTransform = mainCam.transform;

        WorldCamera worldCamera = Instantiate<WorldCamera>(
            worldCameraPrefab,
            camTransform.position,
            camTransform.rotation,
            this.transform
            );
        worldCameras.Add(worldCamera);
        worldCamera.name = "World Camera " + worldCameras.Count;

        Camera cameraComponent = worldCamera.GetComponentInChildren<Camera>();
        cameras.Add(cameraComponent);
        cameraComponent.depth = -1;
    }

    public void EnableCamera(int index) {
        Debug.Log(index + "/" + cameras.Count);
        if (index < 0 || index >= cameras.Count) {
            Debug.LogError("INVALID CAM SELECTED");
            return;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            if (i == index) {
                // this is the correct camera
                Debug.Log("Camera" + index + " activated");
                cameras[i].depth = 1;
            } else {
                cameras[i].depth = -1;
            }
        }
    }
    public void IncrementActiveCamera() {
        activeCameraIndex = (activeCameraIndex + 1) % cameras.Count;
        EnableCamera(activeCameraIndex);
    }

    public void ActivateMainCamera() {
        activeCameraIndex = 0;
        EnableCamera(activeCameraIndex);
    }

    public void Save (BinaryWriter writer) {
        hexMapCamera.Save(writer);

        writer.Write((byte)worldCameras.Count);
        for (int i = 0; i < worldCameras.Count; i++)
        {
            worldCameras[i].Save(writer);
        }

        writer.Write((byte)activeCameraIndex);
    }

    public void Load (BinaryReader reader) {
        hexMapCamera.Load(reader);

        int numSavedWorldCameras = reader.ReadByte();

        if ( worldCameras != null ) {
            /*
            If there isn't the right amount of world cameras already,
            remove the existing ones and start again.
            */
            if ( numSavedWorldCameras != worldCameras.Count ) {
                Debug.Log("loading: " + numSavedWorldCameras + " existing:" + worldCameras.Count );
                for (int i = 0; i < worldCameras.Count; i++)
                {
                    Destroy(worldCameras[i].gameObject);
                }

                worldCameras = new List<WorldCamera>();
                cameras = new List<Camera>();

                cameras.Add(mainCam);

                for (int i = 0; i < numSavedWorldCameras; i++)
                {
                    AddWorldCamera();
                }
                Debug.Log("new count: " + worldCameras.Count );
            }
        }
        for (int i = 0; i < numSavedWorldCameras; i++)
        {
            Debug.Log("loading cam: " + i );
            worldCameras[i].Load(reader);
        }



    }



}
