using UnityEngine;
using System.Collections;

public class MenuObject : MonoBehaviour {
	public bool isQuit = false;
	void OnMouseEnter(){
	
		renderer.material.color = Color.red;
		
	}
	
	void OnMouseExit(){
		renderer.material.color = Color.white;
		
	}
	
	void OnMouseDown(){
		if(isQuit){
			Application.Quit();	
		}
		else{
			Application.LoadLevel(1);
		}
	}
}
