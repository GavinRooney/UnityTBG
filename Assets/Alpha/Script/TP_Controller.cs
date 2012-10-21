using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour {
	
	public static  CharacterController CharacterController;
	public static TP_Controller Instance;
	
	public bool ladderClimbEnabled {get;set;}
	
	// Use this for initialization
	void Awake () {
		CharacterController = GetComponent("CharacterController") as CharacterController;
		Instance = this;
		TP_Camera.UseExistingOrCreateNewMainCamera();
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.mainCamera  == null){
			return;
		}
		
		TP_Motor.Instance.ResetMotor();
		
		if(!TP_Animator.Instance.IsDead &&
			TP_Animator.Instance.State != TP_Animator.CharacterState.Using &&
			(TP_Animator.Instance.State != TP_Animator.CharacterState.Landing || TP_Animator.Instance.animation.IsPlaying("landing_roll")) &&
			TP_Animator.Instance.State != TP_Animator.CharacterState.LadderClimbing){
			
			GetLocomotionInput();
			HandleActionInput();
		}
		else if(TP_Animator.Instance.IsDead){
			if(Input.anyKeyDown){
					TP_Animator.Instance.Reset();
			}
			
		}
		
		
		
			
		TP_Motor.Instance.UpdateMotor();
	}
	
	void GetLocomotionInput(){

		var deadZone = 0.1f;
			
		if(Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone){
			TP_Motor.Instance.MoveVector += new Vector3(0,0,Input.GetAxis("Vertical"));
			
		}
		if(Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone){
			TP_Motor.Instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"),0,0);
			
		}
		
		TP_Animator.Instance.DetermineCurrentMoveDirection();
		
	}
	
	void HandleActionInput(){
		if (Input.GetButton("Jump")){
			
			if(ladderClimbEnabled){
				LadderClimb();	
			}
			else {
				Jump();
			}
		}
		if (Input.GetKeyDown(KeyCode.P)){
			Use();	
		}
		
		if (Input.GetKeyDown(KeyCode.F1)){
			Die();	
		}
	}
	
	public void Jump(){
		TP_Motor.Instance.Jump();
		TP_Animator.Instance.Jump();
	}
	
	public void Use(){
	
		TP_Animator.Instance.Use();
		
	}
	
	public void Die(){
		TP_Animator.Instance.Die();	
	}
	
	public void LadderClimb(){
		TP_Animator.Instance.LadderClimb();
	}
		
}
