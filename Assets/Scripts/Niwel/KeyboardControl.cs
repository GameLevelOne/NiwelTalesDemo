using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour {

	public Vector2 direction;
	public float dx = 0f;
	public float dy = 0f;
	public bool flipX = false;
	public bool canInteract = false;
	public bool canPull = false;
	public bool canHide = false;

	bool startHiding = false;

	float timer = 0f;

	void Update ()
	{
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			dx = 1f;
			dy = 0f;
			flipX = false;
		} else if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			dx = -1f;
			dy = 0f;
			flipX = true;
		} else if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
			dx = 0f;
			dy = 1f;
			flipX = false;
		} else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
			dx = 0f;
			dy = -1f;
			flipX = false;
		} else if (Input.GetKey (KeyCode.Space) && !canInteract) {
			Debug.Log ("press space");
			dx = 0f;
			dy = 0f;
			canInteract = true;
		} else {
			dx = 0f;
			dy = 0f;
		}

		if (canInteract && !canPull) {
			DisableInteraction ();
		}

		if (Input.GetKeyUp (KeyCode.Space)) {
			if (canPull) {
				canInteract = false;
				flipX = !canPull;
			}
		}

		if(Input.GetKeyDown(KeyCode.W)){
			canHide = !canHide;
		}
	}

	void DisableInteraction(){
		if(timer >= 14f/15f){
			canInteract = false;
			timer = 0;
		} else{
			timer += Time.deltaTime;
		}
	}
}
