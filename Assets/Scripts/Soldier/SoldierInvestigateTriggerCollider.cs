using System.Collections;
using UnityEngine;

public class SoldierInvestigateTriggerCollider : MonoBehaviour {
	public Soldier soldier;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == Tags.HIDEABLE){
//			soldier.DetectHidingPlace(other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == Tags.HIDEABLE){
//			soldier.hidingPlace.Remove(other.gameObject);
		}
	}
}
