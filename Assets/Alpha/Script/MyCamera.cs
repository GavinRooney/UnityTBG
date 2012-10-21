using UnityEngine;
using System.Collections;

public class MyCamera : MonoBehaviour {
	
	public float x;
	public float y;
	public Transform target;
	public bool followTarget;
	
	public int speed = 110;
	public float freeMovementTurnSpeed = 10.0f;
	public float freeMovementForwardSpeed = 10.0f;
	public float freeMovementVerticalSpeed = 10.0f;
	public float maxHeight = 30.0f;
	public float minHeight = 2.0f;
	
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20.0f;
	public float yMaxLimit = 80.0f;
	
	public float maxDistanceDelta = 10.0f;
	public float damping = 3.0f;
	
	public Quaternion previousRotation;
	
	public float maxDistance = 50.0f;
	public float minDistance = 10.0f;
	public float timer;
	Vector3 newPosition;
	
	// Use this for initialization
	void Start () {
		Vector3 angles = transform.eulerAngles;
    	x = angles.y;
    	y = angles.x;

	if(target){
		transform.LookAt(target);
	}
	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
	}
	
	void LateUpdate(){
		Vector3 angles = transform.eulerAngles;
    	x = angles.y;
    	y = angles.x;
		
		RaycastHit hit = new RaycastHit() ;

		if (Input.GetButtonDown("Fire1")) {
   
 			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        	// Cast a ray forward from our position for the specified distance
        	if (Physics.Raycast(ray, out hit))
        	{
            	// Add a specified force to the Rigidbody component to the other GameObject
            	if(hit.collider.tag == "Unit"){
            		print("Selected " + hit.collider.gameObject.name);	
					foreach (Transform t in hit.collider.gameObject.transform)
					{
						if (t.gameObject.tag == "UnitCameraMoveToPoint"){
							newPosition =  t.gameObject.transform.position ;
							print(">>>>>>>>>>>>" + newPosition);
							
							Vector3 theForwardDirection = Camera.main.transform.TransformDirection (Vector3.forward);
							theForwardDirection.Normalize ();
							Vector3 distanceVector = new Vector3 (5.0f,	5.0f, 5.0f);
							theForwardDirection = Vector3.Scale(theForwardDirection,distanceVector);
							target.position = theForwardDirection + transform.position;
							
						}
					}
            
            		followTarget = true;
					timer = Time.time;
            	}
            	else {
            		followTarget = false;
            	}
       
        	}
        
  		}
		
		if(followTarget){
			moveTo();
			targetSelectedMovement();
		}
		else {
			freeMovement();
		}
	}
	// Update is called once per frame
	void Update () {
	

		
	}
	
	float ClampAngle (float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	void freeMovement(){
		float zPos = Input.GetAxis("Vertical") * Time.deltaTime * freeMovementForwardSpeed;
		float xPos = Input.GetAxis("Horizontal") * Time.deltaTime * freeMovementTurnSpeed;
		float strafePos = Input.GetAxis("Strafe")* Time.deltaTime * freeMovementForwardSpeed;
		float vertPos = Input.GetAxis("GainVertical")* Time.deltaTime * freeMovementVerticalSpeed;
	
		if (Input.GetButton("Fire2")) {
        	x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        	y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
 			
 			y = ClampAngle(y, yMinLimit, yMaxLimit);
 		       
        	Quaternion rotation2 = Quaternion.Euler(y, x, 0);
			if(rotation2.x > 0.0f){
				transform.rotation = rotation2;
			}
        	
			print("rotation : " + transform.rotation);
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
 

		transform.Translate(strafePos,vertPos,zPos);
	
	}
	
	void moveTo(){
	
	
		float currentDistance = Vector3.Distance(target.transform.position,newPosition);
    	bool closeEnough = false;    
		target.transform.position = Vector3.Lerp (target.transform.position, newPosition,Time.deltaTime * maxDistanceDelta);
	
	}
	
	void targetSelectedMovement() {
		float currentDist = Vector3.Distance(transform.position,target.position);
    	bool closeEnough = false;    
		if (currentDist<80){
			closeEnough = false;
		}
		else {
			transform.position = Vector3.Lerp (transform.position, target.position,Time.deltaTime * maxDistanceDelta);
		}
		Quaternion smoothRotation = Quaternion.LookRotation(target.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * damping);
		
		if(Input.GetAxis("Horizontal") != 0.0f ){
			print(x);
			float xpos = x;
		 	xpos += (-Input.GetAxis("Horizontal"))  * Time.deltaTime * speed;
    		Quaternion rotation = Quaternion.Euler(y, xpos, 0);
    		float currentDistance = Vector3.Distance(transform.position,target.position);
        	Vector3 curVector = new Vector3(0.0f, 0.0f, -currentDistance);
    		Vector3 position = rotation * curVector + target.position;
        
    		transform.rotation = rotation;
    		transform.position = position;
			
			
		}
		
	
		if(Input.GetAxis("Vertical") != 0.0f){
			float zPos = Input.GetAxis("Vertical") * Time.deltaTime * speed;
			float currentDistance = Vector3.Distance(transform.position,target.position);
			if(currentDistance < maxDistance && zPos < 0){
				transform.Translate(0, 0, zPos);
			}
				else if(currentDistance > minDistance && zPos > 0){
				transform.Translate(0, 0, zPos);
			}
		}
		
		if (Input.GetButton("Fire2")) {
			float xpos = x;
			float ypos = y;
        	xpos += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        	ypos -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
 		
 			ypos = ClampAngle(ypos, yMinLimit, yMaxLimit);
 		       
        	Quaternion rotation2 = Quaternion.Euler(ypos, xpos, 0);
        	float currentDistance2 = Vector3.Distance(transform.position,target.position);
        	Vector3 curVector2 = new Vector3(0.0f, 0.0f, -currentDistance2);
        	Vector3 position2 = rotation2 * curVector2 + target.position;
        
        	transform.rotation = rotation2;
        	transform.position = position2;
    	}
	}
}
