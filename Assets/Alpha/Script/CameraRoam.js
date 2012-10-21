#pragma strict
var speed = 10.0;
var freeMovementTurnSpeed = 10.0;
var freeMovementForwardSpeed = 10.0;
var freeMovementVerticalSpeed = 10.0;
var maxHeight = 30;
var minHeight = 2;

var target : Transform;

var distance = 10.0;

var maxDistance = 50;
var minDistance = 10;

var xSpeed = 250.0;
var ySpeed = 120.0;

var yMinLimit = -20;
var yMaxLimit = 80;

private var x = 0.0;
private var y = 0.0;

// The distance in the x-z plane to the target
var followDistance = 30.0;
var maxDistanceDelta = 1.02;
var damping = 3.0;
var followTarget = false;

var previousRotation ;

function Start () {
    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

	if(target){
		transform.LookAt(target);
	}
	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
}

function LateUpdate () {
	var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
    
	if(target){
//		var smoothRotation = Quaternion.LookRotation(target.position - transform.position);
//		transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * damping);
		if(followTarget){
			moveTo();
		}
		else{
			targetSelectedMovement();
		}
		
	}
	else {
		freeMovement();
	}

	
}

function moveTo(){
	
	//transform.LookAt (target);
	
	var currentDistance = Vector3.Distance(transform.position,target.position);
    var closeEnough = false;    
	if (currentDistance<40){
		closeEnough = false;
	}
	else {
			transform.position = Vector3.Lerp (transform.position, target.position,Time.deltaTime * maxDistanceDelta);
	}
	
	
	var smoothRotation = Quaternion.LookRotation(target.position - transform.position);
	
	transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * damping);
	print("smooth rotation " + smoothRotation);
	if((transform.rotation == previousRotation) && (closeEnough == true)){
		followTarget = false;
		print("follow target" + followTarget);
	}
	previousRotation = transform.rotation;
	//print("prev " + previousRotation);
	//print("current " + transform.rotation);
	//print(closeEnough);
}

function freeMovement(){
	var zPos = Input.GetAxis("Vertical") * Time.deltaTime * freeMovementForwardSpeed;
	var xPos = Input.GetAxis("Horizontal") * Time.deltaTime * freeMovementTurnSpeed;
	var strafePos = Input.GetAxis("Strafe")* Time.deltaTime * freeMovementForwardSpeed;
	var vertPos = Input.GetAxis("GainVertical")* Time.deltaTime * freeMovementVerticalSpeed;
	
	if (Input.GetButton("Fire2")) {
        x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
 		
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
 		       
        var rotation2 = Quaternion.Euler(y, x, 0);
        transform.rotation = rotation2;
    }
    else {
    	transform.Rotate(0, Input.GetAxis("Horizontal")*freeMovementTurnSpeed*Time.deltaTime, 0,Space.World);
   	}
   	if (vertPos > 0 && transform.position.y  > maxHeight){
   		vertPos = 0;
   	}
   	else if(vertPos < 0 && transform.position.y < minHeight){
   		vertPos = 0;
   	}
 
   	//print(transform.position.y);
	transform.Translate(strafePos,vertPos,zPos);
	
}
function targetSelectedMovement() {
		
	x += (-Input.GetAxis("Horizontal")) * xSpeed * 0.02;
    var rotation = Quaternion.Euler(y, x, 0);
    var currentDistance = Vector3.Distance(transform.position,target.position);
        
    var position = rotation * Vector3(0.0, 0.0, -currentDistance) + target.position;
        
    transform.rotation = rotation;
    transform.position = position;
	

	var zPos = Input.GetAxis("Vertical") * Time.deltaTime * speed;
	currentDistance = Vector3.Distance(transform.position,target.position);
	if(currentDistance < maxDistance && zPos < 0){
		transform.Translate(0, 0, zPos);
	}
	else if(currentDistance > minDistance && zPos > 0){
		transform.Translate(0, 0, zPos);
	}
	if (Input.GetButton("Fire2")) {
        x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
 		
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
 		       
        var rotation2 = Quaternion.Euler(y, x, 0);
        var currentDistance2 = Vector3.Distance(transform.position,target.position);
        
        var position2 = rotation2 * Vector3(0.0, 0.0, -currentDistance2) + target.position;
        
        transform.rotation = rotation2;
        transform.position = position2;
    }
}

static function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}

function Update () {
	var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
	
	var hit : RaycastHit;

	if (Input.GetButtonDown("Fire1")) {
   
 		var ray : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        // Cast a ray forward from our position for the specified distance
        if (Physics.Raycast(ray, hit))
        {
            // Add a specified force to the Rigidbody component to the other GameObject
            if(hit.collider.tag == "Unit"){
            	print("Selected " + hit.collider.gameObject.name);
            	target = hit.collider.gameObject.transform;
            	//transform.LookAt(target);
            	followTarget = true;
            }
            else {
            	target = null;
            }
       
        }
        
  	}
        
}

