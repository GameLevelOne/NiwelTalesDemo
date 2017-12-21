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
	Attack,
	Confused
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
	[SerializeField] GameObject currentAttackTarget;
	[SerializeField] GameObject targetObj;
	[SerializeField] GameObject targetSoldier;
	[SerializeField] GameObject targetMainChar;
	[SerializeField] GameObject targetRandomSpot;
	[SerializeField] GameObject targetHidingPlace;
	[SerializeField] GameObject wallObj;
	public Vector3 vLeft, vRight;
	public bool flagMonsterTimer;
	public bool flagConfused = false;

	public delegate void MonsterDestroyed();
	public event MonsterDestroyed OnMonsterDestroyed;

	float timer;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	void Start()
	{
		Init(false);
	}

	/// <summary>
	/// Initialize monster behavior, if facingRight, monster will go to direction --->, else <---
	/// </summary>
	public void Init(bool facingRight)
	{
		InitInvestigate();
		vLeft = transform.localScale;
		vRight = new Vector3(transform.localScale.x * -1f,transform.localScale.y,transform.localScale.z);

		SetViewDirection(facingRight);

		flagMonsterTimer = true;
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
	void InitIdle()
	{
		SetMonsterState(MonsterState.Idle);
		timer = idleDuration;
	}
	void Idle()
	{
		if(!flagConfused){
			timer -= Time.deltaTime;
			if(timer <= 0f) InitPatrol();
		}
	}

	void InitPatrol()
	{
		SetMonsterState(MonsterState.Patrol);
		MoveToNextSpot();
	}
	void Patrol()
	{
		MoveToTarget(targetObj.transform);
		if(IsArrived(targetObj.transform) && targetObj.tag == Tags.RANDOM_TARGET){
			
			InitInvestigate();
		}
	}

	void InitInvestigate()
	{
		SetMonsterState(MonsterState.Investigate);
		Destroy(targetObj);
		targetObj = null;
		targetHidingPlace = GetNearestHidingPlace();
	}
	void Investigate()
	{
		if(targetHidingPlace != null)
		{
			MoveToTarget(targetHidingPlace.transform);
			if(IsArrived(targetHidingPlace.transform)){

				InitCheckHidingPlace();
			}
		}else{
			InitIdle();
		}
	}

	void InitCheckHidingPlace()
	{
		SetMonsterState(MonsterState.CheckHidingPlace);
		timer = checkHidingPlaceDuration;
	}
	void CheckHidingPlace()
	{
		timer -= Time.deltaTime;
		if(timer <= 0){
			InitIdle();
		}
	}

	public void InitChase(GameObject target)
	{
		if(currentAttackTarget == null){
			SetMonsterState(MonsterState.Chase);
			currentAttackTarget = target;
		}
	}
	void Chase()
	{
		MoveToTarget(currentAttackTarget.transform);
		if(IsNearbyTarget(currentAttackTarget.transform)){
			InitAttack();
		}
	}

	void InitAttack()
	{
		SetMonsterState(MonsterState.Attack);
		if(currentAttackTarget.tag == Tags.MAINCHAR){
			//kill Niwel
			//game over 
		}else if(currentAttackTarget.tag == Tags.SOLDIER){
			//kill Soldier
			targetSoldier = null;
			currentAttackTarget.GetComponent<Soldier>().InitDie();
		}
		timer = attackDuration;
	}
	void Attack()
	{
		timer -= Time.deltaTime;
		if(timer <= 0){
			
			currentAttackTarget = null;
			InitIdle();
		}
	}

	void InitConfused()
	{
		SetMonsterState(MonsterState.Confused);
	}
	#endregion
	GameObject GetNearestHidingPlace()
	{
		if(hidingPlace.Count == 0) return null;
		else if(hidingPlace.Count == 1){
			if(IsInFront(hidingPlace[0].transform)) return hidingPlace[0];
			else return null;
		}else{
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
//		print (transform.localScale.x);
		targetObj = Instantiate(randomDestinationTargetObj) as GameObject;
		float randomRange = 
			transform.localScale.x < 0 ? 
			UnityEngine.Random.Range(6f,10f) : 
			UnityEngine.Random.Range(-6f,-10f);

		targetObj.transform.position = new Vector3(transform.position.x+randomRange,transform.position.y,transform.position.z);
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
		if(otherObj.tag != Tags.MONSTER){
			if(targetObj == null){
				targetObj = otherObj;
				SetMonsterState(MonsterState.Chase);
			}else if(targetObj != null && targetObj.tag != Tags.SOLDIER){
				if(targetObj.tag == Tags.RANDOM_TARGET) Destroy(targetObj);
				targetObj = otherObj;
				SetMonsterState(MonsterState.Chase);
			}
		}
	}

	public void DetectHidingPlace(GameObject hidingPlaceObj)
	{
		hidingPlace.Add(hidingPlaceObj);
	}

	public void FaceWall()
	{
		InitConfused();
	}

	public void SetObject(GameObject obj)
	{
		if(obj.tag == Tags.MAINCHAR){
			targetMainChar = obj;
		}else if(obj.tag == Tags.SOLDIER){
			targetSoldier = obj;
		}else if(obj.tag == Tags.HIDEABLE){
			hidingPlace.Add(obj);
		}else if(obj.tag == Tags.WALL){
			wallObj = obj;
		}
	}

	public void RemoveObject(GameObject obj)
	{
		if(obj.tag == Tags.MAINCHAR){
			targetMainChar = null;
		}else if(obj.tag == Tags.SOLDIER){
			targetSoldier = null;
		}else if(obj.tag == Tags.HIDEABLE){
			hidingPlace.Remove(obj);
		}else if(obj.tag == Tags.WALL){
			wallObj = null;
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	#region UPDATE
	void Update()
	{
		if(flagMonsterTimer){
			monsterAppearDuration -= Time.deltaTime;
			if(monsterAppearDuration <= 0f){
				if (OnMonsterDestroyed != null)
					OnMonsterDestroyed ();
				Destroy(gameObject);
			}
		}

		StateCheck();
		StateToAnim();
	}

	void StateCheck()
	{
		if(monsterState == MonsterState.Idle){
			flagMonsterTimer = true;
			if(targetSoldier != null){
				InitChase(targetSoldier);
			}else if(targetMainChar != null){
				InitChase(targetMainChar);
			}

			Idle();
		}else if(monsterState == MonsterState.Patrol){
			flagMonsterTimer = true;
			if(targetSoldier != null){
				InitChase(targetSoldier);
			}else if(targetMainChar != null){
				InitChase(targetMainChar);
			}else if(wallObj != null){
				InitConfused();
			}

			Patrol();
		}else if(monsterState == MonsterState.Investigate){
			flagMonsterTimer = true;
			if(targetSoldier != null){
				InitChase(targetSoldier);
			}else if(targetMainChar != null){
				InitChase(targetMainChar);
			}else if(wallObj != null){
				InitConfused();
			}

			Investigate();
		}else if(monsterState == MonsterState.CheckHidingPlace){
			flagMonsterTimer = true;
			if(targetSoldier != null){
				InitChase(targetSoldier);
			}else if(targetMainChar != null){
				InitChase(targetMainChar);
			}

			CheckHidingPlace();
		}else if(monsterState == MonsterState.Chase){
			flagMonsterTimer = false;
			if(currentAttackTarget == targetMainChar && targetSoldier != null){
				InitChase(targetSoldier);
			}

			Chase();
		}else if(monsterState == MonsterState.Attack){
			flagMonsterTimer = false;

			Attack();
		}else if(monsterState == MonsterState.Confused){
			flagMonsterTimer = true;
			if(targetSoldier != null){
				InitChase(targetSoldier);
			}else if(targetMainChar != null){
				InitChase(targetMainChar);
			}
		}
	}

	void StateToAnim()
	{
		if(monsterState == MonsterState.Idle){
			SetAnimation(MonsterAnimationState.Idle);
		}else if(monsterState == MonsterState.Patrol){
			SetAnimation(MonsterAnimationState.Walk);
		}else if(monsterState == MonsterState.Investigate){
			SetAnimation(MonsterAnimationState.Walk);
		}else if(monsterState == MonsterState.CheckHidingPlace){
			SetAnimation(MonsterAnimationState.CheckHidingPlace);
		}else if(monsterState == MonsterState.Chase){
			SetAnimation(MonsterAnimationState.Run);
		}else if(monsterState == MonsterState.Attack){
			SetAnimation(MonsterAnimationState.Attack);
		}else if(monsterState == MonsterState.Confused){
			SetAnimation(MonsterAnimationState.Idle);
		}
	}
	#endregion
	
}