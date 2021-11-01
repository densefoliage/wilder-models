using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        prevPosition = this.transform.position;
    }

    Vector3 prevPosition;


    // Update is called once per frame
    void Update()
    {
        /*
        TO DO:
        Code to move camera:
            Click-and-drag,
            WASD,
            Scroll to zoom,
        */

        CheckIfCameraMoved();
        
    }

    public void PanToHex ( Hex hex ) 
    {
        /*
        To snap camera to a specific hex.
        */
    }

    void CheckIfCameraMoved()
    {
        if (prevPosition != this.transform.position)
        {
            /*
            Something moved the camera!
            */
            // Debug.Log("Something moved the camera!");
            prevPosition = this.transform.position;

            /*
            Maybe there is a better way to cull objects not in view...
            Probably HexMap will have a dictionary of all these later.
            */
            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();
            foreach (HexComponent hex in hexes)
            {
                hex.UpdatePosition();
            }
        }
    }
}
