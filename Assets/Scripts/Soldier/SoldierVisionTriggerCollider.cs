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
		Trigger(other.gameObject);
	}

	protected void Trigger (GameObject otherObj)
	{
		if(otherObj.tag == Tags.MONSTER){
			soldier.DetectObject(otherObj,longVision);
		}else{
			if(!longVision){
				soldier.DetectObject(otherObj,longVision);
			}
		}

	}
}
