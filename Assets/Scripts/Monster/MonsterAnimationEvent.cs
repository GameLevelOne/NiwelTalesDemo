using System.Collections;
using UnityEngine;

public class MonsterAnimationEvent : MonoBehaviour {
	public Monster monster;

	void MoveMonsterForward()
	{
		Vector3 monsterPos = monster.transform.position;
		float direction = monster.transform.localScale == monster.vLeft ? -1f : 1f;
		monster.transform.position = new Vector3(monsterPos.x + (direction * monster.targetNearbyDistanceBeforeAttacking),monsterPos.y,monsterPos.z);
	}
}
