using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierGrabTriggerCollider : MonoBehaviour {
	public Soldier soldier;
	void OnTriggerEnter2D(Collider2D other)
	{
		print (other.name);
		if(other.tag == Tags.MAINCHAR && soldier.soldierState == SoldierState.Chase){
			soldier.InitGrabNiwel();
		}
	}
}
