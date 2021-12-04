using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Save (BinaryWriter writer) {
		/*
		Camera Position
		*/
		writer.Write(transform.position.x);
		writer.Write(transform.position.y);
		writer.Write(transform.position.z);

		/*
		Camera Rotation
		*/
        writer.Write(transform.rotation.x);
		writer.Write(transform.rotation.y);
        writer.Write(transform.rotation.z);
        writer.Write(transform.rotation.w);
	}

	public void Load (BinaryReader reader) {
		/*
		Rig Position
		*/
		float xPos = reader.ReadSingle();
		float yPos = reader.ReadSingle();
		float zPos = reader.ReadSingle();
		transform.position = new Vector3(xPos, yPos, zPos);

		/*
		Rig Rotation
		*/
        float xRotation = reader.ReadSingle();
		float yRotation = reader.ReadSingle();
        float zRotation = reader.ReadSingle();
        float wRotation = reader.ReadSingle();
		transform.rotation = new Quaternion(
            xRotation,
            yRotation,
            zRotation,
            wRotation
        );
	}
}
