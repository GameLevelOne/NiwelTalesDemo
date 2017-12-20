using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactables {
	public bool doorToOpen = true;

	public void OpenCloseDoor(){
		if(doorToOpen){
			transform.parent.GetComponent<Rooms> ().RevealRoom (false);
		} else{
			transform.parent.GetComponent<Rooms> ().RevealRoom (true);
		}
	}
}
