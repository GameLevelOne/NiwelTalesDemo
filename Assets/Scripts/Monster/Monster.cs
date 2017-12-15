using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MonsterState
{
	Idle,
	Patrol,
	Investigate,
	CheckHidingPlace,
	Chase,
	Eat
}

public enum MonsterAnimationState
{
	Idle,
	Walk,
	Run,
	Eat,
	CheckHidingPlace
}

public class Monster : MonoBehaviour {
	#region attributes
	[Header("Monster Attributes")]
	public Animator thisAnim;
	public Rigidbody2D thisRigidbody;
	public GameObject randomDestinationTargetObj;

	[Header("Custom Attributes")]
	public MonsterState monsterState = MonsterState.Idle;
	public float speed = 1.5f;
	public float monsterAppearDuration = 10f;
	public float idleDuration = 2f;
	public float eatDuration = 5f;
	public float checkHidingPlaceDuration = 2f;
	public float targetNearbyDistanceBeforeAttacking = 0.1f;


	[Header("Do Not Modify")]
	public List<GameObject> hidingPlace = new List<GameObject>();
	[SerializeField] GameObject targetObj;
	[SerializeField] GameObject targetHidingPlace;

	public bool flagInvestigate = false;
	public bool flagCheckHidingPlace = false;
	public bool flagChase = false;
	public bool flagEat = false;


