using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {
	
	public static TP_Motor Instance;
	public float ForwardSpeed = 20f;
	public float BackwardSpeed = 10f;
	public float StrafingSpeed = 15f;
	public float SlideSpeed = 10f;
	public float Gravity = 21f;
	public float TerminalVelocity = 20f;
	public float FatalFallHeight = 3f;
	public float JumpSpeed = 12f;
	
	public float SlideThreshold = 0.5f;
	public float MaxControllableSlideMagnitude = 0.4f;
	private Vector3 slideDirection;
	private float startFallHeight = 0f; 
	private bool isFalling = false;
	public bool IsFalling{
		get{ return isFalling;}	
		set{
			isFalling = value;
			if (isFalling){
				startFallHeight = transform.position.y;	
			}
			else{
				if(startFallHeight - transform.position.y > FatalFallHeight){
					TP_Controller.Instance.Die();
				}
			}
		}
	}
	public float VerticalVelocity {get; set;}
	public Vector3 MoveVector { get ; set;}
	public bool IsSliding{get;set;}
	// Use this for initialization
	void Awake () {
		Instance = this;
	}
	
	// Update is called once per frame
	public void UpdateMotor () {
	
		SnapAlignCharacterWithCamera();
		ProcessMotion();
	}
	public void ResetMotor(){
		VerticalVelocity = MoveVector.y;
		MoveVector = Vector3.zero;
	}
	void ProcessMotion(){
		if(!TP_Animator.Instance.IsDead){
			MoveVector = transform.TransformDirection(MoveVector);
		}
		else {
			MoveVector = new Vector3(0,MoveVector.y, 0);	
		}
		
		if(MoveVector.magnitude > 1){
			MoveVector = Vector3.Normalize(MoveVector);
		}
		
		ApplySlide();
		
		MoveVector *= MoveSpeed();
		
		MoveVector = new Vector3(MoveVector.x, VerticalVelocity , MoveVector.z);
		
		ApplyGravity();
		
		TP_Controller.CharacterController.Move(MoveVector * Time.deltaTime);
	}
	
	void ApplyGravity(){
	
		if(MoveVector.y > -TerminalVelocity){
			MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime , MoveVector.z);
		}
		if(TP_Controller.CharacterController.isGrounded && MoveVector.y < -1){
			MoveVector = new Vector3(MoveVector.x, -1 , MoveVector.z);
		}
	}
	
	void ApplySlide(){
		if(!TP_Controller.CharacterController.isGrounded){
			return;	
		}
		else{
			slideDirection = Vector3.zero;
			RaycastHit hitInfo;
			
			if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo)){
				if(hitInfo.normal.y < SlideThreshold){
					slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);	
					if(!IsSliding){
						TP_Animator.Instance.Slide();
					}
					IsSliding = true;
				}
				else {
					IsSliding = false;	
				}
			}
			
			if(slideDirection.magnitude < MaxControllableSlideMagnitude){
				MoveVector += slideDirection;
			}
			else {
				MoveVector = slideDirection;	
			}
		}
	}
	
	public void Jump(){
		if(TP_Controller.CharacterController.isGrounded){
			VerticalVelocity = JumpSpeed;	
		}
	}
	void SnapAlignCharacterWithCamera(){
		if (MoveVector.x != 0 || MoveVector.z != 0){
		
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
													Camera.mainCamera.transform.eulerAngles.y ,
													transform.eulerAngles.z);
			
		}
	}
	
	float MoveSpeed(){
		var moveSpeed = 0f;
		
		switch(TP_Animator.Instance.MoveDirection){
		
		case TP_Animator.Direction.Stationary:
				moveSpeed = 0;
				break;
		
		case TP_Animator.Direction.Forward:
				moveSpeed = ForwardSpeed;
				break;
		
		case TP_Animator.Direction.Backward:
				moveSpeed = BackwardSpeed ;
				break;
		
		case TP_Animator.Direction.Left:
				moveSpeed = StrafingSpeed;
				break;
		
		case TP_Animator.Direction.Right:
				moveSpeed = StrafingSpeed;
				break;
		
		case TP_Animator.Direction.LeftForward:
				moveSpeed = ForwardSpeed;
				break;
		
		case TP_Animator.Direction.RightForward:
				moveSpeed = ForwardSpeed;
				break;
		
		case TP_Animator.Direction.LeftBackward:
				moveSpeed = BackwardSpeed;
				break;
		
		case TP_Animator.Direction.RightBackward:
				moveSpeed = BackwardSpeed;
				break;
		}
		if(IsSliding){
			moveSpeed = SlideSpeed;
		}
		
		return moveSpeed;
		
	}
}
