using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public StreetViewCamera[] cameras;
    public float speed = 3.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if ( Input.GetMouseButton(0) && !isShiftKeyDown ) {
            // http://answers.unity.com/answers/1375115/view.html
            float x = Input.GetAxis("Mouse Y") * speed;
            float y = -Input.GetAxis("Mouse X") * speed;
            Vector3 rotationDelta = new Vector3(x, y, 0);

            foreach (StreetViewCamera cam in cameras)
            {
                cam.AddRotation(rotationDelta);
            }
            // Debug.Log(rotationDelta);
        }
    }
}
