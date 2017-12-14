﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SoldierState{
	Idle,
	Patrol,
	Startled,
	Chase,
	Panic,
	Investigate
}

public enum SoldierAnimationState{
	Idle,
	Walk,
	Startled,
	CheckHidingPlace,
	Panic,
}

public class Soldier : MonoBehaviour {
	#region attributes
	[Header("Soldier Attributes")]
	public Animator thisAnim;
	public Rigidbody2D thisRigidbody;
	public Collider2D thisCollider;
	public Transform[] waypoints;
	public GameObject bulletObject;

	[Header("Custom Attributes")]
	public float speed = 0.025f;
	public float idleDuration = 2f;
	public float startleDuration = 3f;
	public float chaseDuration = 10f;
	public float panicDuration = 10f;

	[Header("Bullet Settings")]
	public float bulletXStartLeft = -0.34f;
	public float bulletXStartRight = 0.34f;
	public float bulletY = 0.11f;
	[Header("Random between positif to negative value")][Range(10f,45f)]
	public float bulletRotationZ = 15f;

	[Header("Do Not Modify")]
	public SoldierState soldierState = SoldierState.Idle;
	public List<GameObject> hidingPlace = new List<GameObject>();
	[SerializeField] GameObject targetHidingPlace;
	[SerializeField] GameObject niwelTarget;
	public int currentWaypoint = 0;

	public bool flagIdle = false;
	public bool flagStartled = false;
	public bool flagChase = false;
	public bool flagPanic = false;
	public bool flagInvestigate = false;

	Vector3 vLeft = new Vector3(-1f,1f,1f);
	Vector3 vRight = Vector3.one;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	void Start()
	{
		Init();
	}

