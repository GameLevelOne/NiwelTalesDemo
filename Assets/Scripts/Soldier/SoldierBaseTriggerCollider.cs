using System.Collections;
using UnityEngine;

public class SoldierBaseTriggerCollider : MonoBehaviour {
	[Header("Base Attribute")]
	public Soldier soldier;
	public string targetTag;

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == targetTag){
			Trigger();
		}
	}

	protected virtual void Trigger()
	{
		
	}
}
