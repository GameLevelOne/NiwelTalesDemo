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
	Fall,
	ClimbLadder,
	MidLadder,
	Hiding,
	Struggle
}

public class Niwel : MonoBehaviour {
	[SerializeField]
	KeyboardControl niwelControl;
	[SerializeField]
	NiwelFlags niwelFlags;
	[SerializeField]
	StressMeter stressMeter;
	[SerializeField]
	HungerMeter hungerMeter;
	[SerializeField]
	CharacterVisualSound visualSound;

	public Fader fader;
	public GameObject gameOverText;
	public float niwelSpeed;

	SpriteRenderer thisSprite;
	Animator thisAnimator;
	Rigidbody2D thisRigidbody;
	NiwelState currentState = NiwelState.Idle;

	GameObject currentColliderObj;
	GameObject currentPushedObj;

	bool canInteract = false;
	bool canHide = false;
	bool canPush = false;
	bool pushRight = false;
	bool isHiding = false;
	bool showVisualSound = false;
	bool isStressed = false;
	bool isStruggling = false;
	bool isDead = false;

	float visualSoundCycleDuration = 0.4f;
	float visualSoundTimer = 0f;
	float currentHeight = 0;

	//ADJUST THESE VALUES LATER
	float heightTolerance = 10f; 
	float fallDamage = 10; 
	float shotDamage = 10;
	float struggleDamage = 10;
	float eatValue = 30;
	float stressValueWhenDropObjectToSoldier = 10;
	float stressValueWhenChasedBySoldier = 10;
	float stressValueWhenHPDecreases = 5;
	int struggleCount = 3;

	int currentStruggleCount = 0;

	InteractableType currentInteractableType;

	Soldier currentSoldierObj;

	void Start(){
		thisSprite = GetComponent<SpriteRenderer> ();
		thisAnimator = GetComponent<Animator> ();
		thisRigidbody = GetComponent<Rigidbody2D> ();
		niwelFlags.InitFlags ();

		if(hungerMeter!=null)
			hungerMeter.OnNiwelDead += OnNiwelDead;
		fader.gameObject.SetActive (true);
		fader.OnFadeOutFinished += Fader_OnFadeOutFinished;
	}

	void OnDisable(){
		if(hungerMeter!=null)
			hungerMeter.OnNiwelDead -= OnNiwelDead;

		fader.OnFadeOutFinished -= Fader_OnFadeOutFinished;
	}

	void Fader_OnFadeOutFinished ()
	{
		gameOverText.SetActive (true);
	}

	void OnNiwelDead ()
	{
		isDead = true;
		fader.FadeOutGameOver ();
	}

	void ChangeAnim(NiwelAnimState state,float animSpeed = 1f){
		thisAnimator.enabled = true;
		thisAnimator.SetInteger ("AnimState", (int)state);
		thisAnimator.speed = animSpeed;
	}

	void UpdateAnimState ()
	{
		if (currentState == NiwelState.Idle) {
			if(niwelControl.canInteract){
				if(canInteract){
					CheckInteractableAnim ();
				} else if(canPush && niwelControl.canPull){
					ChangeAnim (NiwelAnimState.AnimCarry, 0f);
				}
			} else {
				ChangeAnim (NiwelAnimState.AnimIdle);
			}
		} else if (currentState == NiwelState.Walk) {
			if (canPush && niwelControl.canPull && niwelControl.canInteract) {
				ChangeAnim (NiwelAnimState.AnimCarry);
			} else{
				ChangeAnim (NiwelAnimState.AnimWalk);
			}
		} else if (currentState == NiwelState.ClimbLadder) {
			if (niwelControl.dy != 0) {
				ChangeAnim (NiwelAnimState.AnimClimb);
			} 
		} else if(currentState == NiwelState.MidLadder){
			ChangeAnim (NiwelAnimState.AnimClimb,0f);
		} else if(currentState == NiwelState.Hiding){
			if(isHiding){
				ChangeAnim (NiwelAnimState.AnimHide);
			} else{
				ChangeAnim (NiwelAnimState.AnimIdle);
			}
		} else if(currentState == NiwelState.Struggle){
			ChangeAnim (NiwelAnimState.AnimDamaged);
		} else if(currentState == NiwelState.Fall){
			ChangeAnim (NiwelAnimState.AnimFall);
		} else if(isDead){
			ChangeAnim (NiwelAnimState.AnimDamaged);
		}
	}

