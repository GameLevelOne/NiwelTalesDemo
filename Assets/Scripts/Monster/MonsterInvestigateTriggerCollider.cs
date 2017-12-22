using System.Collections;
using UnityEngine;

public class MonsterInvestigateTriggerCollider : MonsterTriggerCollider {

	protected override void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == tagCheck) {
//			print(other.name);
			monster.SetObject(other.gameObject);
		}
	}

	protected override void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == tagCheck) monster.RemoveObject(other.gameObject);
	}

}
