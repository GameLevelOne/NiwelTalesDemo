using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NiwelAnimState{
	AnimIdle,
	AnimWalk
}

public enum NiwelState{
	Idle,
	Walk
}

public class Niwel : MonoBehaviour {
	[SerializeField]
	KeyboardControl niwelControl;

	public float niwelSpeed;

	SpriteRenderer thisSprite;
	Animator thisAnimator;
	Rigidbody2D thisRigidbody;
	NiwelState currentState = NiwelState.Idle;

	void Start(){
		thisSprite = GetComponent<SpriteRenderer> ();
		thisAnimator = GetComponent<Animator> ();
		thisRigidbody = GetComponent<Rigidbody2D> ();
	}

	void ChangeAnim(NiwelAnimState state,float animSpeed = 1f){
		thisAnimator.enabled = true;
		thisAnimator.SetInteger ("AnimState", (int)state);
		thisAnimator.speed = animSpeed;
	}

	void UpdateAnimState(){
		if (currentState == NiwelState.Idle) {
			ChangeAnim (NiwelAnimState.AnimIdle);
		} else if (currentState == NiwelState.Walk){
			ChangeAnim (NiwelAnimState.AnimWalk);
		}
	}

	void UpdateNiwel(){
		if(currentState == NiwelState.Idle){
			if(niwelControl.dx != 0f){
				currentState = NiwelState.Walk;
			}
		} else if(currentState == NiwelState.Walk){
			if(niwelControl.dx == 0f){
				currentState = NiwelState.Idle;
			} else{
				Vector3 newPos = new Vector3 (transform.localPosition.x + niwelControl.dx * niwelSpeed, transform.localPosition.y, transform.localPosition.z);
				transform.localPosition = newPos;
				thisSprite.flipX = niwelControl.flipX;
			}
		}
	}

	void Update(){
		UpdateNiwel ();
		UpdateAnimState ();
	}
}
