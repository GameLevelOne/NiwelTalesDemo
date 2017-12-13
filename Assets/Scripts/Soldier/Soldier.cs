using System.Collections;
using UnityEngine;

public enum SoldierState{
	Idle,
	Patrol,
	Startled,
	Chase,
	Panic,
}



public class Soldier : MonoBehaviour {
	#region attributes
	[Header("Soldier Attributes")]
	public Animator thisAnim;
	public Rigidbody2D thisRigidbody;
	public Collider2D thisCollider;

	public SoldierBaseTriggerCollider bodyTriggerCollider;
	public SoldierBaseTriggerCollider visionShortTriggerCollider;
	public SoldierBaseTriggerCollider visionLongTriggerCollider;

	[Header("Custom Attributes")]
	public float speed = 0.025f;
	public float chaseDuration = 10f;
	public float startleDuration = 3f;

	[Header("Do Not Modify")]
	public SoldierState soldierState = SoldierState.Idle;

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
		if(soldierState == SoldierState.Idle){
			if(transform.localScale.x > 0f){
				transform.localScale = vLeft;
			}else if(transform.localScale.x < 0f){
				transform.localScale = vRight;
			}
			SetSoldierState(SoldierState.Patrol);
			thisAnim.SetInteger("State",(int)soldierState);
		}
	}
	#region AnimationEvent

	#endregion
	void SetSoldierState(SoldierState state)
	{
		if(state != soldierState) soldierState = state;
	}

	Vector3 getSpeed()
	{
		return new Vector3(transform.localScale.x * speed, 0f,0f);	
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules
	public void Stay(bool niwelDetected, float duration)
	{
		if(soldierState == SoldierState.Patrol){
			SetSoldierState(SoldierState.Idle);
			thisAnim.SetInteger("State",(int)soldierState);
			StartCoroutine(Staying(niwelDetected,duration));
		}
	}



	public void DetectObject(string tag, bool longVision)
	{
		if(tag == Tags.NIWEL){
			Stay(true,1f);
		}else if(tag == Tags.MONSTER){
			Startled();
		}
	}

	void Chase()
	{
		print("Soldier is chasing Niwel");

	}

	public void Startled()
	{
		if(soldierState != SoldierState.Startled){
			print("Soldier is startled");
			SetSoldierState(SoldierState.Startled);
			thisAnim.SetInteger("State",(int)soldierState);
		}
	}

	void Shoot()
	{
		//tembak dor dor
		print("Dor");
	}

	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	void Update()
	{
		if(soldierState == SoldierState.Patrol)
		{
			transform.Translate(getSpeed());
		}
	}
		
	IEnumerator Staying(bool niwelDetected, float duration)
	{
		yield return new WaitForSeconds(duration);
		if(niwelDetected){
			StartCoroutine(ChasingNiwel());
		}else{
			Patrol();
		}
	}

	IEnumerator ChasingNiwel()
	{
		SetSoldierState(SoldierState.Chase);
		Chase();
		yield return new WaitForSeconds(chaseDuration);
		print("Stop Chasing Niwel");
		Stay(false,2f);

	}

	IEnumerator Startling()
	{
		SetSoldierState(SoldierState.Startled);
		yield return new WaitForSeconds(startleDuration);
	}
}
