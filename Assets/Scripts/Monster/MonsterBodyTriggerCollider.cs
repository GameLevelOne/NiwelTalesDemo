using System.Collections;
using UnityEngine;

public class MonsterBodyTriggerCollider : MonsterTriggerCollider {

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == tagCheck){
//			print("SDJKLSAJDLKSAJDLKASJDLKASJLKDASJLKDASJKLDASJ");
			monster.currentAttackTarget = other.transform.parent.gameObject;
			monster.InitAttack();
		}
	}

	protected override void OnTriggerExit2D (Collider2D other)
	{
		
	}


}
