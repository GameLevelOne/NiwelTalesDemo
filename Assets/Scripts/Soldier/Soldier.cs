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
	CheckHidingPlace,
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
	public float runSpeed;
	public float idleDuration = 2f;
	public float startleDuration = 3f;
	public float chaseDuration = 10f;
	public float checkHidingPlaceDuration = 0.5f;
	public float panicDuration = 10f;
	public Vector2 niwelGrabOffset = new Vector2(-3f,-4.2f);

	[Header("Bullet Settings")]
	public float bulletXStartLeft = -0.34f;
	public float bulletXStartRight = 0.34f;
	public float bulletY = 0.11f;
	[Header("Random between positif to negative value")][Range(10f,45f)]
	public float bulletRotationZ = 15f;

	[Header("Do Not Modify")]
	public SoldierState soldierState = SoldierState.Patrol;
	public List<GameObject> hidingPlace = new List<GameObject>();
	[SerializeField] GameObject targetHidingPlace;
	[SerializeField] GameObject targetMainChar;
	[SerializeField] GameObject targetMonster;
	[SerializeField] float timer = 0;

	Vector3 vLeft, vRight;
	int currentWaypoint = 0;
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

		InitPatrol();
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics

	#region main behaviour
	void InitIdle()
	{
		SetSoldierState(SoldierState.Idle);
		timer = idleDuration;
	}
	void Idle()
	{
		timer -= Time.deltaTime;
		if(timer <= 0) InitPatrol();
	}

	void InitPatrol()
	{
		ChangeWaypoint();
		SetSoldierState(SoldierState.Patrol);
	}
	void Patrol()
	{
		MoveToTarget(waypoints[currentWaypoint]);

		if(IsArrived(waypoints[currentWaypoint])) InitIdle();
	}

	void InitStartled()
	{
		SetSoldierState(SoldierState.Startled);
		timer = startleDuration;
		SetDirection (targetMainChar.transform);
	}
	void Startled()
	{
		timer -= Time.deltaTime;
		if(timer <= 0f) InitChase();
	}

	void InitChase()
	{
		SetSoldierState(SoldierState.Chase);
		timer = chaseDuration;

		targetMainChar.GetComponent<Niwel> ().ChasedBySoldier ();
	}
	void Chase()
	{
		timer -= Time.deltaTime;
		if(timer <= 0) {
			targetMainChar = null;
			InitIdle();
		}

		MoveToTarget(targetMainChar.transform);
	}

	public void InitGrabNiwel()
	{
		SetSoldierState(SoldierState.GrabNiwel);
		targetMainChar.transform.position = 
			new Vector3(
				transform.position.x+niwelGrabOffset.x,
				transform.position.y+niwelGrabOffset.y,
				targetMainChar.transform.position.z
			);
		targetMainChar.GetComponent<Niwel> ().GrabbedBySoldier (this);
	}

	void InitInvestigate()
	{
		SetSoldierState(SoldierState.Investigate);
		targetMainChar.GetComponent<Niwel> ().NotChasedBySoldier ();

		targetHidingPlace = GetNearestHidingPlace();
		if(targetMainChar.GetComponent<Niwel>().GetNiwelHideStatus()) InitGrabNiwel();
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
		SetSoldierState(SoldierState.CheckHidingPlace);
		timer = checkHidingPlaceDuration;

		if(targetMainChar != null) targetMainChar.GetComponent<Niwel> ().GrabbedFromHidingPlace ();
	}
	void CheckHidingPlace()
	{
		timer -= Time.deltaTime;
		if(timer <= 0){
			targetHidingPlace = null;
			InitIdle();
		}
	}

	void InitPanic()
	{
		targetMainChar = null;
		SetSoldierState(SoldierState.Panic);
	}

	public void InitDie()
	{
		SetSoldierState(SoldierState.Die);
		thisRigidbody.simulated = false;
		thisCollider.enabled = false;
	}
	#endregion

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
	#endregion

	#region core mechanic
	public void SetSoldierState(SoldierState state)
	{
		soldierState = state;
	}

	void SetAnimation(SoldierAnimationState state)
	{
		thisAnim.SetInteger("State",(int)state);
	}

	void MoveToTarget(Transform target)
	{
		SetDirection(target);
		Vector3 direction = transform.localScale == vLeft ? Vector3.left : Vector3.right;
		float spd = soldierState == SoldierState.Chase ? runSpeed : speed;
		transform.position += (direction * spd);
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
		if( (scaleSelf == vLeft && transform.position.x <= xTarget + 1.2f) || 
			(scaleSelf == vRight && transform.position.x >= xTarget - 1.2f)){
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
	#endregion

	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules
	public void ReleaseNiwel()
	{
		InitStartled();
	}

	public void SetObject(GameObject obj)
	{
			  if(obj.tag == Tags.MAINCHAR){
			targetMainChar = obj;
		}else if(obj.tag == Tags.HIDEABLE){
			hidingPlace.Add(obj);
		}else if(obj.tag == Tags.MONSTER ){
			targetMonster = obj;
		}
	}

	public void RemoveObject(GameObject obj)
	{
			  if(obj.tag == Tags.MAINCHAR){
			targetMainChar = null;
		}else if(obj.tag == Tags.HIDEABLE){
			hidingPlace.Remove(obj);
		}else if(obj.tag == Tags.MONSTER ){
			targetMonster = null;
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	void Update()
	{
		StateCheck();
		StateToAnim();
	}

	void StateCheck()
	{
		if(soldierState == SoldierState.Idle){ //--------------IDLE
			if(targetMainChar != null){
				InitStartled();
			}
			if(targetMonster != null){
				InitPanic();
			}

			Idle();
		}else if(soldierState == SoldierState.Patrol){//--------------PATROL
			if(targetMainChar != null){
				InitStartled();
			}
			if(targetMonster != null){
				InitPanic();
			}

			Patrol();
		}else if(soldierState == SoldierState.Startled){//--------------STARTLED
			if(targetMonster != null){
				InitPanic();
			}

			Startled();
		}else if(soldierState == SoldierState.Chase){//--------------CHASE
			if(targetMonster != null){
				InitPanic();
			}

			if(targetMainChar == null){

				InitIdle();
			}else{
				Chase();
			}
		}else if(soldierState == SoldierState.GrabNiwel){//--------------GRABNIWEL
			if(targetMonster != null){
				ReleaseNiwel();
				InitPanic();
			}

		}else if(soldierState == SoldierState.Investigate){//--------------INVESTIGATE
			if(targetMonster != null){
				InitPanic();
			}

			Investigate();
		}else if(soldierState == SoldierState.CheckHidingPlace){//--------------CHECKHIDINGPLACE
			if(targetMonster != null){
				InitPanic();
			}

			CheckHidingPlace();
		}else if(soldierState == SoldierState.Panic){//--------------PANIC
			
		}else if(soldierState == SoldierState.Die){ //--------------DIE
			
		}
	}

	void StateToAnim()
	{
		if(soldierState == SoldierState.Idle){
			SetAnimation(SoldierAnimationState.Idle);
		}else if(soldierState == SoldierState.Patrol){
			SetAnimation(SoldierAnimationState.Walk);
		}else if(soldierState == SoldierState.Chase){
			SetAnimation(SoldierAnimationState.Run);
		}else if(soldierState == SoldierState.Startled){
			SetAnimation(SoldierAnimationState.Startled);
		}else if(soldierState == SoldierState.GrabNiwel){
			SetAnimation(SoldierAnimationState.GrabNiwel);
		}else if(soldierState == SoldierState.Panic){
			SetAnimation(SoldierAnimationState.Panic);
		}else if(soldierState == SoldierState.Investigate){
			SetAnimation(SoldierAnimationState.Walk);
		}else if(soldierState == SoldierState.CheckHidingPlace){
			SetAnimation(SoldierAnimationState.CheckHidingPlace);
		}else if(soldierState == SoldierState.Die){
			SetAnimation(SoldierAnimationState.Die);
		}
	}
}