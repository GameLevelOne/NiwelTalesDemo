using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerMeter : StressMeter {

	float hungerTickDuration = 30f;

	void Start(){
		base.Init ();
		StartCoroutine (TickHungerMeter ());
	}

	IEnumerator TickHungerMeter(){
		while(true){
			ModMeter (-1);
			yield return new WaitForSeconds (hungerTickDuration);
		}
	}
}
