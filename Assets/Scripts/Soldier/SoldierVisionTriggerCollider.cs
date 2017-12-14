using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierVisionTriggerCollider : MonoBehaviour {
	[Header("VisionTriggerCollider Attributes")]
	public Soldier soldier;
	public string niwelTag;
	public string monsterTag;
	public bool longVision;

	protected void OnTriggerEnter2D(Collider2D other)
	{
		 if(!longVision){
			Trigger(other.gameObject);
		}
	}

	protected void Trigger (GameObject otherObj)
	{
		soldier.DetectObject(otherObj,longVision);
	}
}
