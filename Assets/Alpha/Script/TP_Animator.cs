using UnityEngine;
using System.Collections;

public class TP_Animator : MonoBehaviour {
	
	public enum Direction{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}
	
	public enum CharacterState{
		Idle, Running, WalkingBackwards, StrafingLeft, StrafingRight, Jumping,
		Falling, Landing, LadderClimbing, Sliding, Using, Dead, ActionLocked
	}
	
	private CharacterState lastState;
	private Transform ladderClimbPoint;
	
	public Vector3 LadderClimbOffset = Vector3.zero;
	public Vector3 PostLadderClimbOffset = Vector3.zero;
	public float LadderClimbStartTime = 0f;
	public float LadderClimbAnchorTime = 2.0f;
	
	private Transform pelvis,mesh1,mesh2,mesh3;
	
	private Vector3 initialPosition = Vector3.zero;
	private Quaternion initialRotation = Quaternion.identity;
	
	public GameObject ragdoll;
	public static TP_Animator Instance;
	public Direction MoveDirection {get; set;}
	public CharacterState State {get; set;}
	public bool IsDead {get;set;}
	private float climbTargetOffset = 0f;
	private float climbInitialTargetHeight = 0f;
	// Use this for initialization
	void Awake () {
		Instance = this;
		
		pelvis = transform.FindChild("Alpha:Hips") as Transform;
		mesh1 = transform.FindChild("AlphaMidResMeshes/Alpha_MidJointsGeo") as Transform;
		mesh2 = transform.FindChild("AlphaMidResMeshes/Alpha_MidLimbsGeo") as Transform;
		mesh3 = transform.FindChild("AlphaMidResMeshes/Alpha_MidTorsoGeo") as Transform;
		initialPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
		DetermineCurrentState();
		ProcessCurrentState();
		
		Debug.Log("Current Char State " + State.ToString());
	}
	
	public void DetermineCurrentMoveDirection(){
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;
		if (TP_Motor.Instance.MoveVector.z > 0)
			forward = true;
		if (TP_Motor.Instance.MoveVector.z < 0)
			backward = true;
		if (TP_Motor.Instance.MoveVector.x > 0)
			right = true;
		if (TP_Motor.Instance.MoveVector.x < 0)
			left = true;
		
		if (forward){
			if(left)
				MoveDirection = Direction.LeftForward;
			else if(right)
				MoveDirection = Direction.RightForward;
			else
				MoveDirection = Direction.Forward;	
				
		}
		else if(backward){
			if(left)
				MoveDirection = Direction.LeftBackward;
			else if(right)
				MoveDirection = Direction.RightBackward;
			else
				MoveDirection = Direction.Backward;	
				
		}
		else if(left){
			MoveDirection = Direction.Left;
		}
		else if (right){
			MoveDirection = Direction.Right;
		}
		else {
			MoveDirection = Direction.Stationary;	
		}
	}
	
	void DetermineCurrentState(){
		if(State == CharacterState.Dead)
			return;
		if(!TP_Controller.CharacterController.isGrounded)
		{
			if(State != CharacterState.Falling && 
				State != CharacterState.Jumping && 
				State != CharacterState.LadderClimbing &&
				State != CharacterState.Landing)
			{
				// we should be falling at this point
				Fall();
			}
		}
		
		if(State != CharacterState.Falling &&
			State != CharacterState.Jumping && 
			State != CharacterState.Landing &&
			State != CharacterState.Using &&
			State != CharacterState.LadderClimbing &&
			State != CharacterState.Sliding){
				
			switch(MoveDirection)
			{
			case Direction.Stationary:
				State = CharacterState.Idle;
				break;
			case Direction.Forward:
				State = CharacterState.Running;
				break;
			case Direction.Backward:
				State = CharacterState.WalkingBackwards;
				break;
			case Direction.Left:
				State = CharacterState.StrafingLeft;
				break;
			case Direction.Right:
				State = CharacterState.StrafingRight;
				break;
			case Direction.LeftForward:
				State = CharacterState.Running;
				break;
			case Direction.RightForward:
				State = CharacterState.Running;
				break;
			case Direction.LeftBackward:
				State = CharacterState.WalkingBackwards;
				break;
			case Direction.RightBackward:
				State = CharacterState.WalkingBackwards;
				break;
				
			}
		}
	}
	
