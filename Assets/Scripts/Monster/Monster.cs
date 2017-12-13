using System.Collections;
using UnityEngine;

public enum MonsterState
{
	Idle,
	Patrol,
	CheckHidingPlace,
	Chase,
	Eat
}

public enum MonsterAnimationState
{
	Appear,
	Dissapear,
	Idle,
	CheckHidingPlace,
	Chase,
	Eat
}

public class Monster : MonoBehaviour {
	#region attributes
	[Header("Monster Attributes")]
	public Animator thisAnim;
	public Rigidbody2D thisRigidbody;

	[Header("Custom Attributes")]
	public float monsterAppearDuration = 10f;
	public MonsterState monsterState = MonsterState.Idle;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	public void Init()
	{
		
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics

	void CheckHidingPlace(){}
	void Patrol(){}
	void Chase(){}
	void Eat(){}


	void Dissapear()
	{
		
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
	
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
	void Update()
	{
		if(monsterState == MonsterState.Idle){
			
		}else if(monsterState == MonsterState.CheckHidingPlace){
			
		}else if(monsterState == MonsterState.Patrol){
			
		}else if(monsterState == MonsterState.Chase){
			
		}else if(monsterState == MonsterState.Eat){
			
		}
	}

	IEnumerator Appearing()
	{
		yield return new WaitForSeconds(monsterAppearDuration);

	}
}