	public void Init()
	{
//		bodyCollider.OnTriggerEnter += Patrol;
//		forebodytach(SoldierTriggerCollider tc in visionCollider) tc.OnTriggerEnter += DetectObject;

		SetSoldierState(SoldierState.Patrol);
		thisAnim.SetInteger("State",(int)soldierState);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics
	void Patrol()
	{
		print("Soldier is Patrolling");
		SetAnimation(SoldierAnimationState.Walk);
		MoveToTarget(waypoints[currentWaypoint]);
		if(IsArrived(waypoints[currentWaypoint])){
			if(!flagIdle){	
				flagIdle = true;
				StartCoroutine(Idling());
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
		float xScaleSelf = transform.localScale.x;
		float xTarget= target.position.x;
		if((xScaleSelf == -1f && transform.position.x <= target.position.x+0.05f) || (xScaleSelf == 1f && transform.position.x >= target.position.x-0.05f)){
			return true;
		}else{
			return false;
		}
	}

	void ChangeWaypoint()
	{
		currentWaypoint++;
		if(currentWaypoint == waypoints.Length) currentWaypoint = 0;
	}

	void Startled()
	{
		SetAnimation(SoldierAnimationState.Idle);
		StartCoroutine(Startling());
	}

	void Chase()
	{
		MoveToTarget(niwelTarget.transform);
	}

	void Panic()
	{
		if(!flagPanic){
			flagPanic = true;
			print("Soldier is panicked");
			SetSoldierState(SoldierState.Panic);
			SetAnimation(SoldierAnimationState.Panic);
		}
	}

	void Investigate()
	{
		
		if(!flagInvestigate){
			print("Soldier is Investigating");
			niwelTarget = null;
			flagInvestigate = true;
			targetHidingPlace = getNearestHidingPlace();
		}else{
			if(targetHidingPlace != null)
			{
				MoveToTarget(targetHidingPlace.transform);
				if(IsArrived(targetHidingPlace.transform)){
					flagInvestigate = false;
					flagIdle = true;
					StartCoroutine(Idling());
					
				}
			}
		}

	}

	GameObject getNearestHidingPlace()
	{
		if(hidingPlace.Count == 0) return null;
		else if(hidingPlace.Count == 1){
			return hidingPlace[0];
		}else{
			Vector2 Soldier2DPos = new Vector2(transform.position.x,transform.position.y);
			Vector2 hidingPlace2DPos = new Vector2(hidingPlace[0].transform.position.x,hidingPlace[0].transform.position.y);
			int targetIndex = 0;
			float currentNearestDistance = Vector2.Distance(Soldier2DPos,hidingPlace2DPos);

			for(int i = 1; i<hidingPlace.Count;i++){
				hidingPlace2DPos = new Vector2(hidingPlace[i].transform.position.x,hidingPlace[i].transform.position.y);
				if(Vector2.Distance(Soldier2DPos,hidingPlace2DPos) < currentNearestDistance){
					targetIndex = i;
					currentNearestDistance = Vector2.Distance(Soldier2DPos,hidingPlace2DPos);
				}
			}

			return hidingPlace[targetIndex];
		}
	}

	#region AnimationEvent
	void Shoot()
	{
		//tembak dor dor
		print("Dor");
		float randomAngle = UnityEngine.Random.Range(-1*bulletRotationZ,bulletRotationZ);
		float x = transform.localScale.x == 1f ? bulletXStartRight : bulletXStartLeft;
		Vector3 bulletPos = new Vector3(transform.position.x+x,bulletY,0f);
		Vector3 bulletRot = new Vector3(0,0,randomAngle);

		GameObject tempBullet = Instantiate(bulletObject,bulletPos,Quaternion.Euler(bulletRot));
		tempBullet.transform.localScale = transform.localScale.x == 1f ? vRight : vLeft;
		tempBullet.GetComponent<Bullet>().Init();
	}
	#endregion

	void SetSoldierState(SoldierState state)
	{
		soldierState = state;
	}

	void SetAnimation(SoldierAnimationState state)
	{
		thisAnim.SetInteger("State",(int)state);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules
	public void DetectObject(GameObject otherObj, bool longVision)
	{
		if(otherObj.tag == Tags.MAINCHAR){
			niwelTarget = otherObj;
			if(!longVision && (soldierState != SoldierState.Startled && soldierState != SoldierState.Chase && soldierState != SoldierState.Panic)) SetSoldierState(SoldierState.Startled);
		}else if(otherObj.tag == Tags.MONSTER){
			if(!longVision)	SetSoldierState(SoldierState.Panic);
		}
	}

	public void DetectHidingPlace(GameObject hidingPlaceObj)
	{
		if(soldierState != SoldierState.Investigate){
			hidingPlace.Add(hidingPlaceObj);
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	void Update()
	{
		if(soldierState == SoldierState.Idle)
		{
			
		}else if(soldierState == SoldierState.Patrol){
			Patrol();
		}else if(soldierState == SoldierState.Chase){
			Chase();
		}else if(soldierState == SoldierState.Startled){
			if(!flagStartled){
				flagStartled = true;
				Startled();
			}
		}else if(soldierState == SoldierState.Panic){
			Panic();
		}else if(soldierState == SoldierState.Investigate){
			Investigate();
		}
	}
		
	IEnumerator Idling()
	{
		SetSoldierState(SoldierState.Idle);
		SetAnimation(SoldierAnimationState.Idle);

		yield return new WaitForSeconds(idleDuration);

		ChangeWaypoint();
		flagIdle = false;
		SetSoldierState(SoldierState.Patrol);
	}

	IEnumerator ChasingNiwel()
	{
		flagChase = true;
		SetAnimation(SoldierAnimationState.Walk);
		print("Soldier is chasing niwel");
		yield return new WaitForSeconds(chaseDuration);
		print("Soldier Stops Chasing Niwel");
		flagChase = false;
		SetSoldierState(SoldierState.Investigate);
	}

	IEnumerator Startling()
	{
		print("Soldier is startled");
		yield return new WaitForSeconds(startleDuration);
		SetSoldierState(SoldierState.Chase);
		flagStartled = false;
		StartCoroutine(ChasingNiwel());
	}
}