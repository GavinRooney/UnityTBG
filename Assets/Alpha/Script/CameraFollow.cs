using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public Transform Target;
	
	private Rect windowRect = new Rect (20, 320, 120, 250);
	private int selectionGridInt = 0;
	private string[] selectionStrings = {"Arch Guardian 1", "Arch Guardian 2", "Arch Guardian 3", "Arch Guardian 4"};
	
	void LateUpdate(){
	
		transform.position = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
		
		
	}
	
	void OnGUI(){
		
		windowRect = GUI.Window (0, windowRect, WindowFunction, "Units");
		
		if (GUI.RepeatButton (new Rect (450, 25, 100, 30), "Zoom In")) {
		// This code is executed every frame that the RepeatButton remains clicked
			transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
		
		}
		if (GUI.RepeatButton (new Rect (450, 65, 100, 30), "Zoom Out")) {
		// This code is executed every frame that the RepeatButton remains clicked
			transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
		
		}
		
		if (GUI.changed)
		{
			Debug.Log("The toolbar was clicked");

	//		if (0 == selectedToolbar)
	//		{
	//			Debug.Log("First button was clicked");
	//		}
	//		else
	//		{
	//			Debug.Log("Second button was clicked");
	//		}
		}
	}
	
	void WindowFunction (int windowID) {
		// Draw any Controls inside the window here
		
		selectionGridInt = GUI.SelectionGrid (new Rect (5, 25, 110, 80), selectionGridInt, selectionStrings, 1);

	}
}
