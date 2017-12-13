using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NiwelAnimState{
	AnimIdle,
	AnimWalk,
	AnimFall,
	AnimClimb,
	AnimCrawl,
	AnimEat,
	AnimCarry,
	AnimInteract,
	AnimHide,
	AnimDamaged,
	AnimClimbPlatform
}

public enum NiwelState{
	Idle,
	Walk,
	ClimbLadder,
	Hiding
}

public class Niwel : MonoBehaviour {
	[SerializeField]
	KeyboardControl niwelControl;
	[SerializeField]
	StressHungerMeter stressMeter;
	[SerializeField]
	StressHungerMeter hungerMeter;

	public float niwelSpeed;

	SpriteRenderer thisSprite;
	Animator thisAnimator;
	Rigidbody2D thisRigidbody;
	NiwelState currentState = NiwelState.Idle;

	GameObject currentPushedObj;
	GameObject currentFoodObj;

	bool onLadder = false;
	bool canInteract = false;
	bool canHide = false;
	bool canEat = false;
	bool canOpen = false;
	bool canPush = false;
	bool pushRight = false;
	bool isHiding = false;

	void Start(){
		thisSprite = GetComponent<SpriteRenderer> ();
		thisAnimator = GetComponent<Animator> ();
		thisRigidbody = GetComponent<Rigidbody2D> ();

		hungerMeter.OnNiwelDead += OnNiwelDead;
	}

	void OnDisable(){
		hungerMeter.OnNiwelDead -= OnNiwelDead;
	}

	void OnNiwelDead ()
	{
		//TODO: Screen game over?
	}

	void ChangeAnim(NiwelAnimState state,float animSpeed = 1f){
		thisAnimator.enabled = true;
		thisAnimator.SetInteger ("AnimState", (int)state);
		thisAnimator.speed = animSpeed;
	}

	void UpdateAnimState ()
	{
		if (currentState == NiwelState.Idle) {
			if (niwelControl.canInteract) {
				if(canEat){
					ChangeAnim (NiwelAnimState.AnimEat);
				} else if(canInteract){
					ChangeAnim (NiwelAnimState.AnimInteract);
				}
			} else if (canPush) {
				ChangeAnim (NiwelAnimState.AnimCarry, 0f);
			} else {
				ChangeAnim (NiwelAnimState.AnimIdle);
			}
		} else if (currentState == NiwelState.Walk) {
			if (canPush) {
				ChangeAnim (NiwelAnimState.AnimCarry);
			} else{
				ChangeAnim (NiwelAnimState.AnimWalk);
			}
		} else if (currentState == NiwelState.ClimbLadder) {
			if (niwelControl.dy != 0) {
				ChangeAnim (NiwelAnimState.AnimClimb);
			} else {
				ChangeAnim (NiwelAnimState.AnimClimb, 0f);
			}
		} else if(currentState == NiwelState.Hiding){
			if(isHiding){
				ChangeAnim (NiwelAnimState.AnimHide);
			} else{
				ChangeAnim (NiwelAnimState.AnimIdle);
				currentState = NiwelState.Idle;
			}
		}
	}

