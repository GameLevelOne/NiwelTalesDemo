using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == Tags.MAINCHAR){
			Niwel obj = other.transform.GetComponent<Niwel> ();
			obj.SetHideFlag (true);
		}
	} 

	void OnTriggerExit2D(Collider2D other){
		if(other.tag == Tags.MAINCHAR){
			Niwel obj = other.transform.GetComponent<Niwel> ();
			obj.SetHideFlag (false);
		}
	}
}
