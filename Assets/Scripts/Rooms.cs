using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour {

	public bool isLocked = true;
	public GameObject overlay;

	public void RevealRoom(bool isLocked){
		this.isLocked = isLocked;
		overlay.SetActive (isLocked);
	}
}