	void ProcessCurrentState(){
		switch(State)
		{
			case CharacterState.Idle:
				Idle();
				break;
			case CharacterState.Running:
				Running();
				break;
			case CharacterState.WalkingBackwards:
				WalkBackwards();
				break;
			case CharacterState.StrafingLeft:
				StrafingLeft();
				break;
			case CharacterState.StrafingRight:
				StrafingRight();
				break;
			case CharacterState.Jumping:
				Jumping();
				break;
			case CharacterState.Falling:
				Falling();
				break;
			case CharacterState.Landing:
				Landing();
				break;
			case CharacterState.LadderClimbing:
				LadderClimbing();
				break;
			case CharacterState.Sliding:
				Sliding();
				break;
			case CharacterState.Using:
				Using();
				break;
			case CharacterState.Dead:
				Dead ();
				break;
			case CharacterState.ActionLocked:
				break;
		
		}
	}
	
	#region Character State Methods
	
	void Idle(){
		animation.CrossFade("idle");	
	}
	void Running(){
		animation.CrossFade("run");	
	}
	void WalkBackwards(){
		animation.CrossFade("walk_backwards");	
	}
	void StrafingLeft(){
		animation.CrossFade("strafe_run_left");	
	}
	void StrafingRight(){
		animation.CrossFade("strafe_run_right");	
	}
	
	void Using(){
		if (!animation.isPlaying){
			State = CharacterState.Idle;
			animation.CrossFade("idle");
		}
	}
	
	void Jumping(){
	
		if ((!animation.isPlaying && TP_Controller.CharacterController.isGrounded) || TP_Controller.CharacterController.isGrounded){
			if (lastState == CharacterState.Running){
				animation.CrossFade("landing_roll");	
			}
			else {
				animation.CrossFade("hard_landing");	
			}
			State = CharacterState.Landing;
		}
		else if(!animation.IsPlaying("falling")){
			State = CharacterState.Falling;
			animation.CrossFade("falling");
			TP_Motor.Instance.IsFalling = true;
		}
		else {
			State = CharacterState.Jumping;
			// help determine if we fell too far 
		}
	}
	
	void Falling(){
		if (TP_Controller.CharacterController.isGrounded){
			if (lastState == CharacterState.Running){
				animation.CrossFade("landing_roll");	
			}
			else {
				animation.CrossFade("hard_landing");	
			}
			
			State = CharacterState.Landing;
		}
	}
	
	void Landing(){
		if (lastState == CharacterState.Running) {
			if (!animation.IsPlaying("landing_roll")){
				State = CharacterState.Running;
				animation.Play("run");
			}
		}
		else {
			if (!animation.IsPlaying("hard_landing")){
				State = CharacterState.Idle;
				animation.Play("idle");
			}
		}
		
		TP_Motor.Instance.IsFalling = false;
	}
	
	void Sliding(){
		if (!TP_Motor.Instance.IsSliding){
		
			State = CharacterState.Idle;
			animation.CrossFade("idle");
		}
	}
	
	void Dead(){
		
		State = CharacterState.Dead;
		
	}
	
