using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour {

	public Vector2 direction;
	public float dx = 0f;
	public float dy = 0f;
	public bool flipX = false;

	void Update(){
		if(Input.GetKey(KeyCode.RightArrow)){
			dx = 1f;
			dy = 0f;
			flipX = false;
		} else if(Input.GetKey(KeyCode.LeftArrow)){
			dx = -1f;
			dy = 0f;
			flipX = true;
		} else if(Input.GetKey(KeyCode.UpArrow)){
			dx = 0f;
			dy = 1f;
			flipX = false;
		} else if(Input.GetKey(KeyCode.DownArrow)){
			dx = 0f;
			dy = -1f;
			flipX = false;
		} else{
			dx = 0f;
			dy = 0f;
			flipX = false;
		}
	}
}