	void UpdateNiwel ()
	{
		if (currentState == NiwelState.Idle) {
			if (niwelControl.dx != 0) {
				currentState = NiwelState.Walk;
			} else if ((niwelControl.dy > 0) && niwelFlags.GetFlag (NiwelFlag.OnLadder)) {
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				currentState = NiwelState.ClimbLadder;
			} else if ((niwelControl.dy < 0) && niwelFlags.GetFlag (NiwelFlag.LadderUnder)) {
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				currentState = NiwelState.ClimbLadder;
			} else if (!niwelFlags.GetFlag (NiwelFlag.GroundUnder)) {
				currentHeight = transform.localPosition.y;
				currentState = NiwelState.Fall;
			} else if (canHide && niwelControl.canHide && !isHiding) {
				Debug.Log ("start hiding");
				currentState = NiwelState.Hiding;
				isHiding = true;
			} else if (niwelControl.canInteract) {
				if (canInteract) {
					CheckInteractableEffect ();
				} 
			} else if(isStruggling){
				currentState = NiwelState.Struggle;
			}
		} else if (currentState == NiwelState.Fall) {
			Vector3 newPos = new Vector3 (transform.localPosition.x + (transform.localPosition.y * niwelSpeed * Time.deltaTime), 
				                 transform.localPosition.y, transform.localPosition.z);
			transform.localPosition = newPos;

			if (niwelFlags.GetFlag (NiwelFlag.GroundUnder)) {
				float newHeight = transform.localPosition.y;
				Debug.Log ("newHeight:" + newHeight);
				if(Mathf.Abs(currentHeight - newHeight) > heightTolerance){
					Debug.Log ("DAMAGED");
					if(hungerMeter!=null) hungerMeter.ModMeter (-fallDamage);
					if(stressMeter!=null) stressMeter.ModMeter (stressValueWhenHPDecreases);
				}

				currentState = NiwelState.Idle;
			} else if ((niwelControl.dy > 0) && niwelFlags.GetFlag (NiwelFlag.OnLadder)) {
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				currentState = NiwelState.ClimbLadder;
			}

		} else if (currentState == NiwelState.Walk) {
			Vector3 newPos = new Vector3 (transform.localPosition.x + niwelControl.dx * niwelSpeed, transform.localPosition.y, transform.localPosition.z);
			transform.localPosition = newPos;
			thisSprite.flipX = niwelControl.flipX;

			if (niwelControl.dx == 0) {
				currentState = NiwelState.Idle;
			} else if ((niwelControl.dy > 0) && niwelFlags.GetFlag (NiwelFlag.OnLadder)) {
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				currentState = NiwelState.ClimbLadder;
			} else if (!niwelFlags.GetFlag (NiwelFlag.GroundUnder)) {
				currentHeight = transform.localPosition.y;
				currentState = NiwelState.Fall;
			} else if ((niwelControl.dx != 0) && canPush && niwelControl.canInteract) {
				if (niwelControl.canInteract && niwelControl.canPull) {
					//PULL
					if ((niwelControl.dx < 0 && pushRight) || (niwelControl.dx > 0 && !pushRight)) {
						thisSprite.flipX = !niwelControl.flipX;
						Vector3 objPos = new Vector3 (currentPushedObj.transform.localPosition.x + niwelControl.dx * niwelSpeed, 
							currentPushedObj.transform.localPosition.y, currentPushedObj.transform.localPosition.z);
						currentPushedObj.transform.localPosition = objPos;
					} else{
					//PUSH
						if ((niwelControl.dx > 0 && pushRight) || (niwelControl.dx < 0 && !pushRight)) {
							Vector3 objPos = new Vector3 (currentPushedObj.transform.localPosition.x + niwelControl.dx * niwelSpeed, 
								currentPushedObj.transform.localPosition.y, currentPushedObj.transform.localPosition.z);
							currentPushedObj.transform.localPosition = objPos;
						}
					} 
				} 
			} else if(isStruggling){
				currentState = NiwelState.Struggle;
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

			if(niwelControl.dy == 0){
				currentState = NiwelState.MidLadder;
			} else if((niwelControl.dy < 0) && niwelFlags.GetFlag(NiwelFlag.GroundUnder) && !niwelFlags.GetFlag(NiwelFlag.LadderUnder)){
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
				currentState = NiwelState.Idle;
			} else if((niwelControl.dy >0) && !niwelFlags.GetFlag(NiwelFlag.OnLadder) ){
				thisRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
				currentState = NiwelState.Idle;
			}
		} else if(currentState == NiwelState.MidLadder){
			if(niwelControl.dy != 0){
				currentState = NiwelState.ClimbLadder;
			}
		} else if(currentState == NiwelState.Hiding){
			if(!niwelControl.canHide){
				isHiding = false;
			} 

			if(!isHiding){
				thisRigidbody.simulated = true;
				currentState = NiwelState.Idle;
				if(isStruggling){
					currentState = NiwelState.Struggle;
				}
			} else{
				thisRigidbody.simulated = false;
			}

		} else if(currentState == NiwelState.Struggle){
			if(!isStruggling){
				currentState = NiwelState.Idle;
			} else{
				if(niwelControl.canInteract){
					if(hungerMeter!=null) hungerMeter.ModMeter (-struggleDamage); 
					if(stressMeter!=null) stressMeter.ModMeter (stressValueWhenHPDecreases);
					currentStruggleCount++;
					if(currentStruggleCount >= struggleCount){
						currentSoldierObj.ReleaseNiwel ();
						isStruggling = false;
					}

				}
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
			hungerMeter.ModMeter (eatValue);
		Destroy (currentColliderObj);
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

	public void KillNiwel(){
		if(hungerMeter!=null) hungerMeter.ModMeter (-hungerMeter.GetCurrentMeterValue ());
		OnNiwelDead ();
	}

	public void DropObjectToSoldier(){
		if(stressMeter!=null) stressMeter.ModMeter (stressValueWhenDropObjectToSoldier);
	}

	public void ChasedBySoldier(){
		isStressed = true;
		if(stressMeter!=null) stressMeter.StartTickStressMeter (stressValueWhenChasedBySoldier);
	}

	public void NotChasedBySoldier(){
		isStressed = false;
		if(stressMeter!=null) stressMeter.StopTickStressMeter ();
	}

	public void GrabbedBySoldier(Soldier soldierObj){
		isStruggling = true;
		isHiding = false;
		currentSoldierObj = soldierObj;
	}

	public void GrabbedFromHidingPlace(){
		isHiding = false;
		isStruggling = true;
	}

	public bool GetNiwelHideStatus(){
		return isHiding;
	}

	public void NiwelIsShot(){
		if(hungerMeter!=null) hungerMeter.ModMeter (shotDamage);
		if(stressMeter!=null) stressMeter.ModMeter (stressValueWhenHPDecreases);
	}

}
