using UnityEngine;
using System.Collections;

// Attached to menu icon to make them always face the camera
public class MenuIcon : MonoBehaviour {

    public Transform Camera;

	// Use this for initialization
	void Start () {
        Update();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.position, Vector3.up);
	}
}
