using System.Collections;
using UnityEngine;

public class SoldierBodyTriggerCollider : SoldierTriggerCollider {

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == tagCheck){
			soldier.InitIdle();
		}
	}
}
