using System.Collections;
using UnityEngine;

public class MonsterTriggerCollider : MonoBehaviour {
	public string tagCheck;
	protected Monster monster;

	protected void Awake()
	{
		monster = transform.parent.GetComponent<Monster>();
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == tagCheck) monster.SetObject(other.gameObject);
	}

	protected virtual void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == tagCheck) monster.SetObject(other.gameObject);
	}
}
