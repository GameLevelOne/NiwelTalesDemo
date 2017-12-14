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
	StressMeter stressMeter;
	[SerializeField]
	HungerMeter hungerMeter;
	[SerializeField]
	CharacterVisualSound visualSound;

	public float niwelSpeed;

	SpriteRenderer thisSprite;
	Animator thisAnimator;
	Rigidbody2D thisRigidbody;
	NiwelState currentState = NiwelState.Idle;

	GameObject currentColliderObj;
	GameObject currentPushedObj;

	bool canInteract = false;
	bool onLadder = false;
	bool canHide = false;
	bool canOpen = false;
	bool canPush = false;
	bool pushRight = false;
	bool isHiding = false;
	bool showVisualSound = false;

	float visualSoundCycleDuration = 0.4f;
	float visualSoundTimer = 0f;

	InteractableType currentInteractableType;

	void Start(){
		thisSprite = GetComponent<SpriteRenderer> ();
		thisAnimator = GetComponent<Animator> ();
		thisRigidbody = GetComponent<Rigidbody2D> ();

		if(hungerMeter!=null)
			hungerMeter.OnNiwelDead += OnNiwelDead;
	}

	void OnDisable(){
		if(hungerMeter!=null)
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
			if (niwelControl.canInteract && niwelControl.canInteract) {
				CheckInteractableAnim ();
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
				Debug.Log ("found interactable");
				if(canInteract){
					Debug.Log ("ADASDAS");
					CheckInteractableEffect ();
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

			if(!showVisualSound){
				visualSoundTimer += Time.deltaTime;
				if(visualSoundTimer >= visualSoundCycleDuration){
					showVisualSound = true;
				}
			}else{
				visualSound.ShowVisualSound ();
				visualSoundTimer = 0f;
				showVisualSound = false;
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
		currentInteractableType = currentColliderObj.GetComponent<Interactables> ().type;

		if(currentInteractableType == InteractableType.Food){
			Eat ();
		} else if(currentInteractableType == InteractableType.Door){
			Debug.Log ("open/close door");
			currentColliderObj.GetComponent<Door> ().OpenCloseDoor ();
			currentColliderObj.SetActive (false);
		}
	}

	void CheckInteractableAnim(){
		if(currentInteractableType == InteractableType.Food){
			ChangeAnim (NiwelAnimState.AnimEat);
		} else {
			ChangeAnim (NiwelAnimState.AnimInteract);
		}
	}

	void Eat(){
		//TEMP
		if(hungerMeter!=null)
			hungerMeter.ModMeter (30);
		Destroy (currentColliderObj);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == Tags.LADDER) {
			Debug.Log ("ENTER LADDER");
			onLadder = true;
		} else if (other.tag == Tags.GROUND) {
			onLadder = false;
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if(other.tag == Tags.LADDER){
			Debug.Log ("CLIMB LADDER");
			onLadder = true;
		} 
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == Tags.LADDER) {
			Debug.Log ("EXIT LADDER");
			onLadder = false;
		} 
	}

	public void SetColliderObj(bool canInteract,GameObject obj){
		this.canInteract = canInteract;
		currentColliderObj = obj;
	}

	public void SetPushedObj(GameObject obj,bool canPush,bool pushRight){
		currentPushedObj = obj;
		this.canPush = canPush;
		this.pushRight = pushRight;
		niwelControl.canPull = canPush;
	}

	public void SetHideFlag(bool canHide){
		this.canHide = canHide;
	}
}