	void LadderClimbing(){
		
		if (animation.isPlaying){
			var time = animation["climbup"].time;
			if (time > LadderClimbStartTime && time < LadderClimbAnchorTime){
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
														Mathf.Lerp(transform.rotation.eulerAngles.y,
																	ladderClimbPoint.rotation.eulerAngles.y,
																	(time - LadderClimbStartTime)/ (LadderClimbAnchorTime - LadderClimbStartTime)),
														transform.rotation.eulerAngles.z);	
			
				var ladderClimbOffset = transform.TransformDirection(LadderClimbOffset);
	
				Debug.Log("------------------Current Char Before Y: " + transform.position.y);
				transform.position = Vector3.Lerp(transform.position, new Vector3(ladderClimbPoint.position.x,
																		ladderClimbPoint.position.y + ladderClimbPoint.localScale.y / 2,
																		//			transform.position.y ,
																					ladderClimbPoint.position.z) + ladderClimbOffset , (time - LadderClimbStartTime)/ (LadderClimbAnchorTime - LadderClimbStartTime));
			
				Debug.Log("----------------------Current Char After Y: " + transform.position.y);
			}
			
			TP_Camera.Instance.TargetLookAt.localPosition = new Vector3(TP_Camera.Instance.TargetLookAt.localPosition.x,
																		pelvis.localPosition.y + climbTargetOffset,
																		TP_Camera.Instance.TargetLookAt.localPosition.z);
		}
		else {
			State = CharacterState.Idle;
			animation.CrossFade("idle");
			var postLadderClimbOffset = transform.TransformDirection(PostLadderClimbOffset);
			transform.position = new Vector3(pelvis.position.x, ladderClimbPoint.position.y + ladderClimbPoint.localScale.y / 2, pelvis.position.z) + postLadderClimbOffset; 
			Debug.Log("----------------------Current Char After YpostLadderClimbOffset " +postLadderClimbOffset);
			
			TP_Camera.Instance.TargetLookAt.localPosition = new Vector3(TP_Camera.Instance.TargetLookAt.localPosition.x,
																		climbInitialTargetHeight,
																		TP_Camera.Instance.TargetLookAt.localPosition.z);
		
		}
	}
	#endregion
	
	#region Start Action Method
	
	public void Use(){
	
		State = CharacterState.Using;
		animation.CrossFade("pull");
	}
	
	public void Jump(){
	
		if (!TP_Controller.CharacterController.isGrounded || IsDead || State == CharacterState.Jumping){
			return;
		}
		lastState = State;
		State = CharacterState.Jumping;
		animation.CrossFade("falling");
	}
	
	public void Fall(){
		if (TP_Motor.Instance.VerticalVelocity > -5 || IsDead){
			return;
		}
		lastState = State;
		State = CharacterState.Falling;
		
		TP_Motor.Instance.IsFalling = true;
		
		animation.CrossFade("falling");
	}
	
	public void Slide(){
		State = CharacterState.Sliding;
		animation.CrossFade("falling");
	}
	
	public void LadderClimb(){
		if (!TP_Controller.CharacterController.isGrounded || IsDead || ladderClimbPoint == null) {
			return;
		}
		if (Mathf.Abs(ladderClimbPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 60 ){
			TP_Controller.Instance.Jump();
			return;
			
		}
		
		State = CharacterState.LadderClimbing;
		animation.CrossFade("climbup");
		
		climbTargetOffset = TP_Camera.Instance.TargetLookAt.localPosition.y - pelvis.localPosition.y;
		climbInitialTargetHeight = TP_Camera.Instance.TargetLookAt.localPosition.y;
	}
	

	
	public void Die(){
		
		IsDead = true;
		SetupRagdoll();
		
		Dead();
		
	}
	
	public void Reset(){
		IsDead = false;
		transform.position = initialPosition;
		transform.rotation = initialRotation;
		State = CharacterState.Idle;
		animation.Play("idle");
		ClearRagdoll();
	}
	
	#endregion
	
	public void SetLadderClimbPoint(Transform ladderClimbPoint){
		
		this.ladderClimbPoint = ladderClimbPoint;
		TP_Controller.Instance.ladderClimbEnabled = true;
	}
	
	public void ClearLadderClimbPoint(Transform ladderClimbPoint){
		if(this.ladderClimbPoint == ladderClimbPoint){
			this.ladderClimbPoint = null;
			TP_Controller.Instance.ladderClimbEnabled = false;
		}
	}
	
	void SetupRagdoll(){
		
		if(ragdoll == null){
			ragdoll = GameObject.Instantiate(Resources.Load("RagDollFirstRobot"), transform.position,transform.rotation) as GameObject;
			
		}
		
		var characterPelvis = transform.FindChild("Alpha:Hips");
		var ragdollPelvis = ragdoll.transform.FindChild("Alpha:Hips");
		
		MatchChildrenTransforms(characterPelvis, ragdollPelvis);
		
		//pelvis.renderer.enabled = false;
		mesh1.renderer.enabled = false;
		mesh2.renderer.enabled = false;
		mesh3.renderer.enabled = false;
		TP_Camera.Instance.TargetLookAt  = ragdoll.transform.FindChild("Alpha:Spine");
	}
	
	void ClearRagdoll(){
		if(ragdoll != null){
			GameObject.Destroy(ragdoll);
			ragdoll = null;
			
		}
		//pelvis.renderer.enabled = true;
		mesh1.renderer.enabled = true;
		mesh2.renderer.enabled = true;
		mesh3.renderer.enabled = true;
		TP_Camera.Instance.TargetLookAt = transform.FindChild("targetLookAt");
		
	}
	
	void MatchChildrenTransforms(Transform source, Transform target){
		
		if(source.childCount > 0){
			foreach(Transform sourceTransform in source.transform){
				Transform targetTransform = target.Find(sourceTransform.name);
			
				if (targetTransform != null){
					MatchChildrenTransforms(sourceTransform,targetTransform);
					targetTransform.localPosition = sourceTransform.localPosition;
					targetTransform.localRotation = sourceTransform.localRotation;
				}
			}
		}
	}
	
}
