using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SoldierState{
	Idle,
	Patrol,
	Startled,
	Chase,
	GrabNiwel,
	Panic,
	Investigate,
	Die
}

public enum SoldierAnimationState{
	Idle,
	Walk,
	Startled,
	Run,
	CheckHidingPlace,
	Panic,
	GrabNiwel,
	Die
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
	public Vector2 niwelGrabOffset = new Vector2(-3f,-4.2f);

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
	[SerializeField] float timer = 0;
	public int currentWaypoint = 0;

	public bool flagIdle = false;
	public bool flagStartled = false;
	public bool flagChase = false;
	public bool flagGrabNiwel = false;
	public bool flagPanic = false;
	public bool flagInvestigate = false;
	public bool flagDie = false;

	Vector3 vLeft, vRight;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	void Start()
	{
		Init();
	}

	public void Init()
	{
		vLeft = transform.localScale;
		vRight = new Vector3(transform.localScale.x * -1f,transform.localScale.y,transform.localScale.z);
		SetSoldierState(SoldierState.Patrol);
		thisAnim.SetInteger("State",(int)soldierState);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics
	void Idle()
	{
		if(!flagIdle){
			flagIdle = true;
			SetAnimation(SoldierAnimationState.Idle);
			timer = idleDuration;
		}else{
			timer -= Time.deltaTime;
			if(timer <= 0){
				flagIdle = false;
				ChangeWaypoint();
				SetSoldierState(SoldierState.Patrol);
			}
		}
	}

	void Patrol()
	{
		SetAnimation(SoldierAnimationState.Walk);
		MoveToTarget(waypoints[currentWaypoint]);
		if(IsArrived(waypoints[currentWaypoint])){
			SetSoldierState(SoldierState.Idle);
		}
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
		float xTarget= target.position.x;
		if( (scaleSelf == vLeft && transform.position.x <=xTarget + 0.05f) || 
			(scaleSelf == vRight && transform.position.x >= xTarget - 0.05f)){
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
		if(!flagStartled){
			flagStartled = true;
			SetAnimation(SoldierAnimationState.Startled);
			timer = startleDuration;
		}else{
			timer -= Time.deltaTime;
			if(timer <= 0f){
				flagStartled = false;
				SetSoldierState(SoldierState.Chase);
			}
		}
	}

	void Chase()
	{
		if(!flagChase){
			flagChase = true;
			timer = chaseDuration;
			SetAnimation(SoldierAnimationState.Run);
		}else{
			timer -= Time.deltaTime;
			if(timer <= 0){
				timer = 0;
				flagChase = false;
				SetSoldierState(SoldierState.Investigate);
			}

			MoveToTarget(niwelTarget.transform);
			if(IsArrived(niwelTarget.transform)){
				flagChase = false;
				SetSoldierState(SoldierState.GrabNiwel);
			}
		}

	}

	void GrabNiwel()
	{
		if(!flagGrabNiwel){
			flagGrabNiwel = true;
			SetAnimation(SoldierAnimationState.GrabNiwel);
			niwelTarget.transform.position = 
				new Vector3(
					transform.position.x+niwelGrabOffset.x,
					transform.position.y+niwelGrabOffset.y,
					niwelTarget.transform.position.z
				);
		}
	}

	void Panic()
	{
		if(!flagPanic){
			flagPanic = true;
			SetAnimation(SoldierAnimationState.Panic);
		}
	}

	void Die()
	{
		print("DIE");
		if(!flagDie){
			flagPanic = false;
			flagDie = true;
			SetAnimation(SoldierAnimationState.Die);
			thisRigidbody.simulated = false;
			thisCollider.enabled = false;
		}
	}

	void Investigate()
	{
		if(!flagInvestigate){
			print("Soldier is Investigating");
			niwelTarget = null;
			flagInvestigate = true;
			targetHidingPlace = GetNearestHidingPlace();
		}else{
			if(targetHidingPlace != null)
			{
				MoveToTarget(targetHidingPlace.transform);
				if(IsArrived(targetHidingPlace.transform)){
					flagInvestigate = false;
					SetSoldierState(SoldierState.Idle);
				}
			}else{
				flagInvestigate = false;
				SetSoldierState(SoldierState.Idle);
			}
		}

	}

	GameObject GetNearestHidingPlace()
	{
		if(hidingPlace.Count == 0) return null;
		else if(hidingPlace.Count == 1){
			return hidingPlace[0];
		}else{
			Vector2 soldier2DPos = new Vector2(transform.position.x,transform.position.y);
			Vector2 hidingPlace2DPos = new Vector2(hidingPlace[0].transform.position.x,hidingPlace[0].transform.position.y);
			int targetIndex = 0;
			float currentNearestDistance = Vector2.Distance(soldier2DPos,hidingPlace2DPos);

			for(int i = 1; i<hidingPlace.Count;i++){
				hidingPlace2DPos = new Vector2(hidingPlace[i].transform.position.x,hidingPlace[i].transform.position.y);
				if(Vector2.Distance(soldier2DPos,hidingPlace2DPos) < currentNearestDistance){
					targetIndex = i;
					currentNearestDistance = Vector2.Distance(soldier2DPos,hidingPlace2DPos);
				}
			}

			return hidingPlace[targetIndex];
		}
	}

	#region AnimationEvent
	public void Shoot()
	{
		//tembak dor dor
//		print("Dor");
		float randomAngle = UnityEngine.Random.Range(-1*bulletRotationZ,bulletRotationZ);
		float x = transform.localScale.x == 1f ? bulletXStartRight : bulletXStartLeft;
		Vector3 bulletPosision = new Vector3(transform.position.x+x,bulletY,0f);
		Vector3 bulletRotation = new Vector3(0,0,randomAngle);

		GameObject tempBullet = Instantiate(bulletObject,bulletPosision,Quaternion.Euler(bulletRotation));
		tempBullet.transform.localScale = 
			transform.localScale.x == 1f ? 
			tempBullet.transform.localScale : 
			new Vector3(tempBullet.transform.localScale.x * -1f,tempBullet.transform.localScale.y,tempBullet.transform.localScale.z);
		tempBullet.GetComponent<Bullet>().Init(gameObject);
	}

	void OnSoldierDoneDiying()
	{
		print("Soldier is dead");
		thisRigidbody.simulated = false;
		thisCollider.enabled = false;
	}
	#endregion

	public void SetSoldierState(SoldierState state)
	{
		soldierState = state;
	}

	void SetAnimation(SoldierAnimationState state)
	{
		flagGrabNiwel = false;
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
			if(!longVision && (soldierState != SoldierState.Panic))	SetSoldierState(SoldierState.Panic);
		}
	}

	public void DetectHidingPlace(GameObject hidingPlaceObj)
	{
		if(soldierState != SoldierState.Investigate){
			hidingPlace.Add(hidingPlaceObj);
		}
	}
//
//	public void Die(Transform monsterTarget)
//	{
//		SetDirection(monsterTarget);
//		SetSoldierState(SoldierState.Die);
//		SetAnimation(SoldierAnimationState.Die);
//	}

	public void ReleaseNiwel()
	{
		SetSoldierState(SoldierState.Startled);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	void Update()
	{
		if(soldierState == SoldierState.Idle){
			Idle();
		}else if(soldierState == SoldierState.Patrol){
			Patrol();
		}else if(soldierState == SoldierState.Chase){
			Chase();
		}else if(soldierState == SoldierState.Startled){
			Startled();
		}else if(soldierState == SoldierState.Panic){
			Panic();
		}else if(soldierState == SoldierState.Investigate){
			Investigate();
		}else if(soldierState == SoldierState.Die){
			Die();
		}
	}
}