using System.Collections;
using UnityEngine;

public class SoldierVisionTriggerCollider : SoldierBaseTriggerCollider {
	[Header("VisionTriggerCollider Attributes")]
	public string niwelTag;
	public string monsterTag;
	public bool longVision;

	protected override void OnTriggerEnter2D(Collider2D other)
	{
		Trigger(other.tag);
	}

	protected void Trigger (string otherTag)
	{
		soldier.DetectObject(otherTag,longVision);
	}
}
