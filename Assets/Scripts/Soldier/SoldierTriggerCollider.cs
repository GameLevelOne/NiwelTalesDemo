using System.Collections;
using UnityEngine;

public class SoldierTriggerCollider : MonoBehaviour {
	public string tagCheck;

	protected Soldier soldier;

	protected void Awake()
	{
		soldier = transform.parent.GetComponent<Soldier>();
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == tagCheck) soldier.SetObject(other.gameObject);
	}

	protected virtual void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == tagCheck) soldier.RemoveObject(other.gameObject);
	}
}