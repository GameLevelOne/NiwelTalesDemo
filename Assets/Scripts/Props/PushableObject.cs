using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {
	bool pushedByNiwel = false;
	Niwel niwelObj;

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == Tags.MAINCHAR){
			pushedByNiwel = true;
			niwelObj = other.gameObject.GetComponent<Niwel> ();
		} else if(other.tag == Tags.SOLDIER){
			if(pushedByNiwel){
				if(niwelObj!=null){
					niwelObj.DropObjectToSoldier ();
					pushedByNiwel = false;
					niwelObj = null;
				}
			}
		}
	}

//	void OnTriggerExit2D(Collider2D other){
//		
//	}
}
