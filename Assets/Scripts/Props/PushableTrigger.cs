using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTrigger : MonoBehaviour {
	public bool pushRight;

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == Tags.MAINCHAR){
			Niwel obj = other.transform.GetComponent<Niwel> ();
			obj.SetPushedObj (transform.parent.gameObject, true, pushRight);
		}
	} 

	void OnTriggerExit2D(Collider2D other){
		if(other.tag == Tags.MAINCHAR){
			Niwel obj = other.transform.GetComponent<Niwel> ();
			obj.SetPushedObj (null, false, pushRight);
		}
	}
}
