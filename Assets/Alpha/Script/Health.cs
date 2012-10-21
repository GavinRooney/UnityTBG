using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public int MaxHealth = 100;
	public int CurrentHealth = 100;
	public float HealthBarLength;
	// Use this for initialization
	void Start () {
		HealthBarLength = Screen.width / 2;
	}
	
	// Update is called once per frame
	void Update () {
		AdjustCurrentHealth(0);
		
	}
	
	void OnGUI(){
		GUI.Box(new Rect(10,10,HealthBarLength, 20 ), CurrentHealth + "/" + MaxHealth);	
		
		GUI.Button (new Rect (10,50,100,20), new GUIContent ("Click me", "This is the tooltip"));
		GUI.Label (new Rect (10,80,100,20), GUI.tooltip);
		
		
	}
	
	public void AdjustCurrentHealth(int adj){
		CurrentHealth += adj;
		
		if(CurrentHealth < 0){
			CurrentHealth = 0;	
		}
		if (CurrentHealth > MaxHealth ){
			CurrentHealth = MaxHealth;
		}
		if(MaxHealth < 1){
			MaxHealth = 1;	
		}
		
		
		HealthBarLength = (Screen.width/2) * (CurrentHealth / (float)MaxHealth);
	}
}
