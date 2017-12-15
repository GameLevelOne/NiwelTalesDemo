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
	Attack
}

public enum MonsterAnimationState
{
	Idle,
	Walk,
	Run,
	Attack,
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
	public float attackDuration = 5f;
	public float checkHidingPlaceDuration = 2f;
	public float targetNearbyDistanceBeforeAttacking = 5f;


	[Header("Do Not Modify")]
	public List<GameObject> hidingPlace = new List<GameObject>();
	[SerializeField] GameObject targetObj;
	[SerializeField] GameObject targetHidingPlace;
	public Vector3 vLeft, vRight;
	public bool flagInit = false;
	public bool flagMonsterTimer;
	public bool flagIdle = false;
	public bool flagInvestigate = false;
	public bool flagCheckHidingPlace = false;
	public bool flagChase = false;
	public bool flagAttack = false;


	Vector3 currentViewDirection;

	float timer;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	/// <summary>
	/// Initialize monster behavior, if facingRight, monster will go to direction --->, else <---
	/// </summary>
	public void Init(bool facingRight)
	{
		SetMonsterState(MonsterState.Investigate);
		vLeft = transform.localScale;
		vRight = new Vector3(transform.localScale.x * -1f,transform.localScale.y,transform.localScale.z);

		SetViewDirection(facingRight);

		flagMonsterTimer = true;
		StartCoroutine(Appearing());
	}