	Vector3 currentViewDirection;
	Vector3 vLeft, vRight;
	float timer;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	/// <summary>
	/// Initialize monster behavior, if facingRight, monster will go to direction --->, else <---
	/// </summary>
	public void Init(bool facingRight)
	{
		vLeft = transform.localScale;
		vRight = new Vector3(transform.localScale.x * -1f,transform.localScale.y,transform.localScale.z);

		SetMonsterState(MonsterState.Idle);
		StartCoroutine(Appearing());
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics
	void Patrol()
	{
		SetAnimation(MonsterAnimationState.Walk);
		MoveToTarget(targetObj.transform);
		if(IsArrived(targetObj.transform) && targetObj.tag == Tags.RANDOM_TARGET){
			SetMonsterState(MonsterState.Investigate);
		}
	}

	void Investigate()
	{
		if(!flagInvestigate){
			flagInvestigate = true;
			targetObj = null;
			targetHidingPlace = GetNearestHidingPlace();
		}else{
			if(targetHidingPlace != null)
			{
				MoveToTarget(targetHidingPlace.transform);
				if(IsArrived(targetHidingPlace.transform)){
					SetMonsterState(MonsterState.CheckHidingPlace);
				}
			}else{
				StartCoroutine(Idling());
			}
		}
	}

	GameObject GetNearestHidingPlace()
	{
		if(hidingPlace.Count == 0) return null;
		else if(hidingPlace.Count == 1){
			if(IsInFront(hidingPlace[0].transform)) return hidingPlace[0];
			else return null;
		}else{
			List<int> indexes = new List<int>();

			for(int i = 0;i<hidingPlace.Count;i++){
				if(IsInFront(hidingPlace[i].transform)){
					indexes.Add(i);
				}
			}

			if(indexes.Count == 0){
				return null;
			}else if(indexes.Count == 1){
				return hidingPlace[indexes[0]];
			}else{
				int targetIndex = UnityEngine.Random.Range(0,indexes.Count);
				return hidingPlace[indexes[targetIndex]];
			}
		}
	}

	bool IsInFront(Transform hidingPlaceTransform)
	{
		if((transform.localScale == vRight && hidingPlaceTransform.position.x > transform.position.x) || 
			(transform.localScale == vLeft && hidingPlaceTransform.position.x < transform.position.x))
			return true;
		else return false;
	}

	void CheckHidingPlace()
	{
		if(!flagCheckHidingPlace){
			flagCheckHidingPlace = true;
			SetAnimation(MonsterAnimationState.CheckHidingPlace);
			StartCoroutine(CheckingHidingPlace());
		}
	}

	void Chase()
	{
		SetAnimation(MonsterAnimationState.Run);
		MoveToTarget(targetObj.transform);
		if(IsNearbyTarget(targetObj.transform)){
			SetMonsterState(MonsterState.Eat);
			Eat();
		}
	}
	void Eat()
	{
		if(!flagEat){
			flagEat = true;
			if(targetObj.tag == Tags.MAINCHAR){
				//kill Niwel
				//game over 
			}else if(targetObj.tag == Tags.SOLDIER){
				//kill Soldier
				targetObj.GetComponent<Soldier>().Die(transform);
			}
			timer = eatDuration;
		}else if(flagEat){
			timer += Time.deltaTime;
			if(timer <= 0){
				timer = 0;
				flagEat = false;
				SetMonsterState(MonsterState.Investigate);
			}
		}
	}

	void MoveToTarget(Transform target)
	{
		SetDirection(target);
		transform.position = Vector3.MoveTowards(transform.position,target.position,Time.deltaTime*speed);
	}

	void SetDirection(Transform target)
	{
		if(transform.position.x < target.position.x){
			transform.localScale = vRight;
		}else{
			transform.localScale = vLeft;
		}
	}

	bool IsArrived(Transform target)
	{
		Vector3 scaleSelf = transform.localScale;
		float xTarget = target.position.x;
		if( (scaleSelf ==  vLeft && transform.position.x <= xTarget - 0.05f) || 
			(scaleSelf == vRight && transform.position.x >= xTarget + 0.05f)){
			return true;
		}else{
			return false;
		}
	}

	bool IsNearbyTarget(Transform target)
	{
		Vector3 scaleSelf = transform.localScale;
		float xTarget = target.position.x;
		if( (scaleSelf ==  vLeft && transform.position.x <= xTarget - targetNearbyDistanceBeforeAttacking) || 
			(scaleSelf == vRight && transform.position.x >= xTarget + targetNearbyDistanceBeforeAttacking)){
			return true;
		}else{
			return false;
		}
	}

	void MoveToNextSpot()
	{
		targetObj = Instantiate(randomDestinationTargetObj) as GameObject;
		float randomRange = 
			transform.localScale.x == -1f ? 
			UnityEngine.Random.Range(6f,10f) : 
			UnityEngine.Random.Range(-6f,-10f);

		targetObj.transform.position = new Vector3(transform.position.x+randomRange,transform.position.y,transform.position.z);
		SetMonsterState(MonsterState.Patrol);
	}

	void SetMonsterState(MonsterState state)
	{
		monsterState = state;
	}

	void SetAnimation(MonsterAnimationState state)
	{
		thisAnim.SetInteger("State",(int)state);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules
	public void DetectObjects(GameObject otherObj)
	{
		if(targetObj == null){
			targetObj = otherObj;
			SetMonsterState(MonsterState.Chase);
		}else if(targetObj != null && targetObj.tag != Tags.SOLDIER){
			if(targetObj.tag == Tags.RANDOM_TARGET) Destroy(targetObj);
			targetObj = otherObj;
			SetMonsterState(MonsterState.Chase);
		}
	}

	public void DetectHidingPlace(GameObject hidingPlaceObj)
	{
		if(monsterState != MonsterState.Investigate){
			hidingPlace.Add(hidingPlaceObj);
		}
	}

	public void ChangeDirection()
	{
		if(transform.localScale == vLeft) transform.localScale = vRight;
		else transform.localScale = vLeft;
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	void Update()
	{
		if(monsterState == MonsterState.Idle){
			
		}else if(monsterState == MonsterState.Patrol){
			Patrol();
		}else if(monsterState == MonsterState.Investigate){
			Investigate();
		}else if(monsterState == MonsterState.CheckHidingPlace){
			CheckHidingPlace();
		}else if(monsterState == MonsterState.Chase){
			Chase();
		}else if(monsterState == MonsterState.Eat){
			Eat();
		}
	}

	IEnumerator Appearing()
	{
		yield return null;
		SetMonsterState(MonsterState.Investigate);

		yield return new WaitForSeconds(monsterAppearDuration);
		Destroy(gameObject);
	}

	IEnumerator CheckingHidingPlace()
	{
		yield return new WaitForSeconds(checkHidingPlaceDuration);
		SetMonsterState(MonsterState.Idle);
		flagCheckHidingPlace = false;
		StartCoroutine(Idling());
	}

	IEnumerator Idling()
	{
		hidingPlace.Clear();
		SetAnimation(MonsterAnimationState.Idle);
		yield return new WaitForSeconds(idleDuration);
		MoveToNextSpot();
	}

	IEnumerator Eating()
	{
		yield return new WaitForSeconds(eatDuration);
		SetMonsterState(MonsterState.Investigate);
	}
}