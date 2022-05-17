using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetViewCamera : MonoBehaviour
{
    public float speed = 3.5f;
    public float cameraBobAmplitude = 0.1f;
    public float cameraBobPeriod = 5f;
    public Camera cam;
    private float X;
    private float Y;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.allowHDR = false;
        cam.allowMSAA = false;

        startPos = new Vector3(0,0,0);
        transform.localPosition = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        // http://answers.unity.com/answers/1375115/view.html
        // if(Input.GetMouseButton(0)) {
        //     transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
        //     X = transform.rotation.eulerAngles.x;
        //     Y = transform.rotation.eulerAngles.y;
        //     transform.rotation = Quaternion.Euler(X, Y, 0);
        // }

        float theta = Time.timeSinceLevelLoad / cameraBobPeriod;
        float distance = cameraBobAmplitude * Mathf.Sin(theta);
        
        transform.localPosition = startPos + ( Vector3.up * distance );
    }

    public void AddRotation(Vector3 rotationDelta) {
        transform.Rotate(rotationDelta);
        float x = transform.rotation.eulerAngles.x;
        float y = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(x, y, 0);
    }
}