	void SetViewDirection(bool facingRight)
	{
		if(facingRight){
			transform.localScale = vRight;
		}else{
			transform.localScale = vLeft;
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics

	#region main behavior
	void Idle()
	{
		if(!flagIdle){
			hidingPlace.Clear();
			flagIdle = true;
			timer = idleDuration;
			SetAnimation(MonsterAnimationState.Idle);
		}else if(flagIdle){
			timer -= Time.deltaTime;
			if(timer <= 0f){
				flagIdle = false;
				MoveToNextSpot();
			}
		}
	}

	void Patrol()
	{
		SetAnimation(MonsterAnimationState.Walk);
		MoveToTarget(targetObj.transform);
		if(IsArrived(targetObj.transform) && targetObj.tag == Tags.RANDOM_TARGET){
			Destroy(targetObj);
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
				SetAnimation(MonsterAnimationState.Walk);
				MoveToTarget(targetHidingPlace.transform);
				if(IsArrived(targetHidingPlace.transform)){
					flagInvestigate = false;
					SetMonsterState(MonsterState.CheckHidingPlace);
				}
			}else{
				flagInvestigate = false;
				SetMonsterState(MonsterState.Idle);
			}
		}
	}

	void CheckHidingPlace()
	{
		if(!flagCheckHidingPlace){
			flagCheckHidingPlace = true;
			SetAnimation(MonsterAnimationState.CheckHidingPlace);
			timer = checkHidingPlaceDuration;
		}else if(flagCheckHidingPlace){
			timer -= Time.deltaTime;
			if(timer <= 0){
				flagCheckHidingPlace = false;
				SetMonsterState(MonsterState.Idle);
			}
		}
	}

	void Chase()
	{
		SetAnimation(MonsterAnimationState.Run);
		MoveToTarget(targetObj.transform);
		if(IsNearbyTarget(targetObj.transform)){
			SetMonsterState(MonsterState.Attack);
			Attack();
		}
	}

	void Attack()
	{
		if(!flagAttack){
			flagAttack = true;
			SetAnimation(MonsterAnimationState.Attack);
			if(targetObj.tag == Tags.MAINCHAR){
				//kill Niwel
				//game over 
			}else if(targetObj.tag == Tags.SOLDIER){
				//kill Soldier
				targetObj.GetComponent<Soldier>().SetSoldierState(SoldierState.Die);
			}
			timer = attackDuration;
		}else if(flagAttack){
			timer -= Time.deltaTime;
			if(timer <= 0){
				timer = 0;
				flagAttack = false;
				SetMonsterState(MonsterState.Idle);
			}
		}
	}
	#endregion
	GameObject GetNearestHidingPlace()
	{
		if(hidingPlace.Count == 0) return null;
		else if(hidingPlace.Count == 1){
			if(IsInFront(hidingPlace[0].transform)) return hidingPlace[0];
			else return null;
		}else{
			//			List<int> indexes = new List<int>();

			int targetIndex = -1;
			Vector2 monster2DPos = new Vector2(transform.position.x,transform.position.y);
			float currentNearestDistance = 0f;

			for(int i = 0;i<hidingPlace.Count;i++){
				if(IsInFront(hidingPlace[i].transform)){
					Vector2 hidingPlace2DPos = new Vector2(hidingPlace[i].transform.position.x,hidingPlace[i].transform.position.y);
					if(targetIndex == -1){
						currentNearestDistance = Vector2.Distance(monster2DPos,hidingPlace2DPos);
						targetIndex = i;
					}else{
						if(Vector2.Distance(monster2DPos,hidingPlace2DPos) < currentNearestDistance){
							currentNearestDistance = Vector2.Distance(monster2DPos,hidingPlace2DPos);
							targetIndex = i;
						}
					}
				}
			}

			if(targetIndex == -1) return null;
			else return hidingPlace[targetIndex];
		}
	}

	bool IsInFront(Transform hidingPlaceTransform)
	{
		if((transform.localScale == vRight && hidingPlaceTransform.position.x > transform.position.x) || 
			(transform.localScale == vLeft && hidingPlaceTransform.position.x < transform.position.x))
			return true;
		else return false;
	}

	void MoveToTarget(Transform target)
	{
		SetDirection(target);
		Vector3 direction = transform.localScale == vLeft ? Vector3.left : Vector3.right;
		transform.position += (direction * speed);
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

		if( (scaleSelf ==  vLeft && transform.position.x <= xTarget + 0.1f) || 
			(scaleSelf == vRight && transform.position.x >= xTarget - 0.1f)){

			return true;
		}else{
			
			return false;
		}
	}

	bool IsNearbyTarget(Transform target)
	{
		Vector3 scaleSelf = transform.localScale;
		float xTarget = target.position.x;
		if( (scaleSelf ==  vLeft && transform.position.x <= xTarget + targetNearbyDistanceBeforeAttacking) || 
			(scaleSelf == vRight && transform.position.x >= xTarget - targetNearbyDistanceBeforeAttacking)){
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
		hidingPlace.Add(hidingPlaceObj);
	}

	public void ChangeDirection()
	{
		if(transform.localScale == vLeft) transform.localScale = vRight;
		else transform.localScale = vLeft;
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	#region UPDATE
	void Update()
	{
		if(monsterState == MonsterState.Idle){
			if(flagInit) Idle();
			if(!flagMonsterTimer) flagMonsterTimer = true;
		}else if(monsterState == MonsterState.Patrol){
			Patrol();
			if(!flagMonsterTimer) flagMonsterTimer = true;
		}else if(monsterState == MonsterState.Investigate){
			if(!flagInit) flagInit = true;
			Investigate();
			if(!flagMonsterTimer) flagMonsterTimer = true;
		}else if(monsterState == MonsterState.CheckHidingPlace){
			CheckHidingPlace();
			if(!flagMonsterTimer) flagMonsterTimer = true;
		}else if(monsterState == MonsterState.Chase){
			Chase();
			if(flagMonsterTimer) flagMonsterTimer = false;
		}else if(monsterState == MonsterState.Attack){
			Attack();
			if(flagMonsterTimer) flagMonsterTimer = false;
		}

		if(flagMonsterTimer){
			monsterAppearDuration -= Time.deltaTime;
			if(monsterAppearDuration <= 0f){
				Destroy(gameObject);
			}
		}
	}
	#endregion

	IEnumerator Appearing()
	{
		yield return null;
		SetMonsterState(MonsterState.Investigate);

	}
}