	void UpdateNiwel ()
	{
		if (currentState == NiwelState.Idle) {
			if (niwelControl.dx != 0) {
				currentState = NiwelState.Walk;
			} else if ((niwelControl.dy > 0) && onLadder) {
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				currentState = NiwelState.ClimbLadder;
			} else if(canHide && niwelControl.canHide && !isHiding){
				Debug.Log ("start hiding");
				currentState = NiwelState.Hiding;
				isHiding = true;
			} else if(niwelControl.canInteract){
				if(canInteract){
					CheckInteractableEffect ();
				} else if(canEat){
					Eat ();
				}
			}
		} else if (currentState == NiwelState.Walk) {
			Vector3 newPos = new Vector3 (transform.localPosition.x + niwelControl.dx * niwelSpeed, transform.localPosition.y, transform.localPosition.z);
			transform.localPosition = newPos;
			thisSprite.flipX = niwelControl.flipX;

			if (niwelControl.dx == 0) {
				currentState = NiwelState.Idle;
			} else if ((niwelControl.dy > 0) && onLadder) {
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				currentState = NiwelState.ClimbLadder;
			} else if ((niwelControl.dx != 0) && canPush) {
				if (niwelControl.canInteract && niwelControl.canPull) {
					thisSprite.flipX = !niwelControl.flipX;
					//PULL
					if ((niwelControl.dx < 0 && pushRight) || (niwelControl.dx > 0 && !pushRight)) {
						Vector3 objPos = new Vector3 (currentPushedObj.transform.localPosition.x + niwelControl.dx * niwelSpeed, 
							                currentPushedObj.transform.localPosition.y, currentPushedObj.transform.localPosition.z);
						currentPushedObj.transform.localPosition = objPos;
					}
				} else{
				//PUSH
					if ((niwelControl.dx > 0 && pushRight) || (niwelControl.dx < 0 && !pushRight)) {
						Vector3 objPos = new Vector3 (currentPushedObj.transform.localPosition.x + niwelControl.dx * niwelSpeed, 
							                currentPushedObj.transform.localPosition.y, currentPushedObj.transform.localPosition.z);
						currentPushedObj.transform.localPosition = objPos;
					}
				} 
			} 
		}  else if(currentState == NiwelState.ClimbLadder){
			Vector3 newPos = new Vector3 (transform.localPosition.x, transform.localPosition.y + niwelControl.dy * niwelSpeed, transform.localPosition.z);
			transform.localPosition = newPos;

			if((niwelControl.dy < 0) && !onLadder){
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
				currentState = NiwelState.Idle;
			} 
		} else if(currentState == NiwelState.Hiding){
			if(!niwelControl.canHide){
				isHiding = false;
			}
		}
	}

	void Update(){
		UpdateNiwel ();
		UpdateAnimState ();
	}

	void CheckInteractableEffect(){

	}

	void Eat(){
		//TEMP
		hungerMeter.ModMeter (30);
		Destroy (currentFoodObj);
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == Tags.LADDER){
			Debug.Log ("ENTER LADDER");
			onLadder = true;
		} else if(other.tag == Tags.GROUND){
			onLadder = false;
		} else if(other.tag == Tags.INTERACTABLE){
			canInteract = true;
		} else if(other.tag == Tags.RIGHT_PUSHABLE){
			canPush = true;
			pushRight = false;
			niwelControl.canPull = true;
			currentPushedObj = other.transform.parent.gameObject;
		} else if(other.tag == Tags.LEFT_PUSHABLE){
			canPush = true;
			pushRight = true;
			niwelControl.canPull = true;
			currentPushedObj = other.transform.parent.gameObject;
		} else if(other.tag == Tags.HIDEABLE){
			canHide = true;
		} else if(other.tag == Tags.FOOD){
			canEat = true;
			currentFoodObj = other.gameObject;
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if(other.tag == Tags.LADDER){
			Debug.Log ("CLIMB LADDER");
			onLadder = true;
		} 
	}

	void OnTriggerExit2D(Collider2D other){
		if(other.tag == Tags.LADDER){
			Debug.Log ("EXIT LADDER");
			onLadder = false;
		} else if(other.tag == Tags.INTERACTABLE){
			canInteract = false;
			niwelControl.canPull = false;
		} else if(other.tag == Tags.LEFT_PUSHABLE || other.tag == Tags.RIGHT_PUSHABLE){
			canPush = false;
			currentPushedObj = null;
			niwelControl.canPull = false;
		} else if(other.tag == Tags.HIDEABLE){
			canHide = false;
		}  else if(other.tag == Tags.FOOD){
			canEat = false;
			currentFoodObj = null;
		}
	}
}
