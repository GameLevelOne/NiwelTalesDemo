using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierSoundTriggerCollider : MonoBehaviour {
	public Soldier soldier;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "VisualSound"){
			soldier.DetectObject (other.transform.parent.gameObject,false);
		}
	}
	
}
