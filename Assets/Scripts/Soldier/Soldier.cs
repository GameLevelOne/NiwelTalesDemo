using System.Collections;
using UnityEngine;

public enum SoldierState{
	Idle,
	Patrol,
	Startled,
	Chase,
	Panic,
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
	public GameObject niwelTarget;
	public int currentWaypoint = 0;

	public bool flagIdle = false;
	public bool flagStartled = false;
	public bool flagChase = false;
	public bool flagPanic = false;

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
		SetAnimation(SoldierAnimationState.Walk);
		MoveToWaypoint(waypoints[currentWaypoint]);
		if(IsArrived(waypoints[currentWaypoint])){
			if(!flagIdle){	
				flagIdle = true;
				StartCoroutine(Idling());
			}
		}
	}

	void MoveToWaypoint(Transform target)
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
		if((xScaleSelf == -1f && transform.position.x <= target.position.x) || (xScaleSelf == 1f && transform.position.x >= target.position.x)){
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
		print("Soldier is chasing Niwel");

	}

	void Panic()
	{
		if(soldierState != SoldierState.Panic){
			print("Soldier is panicked");
			SetSoldierState(SoldierState.Panic);
			SetAnimation(SoldierAnimationState.Panic);
		}
	}

	#region AnimationEvent
	void Shoot()
	{
		//tembak dor dor
		print("Dor");
		float randomAngle = Random.Range(-1*bulletRotationZ,bulletRotationZ);
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

	Vector3 getDirection()
	{
		float direction = transform.position.x > waypoints[currentWaypoint].position.x ? 1f : -1f;
		return new Vector3(direction * speed, 0f,0f);	
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules
	public void DetectObject(string tag, bool longVision)
	{
		if(tag == Tags.MAINCHAR){
			if(!longVision) SetSoldierState(SoldierState.Startled);
		}else if(tag == Tags.MONSTER){
			if(!longVision)	SetSoldierState(SoldierState.Panic);
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
		SetSoldierState(SoldierState.Chase);
		Chase();
		yield return new WaitForSeconds(chaseDuration);
		print("Stop Chasing Niwel");
		SetSoldierState(SoldierState.Patrol);
		flagChase = false;
	}

	IEnumerator Startling()
	{
		yield return new WaitForSeconds(startleDuration);
		SetSoldierState(SoldierState.Chase);
		flagStartled = false;
		StartCoroutine(ChasingNiwel());
	}

	IEnumerator Chasing()
	{
		yield return new WaitForSeconds(chaseDuration);
		flagChase = false;
		StartCoroutine(Idling());
	}
}
