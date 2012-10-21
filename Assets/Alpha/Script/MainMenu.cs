using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	void OnMouseEnter(){
	
		renderer.material.color = Color.red;
		
	}
	
	void OnMouseExit(){
		renderer.material.color = Color.white;
		
	}
}
