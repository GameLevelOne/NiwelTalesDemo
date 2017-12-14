using System.Collections;
using UnityEngine;

public class MonsterInvestigateTriggerCollider : MonoBehaviour {
	
	public Monster monster;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == Tags.HIDEABLE){
			monster.DetectHidingPlace(other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == Tags.HIDEABLE){
			if(monster.hidingPlace.Count > 0) monster.hidingPlace.Remove(other.gameObject);
		}
	}
}
