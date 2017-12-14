using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log (other.tag);
		if(other.tag == Tags.MAINCHAR){
			Niwel obj = other.transform.parent.GetComponent<Niwel> ();
			obj.SetColliderObj (true,transform.parent.gameObject);
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == Tags.MAINCHAR) {
			Niwel obj = other.transform.parent.GetComponent<Niwel> ();
			obj.SetColliderObj (false,null);
		}
	}
}
