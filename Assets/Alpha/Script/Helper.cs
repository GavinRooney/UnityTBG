//using System;
using UnityEngine;

public static class Helper{
	
	public struct ClipPlanePoints{
	
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
		
		
	}
	
	public static float ClampAngle(float angle, float min, float max){
		do {
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
			
		} while (angle < -360 || angle > 360);
	
		return Mathf.Clamp(angle,min,max);
	}

	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos){
		var clipPlanePoints = new ClipPlanePoints();
		
		if(Camera.mainCamera == null)
			return clipPlanePoints;
		
		var transform = Camera.mainCamera.transform;
		var halfFOV = (Camera.mainCamera.fieldOfView /2 ) * Mathf.Deg2Rad;
		var aspect = Camera.mainCamera.aspect;
		var distance = Camera.mainCamera.nearClipPlane;
		var height = distance * Mathf.Tan(halfFOV);
		var width = height * aspect;
		
		
		clipPlanePoints.LowerRight = pos + transform.right * width;
		clipPlanePoints.LowerRight -= transform.up * height;
		clipPlanePoints.LowerRight += transform.forward * distance;
		
		clipPlanePoints.LowerLeft = pos - transform.right * width;
		clipPlanePoints.LowerLeft -= transform.up * height;
		clipPlanePoints.LowerLeft += transform.forward * distance;
		
		clipPlanePoints.UpperRight = pos + transform.right * width;
		clipPlanePoints.UpperRight += transform.up * height;
		clipPlanePoints.UpperRight += transform.forward * distance;
		
		clipPlanePoints.UpperLeft = pos - transform.right * width;
		clipPlanePoints.UpperLeft += transform.up * height;
		clipPlanePoints.UpperLeft += transform.forward * distance;
		
		
		return clipPlanePoints;
	}

}


