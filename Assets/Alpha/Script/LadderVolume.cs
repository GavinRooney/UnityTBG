using UnityEngine;
using System.Collections;

public class LadderVolume : MonoBehaviour {

	void OnTriggerEnter(){
		TP_Animator.Instance.SetLadderClimbPoint(transform);
		Debug.Log("----------------------entering zone: " + transform.position.y);
	}
	
	void OnTriggerExit(){
		TP_Animator.Instance.ClearLadderClimbPoint(transform);
		Debug.Log("----------------------leaving zone " + transform.position.y);
	}
}
