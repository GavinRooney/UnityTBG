using UnityEngine;
using System.Collections;

public class CameraUnit : MonoBehaviour {
	
	public Transform unitTarget;
	// Use this for initialization
	void Start () {
		foreach (Transform t in unitTarget)
					{
						if (t.gameObject.tag == "UnitCameraMoveToPoint"){
							
							/*Vector3 theForwardDirection = Camera.main.transform.TransformDirection (Vector3.forward);
							theForwardDirection.Normalize ();
							Vector3 distanceVector = new Vector3 (5.0f,	5.0f, 5.0f);
							theForwardDirection = Vector3.Scale(theForwardDirection,distanceVector);
							target.position = theForwardDirection + transform.position;*/
							transform.position = t.gameObject.transform.position;
							transform.rotation = t.gameObject.transform.rotation;
						}
					}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
