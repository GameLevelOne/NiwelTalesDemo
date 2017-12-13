using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierVisionTriggerCollider : MonoBehaviour {
	[Header("VisionTriggerCollider Attributes")]
	public Soldier soldier;
	public string niwelTag;
	public string monsterTag;
	public bool longVision;

	[Header("Do Not Modify")]
	public List<GameObject> hidingPlace = new List<GameObject>();

	protected void OnTriggerEnter2D(Collider2D other)
	{
		if(!longVision && other.tag == Tags.HIDEABLE){
			hidingPlace.Add(other.gameObject);
		}

		Trigger(other.tag);
	}

	protected void OnTriggerExit2D(Collider2D other)
	{
		if(!longVision && other.tag == Tags.HIDEABLE){
			hidingPlace.Remove(other.gameObject);
		}
	}

	protected void Trigger (string otherTag)
	{
		soldier.DetectObject(otherTag,longVision);
	}
}
